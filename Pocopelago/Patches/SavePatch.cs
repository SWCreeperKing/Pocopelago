using System.Reflection;
using HarmonyLib;
using Pocopelago.Archipelago;
using UnityEngine;

namespace Pocopelago.Patches;

[PatchAll]
public static class SavePatch
{
    // [HarmonyPatch(typeof(FBPP), "GetSaveFilePath"), HarmonyPostfix]
    // public static void PatchDir(ref string __result)
    // {
    //     try
    //     {
    //         Core.Log.Msg("trying to set save folder/file");
    //         typeof(FBPP).GetMethod("CheckForInit", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, null);
    //         __result = "Archipelago/ap_id.txt";
    //     }
    //     catch (Exception e)
    //     {
    //         Core.Log.Error(e);
    //     }
    // }
    
    [HarmonyPatch(typeof(FBPP), "Start"), HarmonyPrefix]
    public static void PatchDir(FBPPConfig config)
    {
        try
        {
            config.SaveFilePath =  Path.Combine(Directory.GetCurrentDirectory(), "Archipelago",  ClownClient.GameUUID);
            config.SaveFileName = "ap_id";
            if (!Directory.Exists(config.SaveFilePath)) Directory.CreateDirectory(config.SaveFilePath);
            Core.Log.Msg($"trying to set save folder/file: [{config.SaveFilePath}/{config.SaveFileName}]");
            if (Core.PlayerController is not null) Core.PlayerController.PullInventoryFromSave();
        }
        catch (Exception e)
        {
            Core.Log.Error(e);
        }
    }
}