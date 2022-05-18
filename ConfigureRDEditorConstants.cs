using RDLevelEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RDVertPlugin
{
    public static class ConfigureRDEditorConstants
    {
        static Dictionary<LevelEventType, Tab> oldConstants = new Dictionary<LevelEventType, Tab>();
        static Dictionary<LevelEventType, KeyCode> oldKeyCodes = new Dictionary<LevelEventType, KeyCode>();

        static bool weAreVerted = false;

        // this is not called "PathRDEditorConstants"
        // because we're not actually "patching"
        // we're just mutating it for real.
        public static void Configure()
        {
            if (!weAreVerted)
            {
                oldConstants = RDEditorConstants.levelEventTabs.ToDictionary(entry => entry.Key, entry => entry.Value);
                oldKeyCodes = RDEditorConstants.eventKeyCodes.ToDictionary(entry => entry.Key, entry => entry.Value);
                RDEditorConstants.levelEventTabs.Add((LevelEventType)PatchLevelEventType.RunScript, Tab.Actions);
                RDEditorConstants.eventKeyCodes.Add((LevelEventType)PatchLevelEventType.RunScript, UnityEngine.KeyCode.Home);
            }
            weAreVerted = true;
        }

        public static void Unconfigure()
        {
            RDEditorConstants.levelEventTabs = oldConstants;
            RDEditorConstants.eventKeyCodes = oldKeyCodes;
            weAreVerted = false;
        }
    }
}
