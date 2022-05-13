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
                __result = "Project VERT :eyes:";
                return false;
            }
            return true;
        }
    }
}
