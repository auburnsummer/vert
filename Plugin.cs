using BepInEx;
using BepInEx.Logging;
using Miniscript;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

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

            Harmony.CreateAndPatchAll(typeof(PatchScnMenu));
            Harmony.CreateAndPatchAll(typeof(PatchRDString));
            Harmony.CreateAndPatchAll(typeof(PatchScnLogo));
        }

        private void OnDestroy()
        {
            Harmony.UnpatchAll();
            NowHijacking = false;
        }
    }
}
