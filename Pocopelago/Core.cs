using MelonLoader;
using Pocopelago.Archipelago;
using UnityEngine;
using Werepelago.Archipelago;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(Pocopelago.Core), "Pocopelago", "0.1.0", "SW_CreeperKing", null)]
[assembly: MelonGame("Micah Boursier", "Poco")]

namespace Pocopelago;

public class Core : MelonMod
{
    public const string DataFolder = "Mods/SW_CreeperKing.Pocopelago/Data";
    public static MelonLogger.Instance Log;
    public static GameObject StartButton;
    public static WorldController WorldController;
    public static PlayerController PlayerController;

    public override void OnInitializeMelon()
    {
        Log = LoggerInstance;

        ClownClient.Init();
        ClownClient.LocationDictionary = File.ReadAllLines($"{DataFolder}/locations.txt")
                                             .Select(line => line.Split(':'))
                                             .ToDictionary(arr => arr[1], arr => arr[0]);
        
        ClownClient.ItemsGiven.AddFlags(File.ReadAllLines($"{DataFolder}/items.txt"));
        ClownClient.ItemsRemoved.AddFlags(File.ReadAllLines($"{DataFolder}/items.txt"));
        ClownClient.ItemBlockers = File.ReadAllLines($"{DataFolder}/blockers.txt").Select(s => s.Split(':'))
                                       .ToDictionary(arr => arr[0], arr => arr[1].Split(','));

        Log.Msg("Loaded Data Files");

        var classesToPatch = MelonAssembly.Assembly.GetTypes()
                                          .Where(t => t.GetCustomAttributes(typeof(PatchAllAttribute), false).Any())
                                          .ToArray();

        Log.Msg($"Loading [{classesToPatch.Length}] Class patches");

        foreach (var patch in classesToPatch)
        {
            HarmonyInstance.PatchAll(patch);

            Log.Msg($"Loaded: [{patch.Name}]");
        }

        LoggerInstance.Msg("Initialized.");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        // Log.Msg($"Scene Loaded: [{sceneName}]");
        if (sceneName is not "scn_Initial")
        {
            if (PlayerController is null)
            {
                PlayerController = Object.FindAnyObjectByType<PlayerController>();
                PlayerController.PullInventoryFromSave();
            }
            return;
        }

        var panel = GameObject.Find("UICanvas/MainMenu/MainMenuSidePanel");
        StartButton = panel.GetChild(1);
        var @continue = panel.GetChild(2);

        if (!ClownClient.Client.IsConnected)
        {
            StartButton.SetActive(false);
            @continue.SetActive(false);
        }

        WorldController = Object.FindObjectOfType<WorldController>();

        var obj = new GameObject("AP Menu");
        obj.AddComponent<APGui>();
    }

    public override void OnUpdate() => ClownClient.Update();
}

[AttributeUsage(AttributeTargets.Class)]
public class PatchAllAttribute : Attribute;