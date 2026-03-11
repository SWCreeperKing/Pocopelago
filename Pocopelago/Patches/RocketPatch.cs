using HarmonyLib;
using Pocopelago.Archipelago;

namespace Pocopelago.Patches;

[PatchAll]
public static class RocketPatch
{
    [HarmonyPatch(typeof(RocketCutsceneStarter), "StartEndingCutscene"), HarmonyPrefix]
    public static void StartRocket()
    {
        ClownClient.Client.Goal();
    }
}