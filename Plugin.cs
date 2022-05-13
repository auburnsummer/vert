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
            Harmony.CreateAndPatchAll(typeof(PatchRDString));
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
    }
}
