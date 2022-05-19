using HarmonyLib;
using RDLevelEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RDVertPlugin
{
    public static class PatchScnEditor
    {
        static Dictionary<LevelEventType, Tab> oldConstants = new Dictionary<LevelEventType, Tab>();

        static GameObject runScriptThingo;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(scnEditor), "Start")]
        public static bool Start(scnEditor __instance)
        {
            Vert.Log.LogInfo("We are here now");
            // hmmmmmmmmmmmmm
            InspectorPanel[] componentsInChildren =  Resources.FindObjectsOfTypeAll<InspectorPanel>();
            Vert.Log.LogInfo(componentsInChildren.Length);
            foreach (InspectorPanel component in componentsInChildren)
            {
                Vert.Log.LogInfo(component.name);
            }

            return true;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(scnEditor), "SetLevelEventControlType")]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var codeMatcher = new CodeMatcher(instructions, il);

            // RD creates events using mscorlib System.Type.GetType(string) (i.e. RDLevelEditor.LevelEvent_{name})
            // we can, in Harmony, add new classes to the RDLevelEditor namespace, but the lookup doesn't work. why?
            // because Type.GetType only looks in the *executing assembly*, which is Assembly-CSharp.dll, and our code lives in RDVertPlugin.dll!
            // instead, we create a new version of GetType that works outside of Assembly-CSharp, and replace the call.

            return codeMatcher
                .MatchForward(false,
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(System.Type), "GetType", new Type[] { typeof(string) }))
                )
                .Set(OpCodes.Call, AccessTools.Method(typeof(Utils), "GetType", new Type[] { typeof(string) }))
                .InstructionEnumeration();
        }
    }
}
