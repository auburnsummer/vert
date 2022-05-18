using HarmonyLib;
using RDLevelEditor;
using System;
using System.Collections.Generic;
using System.Linq;
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

    }
}
