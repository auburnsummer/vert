using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RDLevelEditor;
using UnityEngine;

namespace RDVertPlugin
{
    public static class PatchResources
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Resources), "Load", new Type[] {typeof(string), typeof(Type)})]
        public static bool Load(ref UnityEngine.Object __result, string path)
        {
            if (path == "LevelEventControl_MiniScriptEditor")
            {
                Vert.Log.LogInfo("I am in my custom miniscript control constructor");
                UnityEngine.GameObject myCoolObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                myCoolObject.name = "OhWowHahaAwesome";
                LevelEventControl_Base panel = myCoolObject.AddComponent<LevelEventControl_MiniScriptEditor>();
                myCoolObject.AddComponent<RectTransform>();
                __result = myCoolObject;
                Vert.Log.LogInfo("okay here next");
                return false;
            }
            return true;
        }

    }
}
