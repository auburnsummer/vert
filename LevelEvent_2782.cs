using Miniscript;
using RDVertPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDLevelEditor
{
    public class LevelEvent_2782 : LevelEvent_Base
    {
        public string text;
        public string id;


        public Interpreter inte;

        public override void OnCreate()
        {
            base.OnCreate();
            this.id = System.Guid.NewGuid().ToString();
        }

        public override string Encode()
        {
            return base.EncodeBaseProperties(false) + 
                   RDEditorUtils.EncodeUnicodeString("text", this.text, false) + 
                   RDEditorUtils.EncodeUnicodeString("id", this.id, true);
        }


        public override void Decode(Dictionary<string, object> dict)
        { 
            base.Decode(dict);
            this.text = (dict["text"] as string);
            this.id = dict["id"] as string;
            this.executionTime = LevelEventExecutionTime.OnBar;
        }

        public override void CopyFromInternal(LevelEvent_Base levelEvent)
        {
            LevelEvent_2782 levelEvent_RunScript = (LevelEvent_2782)levelEvent;
            this.text = levelEvent_RunScript.text;
            this.id = System.Guid.NewGuid().ToString();
        }

        public override System.Collections.IEnumerator Prepare()
        {
            Singleton<ExecutorManager>.Instance.PrepareScriptForExecution(this.id, this.text);
            this.prepared = true;
            yield break;
        }


        public override void Run()
        {
            base.RunOnBeat(delegate
            {
                Singleton<ExecutorManager>.Instance.ActivateScript(this.id);
            }, false, null, -1, false, false);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
