using HarmonyLib;
using Pocopelago.Archipelago;

namespace Pocopelago.Patches;

[PatchAll]
public static class SkullPatch
{
    [HarmonyPatch(typeof(RatSkull), "Interact"), HarmonyPrefix]
    public static void GetSkull(RatSkull __instance)
    {
        // Core.Log.Msg($"Pickuped rat skull [skull.{__instance.SkullNumber}]");
        ClownClient.SendLocation($"skull.{__instance.SkullNumber}");
    }
}