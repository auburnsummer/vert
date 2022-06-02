using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDVertPlugin
{
    public static class PatchRDFile
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(RDFile), "WriteAllText")]
        public static bool WriteAllText(ref string path, string data, Encoding encoding = null)
        {
            // if we are hijacking (i.e. we are in VERT editor)
            // ...and we are writing an rdlevel
            // we should change the path to .rdlevel.vert before continuing.
            // this is to make it harder to open VERT files in the normal editor.
            if (!Vert.NowHijacking)
            {
                // just return true, which keeps the chain without touching anything.
                return true;
            }
            if (path.EndsWith(".rdlevel"))
            {
                // redirect it to vert instead, then pass it on
                path = String.Format("{0}.vert", path);
            }
            return true;
        }
    }
}
