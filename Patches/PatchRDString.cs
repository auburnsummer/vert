using HarmonyLib;

namespace RDVertPlugin
{
    public static class PatchRDString
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(RDString), "Get")]
        public static bool Get(string key, ref string __result)
        {
            if (key == "mainMenu.VertMenuOption")
            {
                __result = "Project VERT rdlevel version :eyes:";
                return false;
            }
            if (key == "editor.2782")
            {
                // 0x0ade = 2782
                __result = "Run Script";
                return false;
            }
            return true;
        }
    }
}
