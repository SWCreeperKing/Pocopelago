using HarmonyLib;

namespace Pocopelago.Patches;

[PatchAll]
public static class CerberusFix
{
    [HarmonyPatch(typeof(ItemPrompt), "GetSaveEntryName"), HarmonyPostfix]
    public static void FixInteraction(ItemPrompt __instance, ref string __result)
    {
        switch (__instance.requiredItem)
        {
            case "Dog Treat":
                __result = "ipp_scn_Cerberus_0.0";
                break;
            case "Pacifier":
                __result = "ipp_scn_Cerberus_0.1";
                break;
            case "Bug Steak":
                __result = "ipp_scn_Cerberus_0.2";
                break;
            default:
                return;
        }
    }
}