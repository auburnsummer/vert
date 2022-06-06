using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDVertPlugin
{
    public static class PatchScrConductor
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrConductor), "Update")]
        public static bool Update(scrConductor __instance)
        {
            // this is a prefix, so that if a script schedules an event to occur...
            // ...it will happen in the same frame instead of the next frame.
            // i think?? the way RD schedules events is cool but also i don't understand it at all

            Singleton<ExecutorManager>.Instance.DoUpdate();
            return true;
        }

    }
}
