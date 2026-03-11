using HarmonyLib;

namespace Pocopelago.Patches;

[PatchAll]
public static class OpenCutscenePatch
{
    [HarmonyPatch(typeof(OPNController), "BeginUnicycleQTE"), HarmonyPostfix]
    public static void EndOpening1(OPNController __instance)
    {
        __instance.BeginUnicycleCircles();
        __instance.FailUnicycle();
    }
    
    [HarmonyPatch(typeof(OPNController), "BeginBalloonQTE"), HarmonyPostfix]
    public static void EndOpening2(OPNController __instance) => __instance.FailBalloon();
    
    [HarmonyPatch(typeof(OPNController), "BeginHornQTE"), HarmonyPostfix]
    public static void EndOpening3(OPNController __instance) => __instance.FailHorn();
}