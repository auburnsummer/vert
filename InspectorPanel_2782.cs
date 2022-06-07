using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
    public enum RunScriptStateMachine
    {
        INACTIVE,
        ACTIVE
    }

    public class InspectorPanel_2782 : InspectorPanel
    {
        public InputField dialogue;
        public RunScriptStateMachine state = RunScriptStateMachine.INACTIVE;
        StreamReader reader;
        FileStream file;
        GameObject deleteButton;

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

            // add a new button. this is for editing the script in an external text editor.
            // it's easier to clone an existing one. we'll use the Delete button in the title.
            var pathToDeleteButtonObject = "Canvas/Level Editor Panel/MovementPanel/Left Panel (Inspector)/InspectorPanelTitle/DeleteButton";
            var deleteButtonObject = GameObject.Find(pathToDeleteButtonObject);
            // make a clone of it...
            deleteButton = GameObject.Instantiate(deleteButtonObject);
            deleteButton.name = "openExternButton";
            // attach the parent.
            deleteButton.transform.SetParent(this.transform, false);
            // change the text.
            deleteButton.GetComponentInChildren<RDStringToUIText>().key = "editor.openInExternalEditor";

            var newRect = deleteButton.GetComponent<RectTransform>();
            newRect.localPosition = newRect.localPosition.WithXY(50f, -114f); // by experimentation.
            newRect.sizeDelta = newRect.sizeDelta.WithX(130f);

            // we need to use DestroyImmediate here bc we are immediately creating a new button, and otherwise
            // unity will prevent us from attaching more than one Selectable.
            DestroyImmediate(deleteButton.GetComponent<Button>());

            var newButtonComp = deleteButton.AddComponent<Button>();

            newButtonComp.onClick.AddListener(onClick);

            base.Awake();
            this.dialogue.onEndEdit.RemoveAllListeners();
        }

        public void onClick()
        {
            // copy the file contents into a tempfile.
            LevelEvent_2782 evt = (LevelEvent_2782)this.currentLevelEvent;
            string filePath = Path.Combine(Application.persistentDataPath, String.Format("{0}.ms", evt.id));
            StreamWriter writer = new StreamWriter(filePath, false);
            writer.Write(this.dialogue.text);
            writer.Flush();
            writer.Close();

            // create a reader to poll the file for changes.
            file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            reader = new StreamReader(file, true);

            // change the button text.
            // unsure why, but it seems like changing the localisation key via the RDStringToUiText property doesn't work here, we change text directly.
            deleteButton.GetComponentInChildren<Text>().text = RDString.Get("editor.currentlyEditingInExternalEditor");
            this.Localize(this.transform);

            // deactivate the input field.
            this.dialogue.DeactivateInputField();

            // attempt to open the file.
            this.AttemptToOpenFile(filePath);

            // set state to active. this is checked in Update.
            this.state = RunScriptStateMachine.ACTIVE;
        }

        public void AttemptToOpenFile(string filePath)
        {
            RuntimePlatform platform = Application.platform;
            // sorry, only Windows for now.
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                new Process
                {
                    StartInfo = new ProcessStartInfo(String.Format(@"{0}", filePath))
                    {
                        UseShellExecute = true
                    }
                }.Start();
            }
        }

        public void Update()
        {
            if (this.state == RunScriptStateMachine.ACTIVE)
            {
                // LevelEvent_2782 evt = (LevelEvent_2782)this.currentLevelEvent;
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                reader.DiscardBufferedData();
                string text = reader.ReadToEnd();
                if (!String.Equals(text, this.dialogue.text))
                {
                    this.dialogue.text = text;
                }
            }
        }

        public override void UpdateUIInternal(LevelEvent_Base levelEvent)
        {
            LevelEvent_2782 levelEvent_2782 = (LevelEvent_2782)levelEvent;
            this.dialogue.text = levelEvent_2782.text;
            RDEditorUtils.UpdateUIText(this.dialogue.textComponent, this.dialogue.textComponent.text, false);
        }

        protected override void SaveInternal(LevelEvent_Base levelEvent)
        {
            // set the event's properties.
            LevelEvent_2782 evt = (LevelEvent_2782)levelEvent;
            evt.text = this.dialogue.text;

            // go back to inactive. a script can't be edited unless it is active in property panel.
            this.state = RunScriptStateMachine.INACTIVE;

            this.dialogue.ActivateInputField();

            // cleanup after ourselves.
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }
            if (file != null)
            {
                file.Close();
                file = null;
            }
            File.Delete(Path.Combine(Application.persistentDataPath, String.Format("{0}.ms", evt.id)));

            // change the button text back.
            deleteButton.GetComponentInChildren<Text>().text = RDString.Get("editor.openInExternalEditor");
            this.Localize(this.transform);


        }

    }
}
