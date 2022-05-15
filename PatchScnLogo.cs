using HarmonyLib;
using Miniscript;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RDVertPlugin
{
    /*
     * scnLogo is the seizure warning / early access menu at the start of the game.
     * Even when the game's released, the seizure warning will still need to be displayed.
     * Therefore, the probability scnLogo gets removed is pretty low.
     * In addition, after the initial launch, the player has no way to access scnLogo again naturally.
     */
    public static class PatchScnLogo
    {

        static scnBase scn;

        static Interpreter interpreter;
        static bool intrinsticsAdded = false;

        // setup the Miniscript interpreter: https://miniscript.org/files/MiniScript-Integration-Guide.pdf
        public static void SetUpInterpreter()
        {
            interpreter = new Interpreter();
            interpreter.standardOutput = (string s) => Vert.Log.LogInfo(s);
            interpreter.implicitOutput = (string s) => Vert.Log.LogInfo(s);
            interpreter.errorOutput = (string s) => Vert.Log.LogError(s);

            if (intrinsticsAdded)
            {
                return;
            }
            // intrinstics.
            // input(str key) -> bool
            // return true if given key is pressed, otherwise false.
            // note: miniscript booleans are actually numbers 1 and 0
            var f = Intrinsic.Create("input");
            f.AddParam("key");
            f.code = (context, partialResult) =>
            {
                string key = context.GetVar("key").ToString();
                if (Input.GetKey(key))
                {
                    return Intrinsic.Result.True;
                }
                return Intrinsic.Result.False;
            };


            intrinsticsAdded = true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(scnLogo), "Start")]
        public static bool Start(scnLogo __instance)
        {
            if (Vert.NowHijacking)
            {
                Vert.Log.LogInfo("Okay we should be hijacking now");
                Scene currentScene = SceneManager.GetActiveScene();
                GameObject[] objectsInScene = currentScene.GetRootGameObjects();
                // Remove anything that's not us.
                foreach (GameObject obj in objectsInScene)
                {
                    Vert.Log.LogInfo(obj.name);
                    if (obj.name != "scnLogo")
                    {
                        UnityEngine.Object.Destroy(obj);
                    }
                }
                // hmmm, do I have a scnBase somewhere? nice I do
                scnBase scnCurrent = __instance.scnCurrent;
                PatchScnLogo.scn = scnCurrent;

                SetUpInterpreter();

                // get the .ms file we've tucked away somewhere
                string bootstrapMsFile;
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RDVertPlugin.VertBootstrapper.main.ms"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    bootstrapMsFile = reader.ReadToEnd();
                }

                // and load that into the intepreter.
                interpreter.Reset(bootstrapMsFile);
                interpreter.Compile();

                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(scnLogo), "Update")]
        public static bool Update(scnLogo __instance)
        {
            if (Vert.NowHijacking)
            {
                try
                {
                    interpreter.RunUntilDone(0.01);
                }
                catch (MiniscriptException err)
                {
                    Vert.Log.LogError("Script error: " + err.Description());
                }
                return false;
            }
            return true;
        }
    }
}
