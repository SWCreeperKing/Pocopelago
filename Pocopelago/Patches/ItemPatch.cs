using HarmonyLib;
using Pocopelago.Archipelago;
using UnityEngine;

namespace Pocopelago.Patches;

[PatchAll]
public static class ItemPatch
{
    public const string ApPrefix = "ApItem_";

    [HarmonyPatch(typeof(PlayerController), "AcquireItem", [typeof(string), typeof(Vector2)]), HarmonyPrefix]
    public static bool AcquireItem(ref string n, Vector2 pos, List<string> ___inv)
    {
        if (!n.StartsWith(ApPrefix)) { ClownClient.SendLocation(n); }
        else { n = n[ApPrefix.Length..]; }

        if (!ClownClient.Items.Contains(n)) return false;
        if (n is not "Potato Battery" && ClownClient.ItemsRemoved[n]) return false;
        return !___inv.Contains(n);
    }

    [HarmonyPatch(typeof(PlayerController), "RemoveItem"), HarmonyPostfix]
    public static void RemoveItem(string n)
    {
        ClownClient.ItemsRemoved += n;
        FBPP.SetString("items_removed", ClownClient.ItemsRemoved);
    }
}