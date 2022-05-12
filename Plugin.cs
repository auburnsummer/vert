using BepInEx;
using Miniscript;

namespace RDVertPlugin
{
    [BepInPlugin("cafe.rhythm.vert", "Rhythm Doctor VERT Plugin", "0.0.1")]
    [BepInProcess("Rhythm Doctor.exe")]
    public class RDVertPlugin : BaseUnityPlugin
    {
        public Interpreter interpreter;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            interpreter = new Interpreter();
            interpreter.standardOutput = (string s) => Logger.LogInfo(s);
            interpreter.implicitOutput = (string s) => Logger.LogDebug(s);
            interpreter.errorOutput = (string s) => Logger.LogError(s);

            PrepareScript("print \"Hello World4\"");
            interpreter.RunUntilDone(0.01);
        }

        private void PrepareScript(string sourceCode)
        {
            interpreter.Reset(sourceCode);
            interpreter.Compile();
        }
    }
}
