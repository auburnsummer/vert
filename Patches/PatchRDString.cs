using HarmonyLib;

namespace RDVertPlugin
{
    public static class PatchRDString
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(RDString), "Get")]
        public static bool Get(string key, ref string __result)
        {
            // Patch strings we are using.
            // in the future, we can check language here as well.
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
            if (key == "editor.openInExternalEditor")
            {
                __result = "Open in external editor";
                return false;
            }
            if (key == "editor.currentlyEditingInExternalEditor")
            {
                __result = "Editing externally...";
                return false;
            }
            return true;
        }
    }
}
