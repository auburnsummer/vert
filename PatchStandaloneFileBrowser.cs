using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using SFB;

namespace RDVertPlugin
{
    public static class PatchStandaloneFileBrowser
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StandaloneFileBrowser), "OpenFilePanel", new Type[] { typeof(string), typeof(string), typeof(ExtensionFilter[]), typeof(bool) } )]
        public static bool OpenFilePanel(string title, string directory, ref ExtensionFilter[] extensions, bool multiselect)
        {
            bool hasRdlevelFlag = false;
            foreach (ExtensionFilter filter in extensions)
            {
                foreach (string ext in filter.Extensions)
                {
                    if (String.Equals(ext, "rdlevel"))
                    {
                        hasRdlevelFlag = true;
                    }
                }
            }
            if (hasRdlevelFlag)
            {
                extensions[0] = new ExtensionFilter("Hi you are in a VERT menu instead", new string[] { "rdlevel.vert" });
            }
            return true;
        }
    }
}
