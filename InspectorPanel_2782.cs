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
            // Remove the "show/hide" box.
            this.dialogue = this.gameObject.GetComponentInChildren<InputField>();
            Destroy(this.gameObject.transform.Find("show").gameObject);

            // bump up the character limit...
            this.dialogue.characterLimit = Int32.MaxValue;

            // also, while i'm here, might as well change the colours a bit.
            var colors = this.dialogue.colors;
            colors.normalColor = "DEFAEE".HexToColor();
            colors.highlightedColor = "F0FCF7".HexToColor();
            this.dialogue.colors = colors;

            // Remove the text...
            GameObject comment = this.gameObject.transform.Find("comment").gameObject;
            var text = comment.GetComponent<Text>();
            Destroy(text);

            // and shift up the box.
            comment.transform.TranslateY(15f);

            base.Awake();
            this.dialogue.onEndEdit.RemoveAllListeners();
        }

        public override void UpdateUIInternal(LevelEvent_Base levelEvent)
        {
            LevelEvent_2782 levelEvent_2782 = (LevelEvent_2782)levelEvent;
            this.dialogue.text = levelEvent_2782.text;
            RDEditorUtils.UpdateUIText(this.dialogue.textComponent, this.dialogue.textComponent.text, false);
        }

        protected override void SaveInternal(LevelEvent_Base levelEvent)
        {
            LevelEvent_2782 levelEvent_2782 = (LevelEvent_2782)levelEvent;
            levelEvent_2782.text = this.dialogue.text;
        }

        public InputField dialogue;
    }
}
