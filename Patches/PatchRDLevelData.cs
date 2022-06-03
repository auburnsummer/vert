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
    public static class PatchRDLevelData
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(RDLevelData), "Decode")]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            // see PatchScnEditor.cs, same thing.
            var codeMatcher = new CodeMatcher(instructions, il);

            return codeMatcher
                .MatchForward(false,
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(System.Type), "GetType", new Type[] { typeof(string) }))
                )
                .Set(OpCodes.Call, AccessTools.Method(typeof(Utils), "GetType", new Type[] { typeof(string) }))
                .InstructionEnumeration();
        }
    }
}
