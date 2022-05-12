using BepInEx;
using BepInEx.Logging;
using Miniscript;
using HarmonyLib;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace RDVertPlugin
{
    [BepInPlugin("cafe.rhythm.vert", "Rhythm Doctor VERT Plugin", "0.0.1")]
    [BepInProcess("Rhythm Doctor.exe")]
    public class RDVertPlugin : BaseUnityPlugin
    {
        public Interpreter interpreter;
        internal static new ManualLogSource Log;

        private void Awake()
        {
            RDVertPlugin.Log = base.Logger;
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            interpreter = new Interpreter();
            interpreter.standardOutput = (string s) => Logger.LogInfo(s);
            interpreter.implicitOutput = (string s) => Logger.LogDebug(s);
            interpreter.errorOutput = (string s) => Logger.LogError(s);

            PrepareScript("print \"Hello World4\"");
            interpreter.RunUntilDone(0.01);

            Harmony.CreateAndPatchAll(typeof(PatchScnMenu));
        }

        private void OnDestroy()
        {
            Harmony.UnpatchAll();
        }

        private void PrepareScript(string sourceCode)
        {
            interpreter.Reset(sourceCode);
            interpreter.Compile();
        }

        public static class PatchScnMenu
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(scnMenu), "Awake")]
            public static void Awake(scnMenu __instance, ref Text[] ___optionsText)
            {
                ___optionsText[6].text = "Project VERT :eyes:";
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(scnMenu), "SelectOption")]
            public static bool SelectOption(scnMenu __instance, int ___currentOption)
            {
                if (___currentOption == 6)
                {
                    RDVertPlugin.Log.LogInfo("Block entering the OST");
                    return false;
                }
                return true;
            }
        }
    }
}
