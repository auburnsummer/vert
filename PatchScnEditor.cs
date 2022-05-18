using HarmonyLib;
using RDLevelEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RDVertPlugin
{
    public static class PatchScnEditor
    {
        static Dictionary<LevelEventType, Tab> oldConstants = new Dictionary<LevelEventType, Tab>();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(scnEditor), "Start")]
        public static bool Start(scnEditor __instance)
        {
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
