using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
    public class InspectorPanel_2782 : InspectorPanel
    {
        public new void Awake()
        {
            RDVertPlugin.Vert.Log.LogInfo("whoa okay now we're in here");
            this.dialogue = this.gameObject.GetComponentInChildren<InputField>();
            this.show = this.gameObject.GetComponentInChildren<Toggle>();
            this.auto = false;
            base.Awake();
            this.dialogue.onEndEdit.RemoveAllListeners();
            this.show.onValueChanged.RemoveAllListeners();
        }

        public override void UpdateUIInternal(LevelEvent_Base levelEvent)
        {
            LevelEvent_2782 levelEvent_2782 = (LevelEvent_2782)levelEvent;
            this.dialogue.text = levelEvent_2782.text;
            this.show.isOn = levelEvent_2782.show;
            RDEditorUtils.UpdateUIText(this.dialogue.textComponent, this.dialogue.textComponent.text, false);
        }

        protected override void SaveInternal(LevelEvent_Base levelEvent)
        {
            LevelEvent_2782 levelEvent_2782 = (LevelEvent_2782)levelEvent;
            levelEvent_2782.text = this.dialogue.text;
            levelEvent_2782.show = this.show.isOn;
        }

        public InputField dialogue;

        public Toggle show;
    }
}
