using HarmonyLib;
using RDLevelEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RDVertPlugin
{
    public static class PatchScnEditor
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scnEditor), "Start")]
        public static bool Start(scnEditor __instance)
        {
            Vert.Log.LogInfo("We are here now");
            // honestly no idea what I was doing here.
            var pathToContentGameObject = "Canvas/Level Editor Panel/MovementPanel/Left Panel (Inspector)/InspectorPanelManager/Viewport/Content";
            var contentGameObject = GameObject.Find(pathToContentGameObject);
            var contentGameObjectTransform = contentGameObject.transform;
            Vert.Log.LogInfo(contentGameObject.name);
            Vert.Log.LogInfo("=====================");
            InspectorPanel[] componentsInChildren = Resources.FindObjectsOfTypeAll<InspectorPanel>();
            Vert.Log.LogInfo(componentsInChildren.Length);
            foreach (InspectorPanel component in componentsInChildren)
            {
                if (component.name == "Comment")
                {
                    // we're using Comment as a base to construct our panel.
                    var templateGameObject = component.gameObject;
                    var ourNewGameObject = GameObject.Instantiate(templateGameObject, contentGameObjectTransform);
                    // now, ourNewGameObject has everything that templateGameObject has.....
                    // afaik, all gameobjects have a transform that indicate that gameobject's position.
                    // this has one of those too! the transform is right where a panel should be, so we don't touch it.
                    // since we cloned Comment, it's also got a InspectorPanel_Comment attached. let's yeet that first...
                    ourNewGameObject.name = "RunScript";
                    Component.Destroy(ourNewGameObject.GetComponent<InspectorPanel>());
                    // and attach our new inspector panel!
                    InspectorPanel_2782 panel = (InspectorPanel_2782)ourNewGameObject.AddComponent(typeof(InspectorPanel_2782));

                }
                Vert.Log.LogInfo(component.name);
                Vert.Log.LogInfo(component.gameObject.name);
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(scnEditor), "CreateEventControl")]
        public static bool CreateEventControl(scnEditor __instance, LevelEvent_Base levelEvent, Tab tab, bool skipSaveState)
        {
            Vert.Log.LogInfo("CREATE EVENT CONTROL");
            Vert.Log.LogInfo(levelEvent.CustomControlName());
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

/*        [HarmonyPrefix]
        [HarmonyPatch(typeof(scnEditor), "AddNewEventControl")]
        public static bool AddNewEventControl(scnEditor __instance, LevelEventControl_Base eventControl, Tab tab)
        {
            return true;
            *//*Vert.Log.LogInfo("ADD NEW EVENT CONTROL");
            Vert.Log.LogInfo(eventControl.ToString());
            TabSection tabSection = __instance.tabSections[(int)tab];
            Vert.Log.LogInfo("1");
            eventControl.tabSection = tabSection;
            Vert.Log.LogInfo("2");
            eventControl.container.Add(eventControl);
            Vert.Log.LogInfo("3");

            eventControl.UpdateUI();
            Vert.Log.LogInfo("4");

            __instance.eventControls.Add(eventControl);
            Vert.Log.LogInfo("5");

            Transform parent;
            parent = tabSection.containerTransform;
            Vert.Log.LogInfo("6");

            eventControl.GetComponent<RectTransform>().SetParent(parent, false);
            Vert.Log.LogInfo("7");

            return false;*//*
        }*/
    }
}
