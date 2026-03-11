using HarmonyLib;
using Pocopelago.Archipelago;

namespace Pocopelago.Patches;

[PatchAll]
public static class PhotoPatch
{
    [HarmonyPatch(typeof(PhotoPiece), "Interact"), HarmonyPrefix]
    public static void PickPhoto(PhotoPiece __instance)
    {
        // Core.Log.Msg($"Photo piece: [pic.{__instance.pieceID}]");
        ClownClient.SendLocation($"pic.{__instance.pieceID}");
    }
}