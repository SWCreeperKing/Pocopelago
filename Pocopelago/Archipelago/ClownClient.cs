using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using CreepyUtil.Archipelago;
using CreepyUtil.Archipelago.ApClient;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Pocopelago.Patches;
using Werepelago.Archipelago;

namespace Pocopelago.Archipelago;

public static class ClownClient
{
    public static ApClient Client = new(new TimeSpan(0, 1, 0));
    public static ApData Data = new();
    public static Dictionary<string, string> LocationDictionary;
    public static Dictionary<string, string[]> ItemBlockers = [];
    public static List<string> Items = [];
    public static string GameUUID = "Generic";
    public static LoseFlag<string> ItemsGiven;
    public static LoseFlag<string> ItemsRemoved;
    public static Dictionary<string, string[]> ItemHolders = [];

    public static void Init()
    {
        if (File.Exists("ApConnection.json"))
        {
            Data = JsonConvert.DeserializeObject<ApData>(File.ReadAllText("ApConnection.json").Replace("\r", ""));
        }

        Client.OnConnectionLost += () =>
        {
            // if (Core.Scene is "Game") GameUI.Instance.IngameMenuReturnToTitle();
            Core.Log.Error("Lost Connection to Ap");
        };

        Client.OnConnectionEvent += _ =>
        {
            try
            {
                ItemHolders.Clear();
                Items.Clear();
                GameUUID = (string)Client.SlotData["uuid"];
                ItemsGiven.SetFlag(Client.GetFromStorage("items_got", def: 0ul));
                ItemsRemoved.SetFlag(Client.GetFromStorage("items_removed", def: 0ul));

                try { Core.WorldController.CallPrivateMethod("Awake"); }
                catch (Exception e) { Core.Log.Error(e); }
            }
            catch (Exception e) { Core.Log.Error(e); }
        };

        Client.OnConnectionErrorReceived += (e, _) => Core.Log.Error(e);
        Client.OnErrorReceived += e => Core.Log.Error(e);
    }

    [CanBeNull]
    public static string[] TryConnect(string addressPort, string password, string slotName)
    {
        var addressSplit = addressPort.Split(':');

        if (addressSplit.Length != 2) return ["Address Field is incorrect"];
        if (!int.TryParse(addressSplit[1], out var port)) return ["Port is incorrect"];

        var login = new LoginInfo(port, slotName, addressSplit[0], password);

        return Client.TryConnect(login, "Poco", ItemsHandlingFlags.AllItems);
    }

    public static void SaveFile() => File.WriteAllText("ApConnection.json", JsonConvert.SerializeObject(Data));

    public static void Update()
    {
        try
        {
            if (Client is null) return;
            Client.UpdateConnection();

            if (!Client.IsConnected) return;
            if (Core.PlayerController is null) return;
            var items = Client.GetOutstandingItems();
            if (!items.Any()) return;
            foreach (var item in items) HandleItems(item);
            Client.SendToStorage("items_got", (ulong)ItemsGiven);
        }
        catch (Exception e) { Core.Log.Error(e); }
    }

    public static void HandleItems(ItemInfo item)
    {
        if (item.Flags is not ItemFlags.Advancement) return;
        GiveItem(item.ItemName);
    }

    public static void GiveItem(string item)
    {
        if (ItemBlockers.ContainsKey(item) && ItemBlockers[item].Any(Client.MissingLocations.Contains))
        {
            ItemHolders[item] = ItemBlockers[item];
            return;
        }
        
        Items.Add(item);
        if (ItemsGiven[item]) return;
        Core.PlayerController.AcquireItem($"{ItemPatch.ApPrefix}{item}");
        ItemsGiven += item;
    }

    public static void SendLocation(string location)
    {
        var loc = LocationDictionary[location];
        Client.SendLocation(loc);
        foreach (var (itemBlocked, blockers) in ItemHolders.ToArray())
        {
            if (!blockers.Contains(loc) || blockers.Any(Client.MissingLocations.Contains)) continue;
            GiveItem(itemBlocked);
            ItemHolders.Remove(itemBlocked);
        }
    }
}