using BepInEx;
using BepInEx.Logging;
using Miniscript;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RDVertPlugin
{
    [BepInPlugin("cafe.rhythm.vert", "Rhythm Doctor VERT Plugin", "0.0.1")]
    [BepInProcess("Rhythm Doctor.exe")]
    public class Vert : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        internal static bool NowHijacking = false;

        private void Awake()
        {
            Vert.Log = base.Logger;
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            Type[] patches = new Type[]
            {
                typeof(PatchScnMenu),
                typeof(PatchRDString),
                typeof(PatchRDFile),
                typeof(PatchStandaloneFileBrowser),
                typeof(PatchScnEditor),
                typeof(PatchLevelEvent_Base)
            };

            foreach (Type patch in patches)
            {
                Harmony.CreateAndPatchAll(patch);
            }
        }

        private void OnDestroy()
        {
            Harmony.UnpatchAll();
            NowHijacking = false;
        }
    }
}
