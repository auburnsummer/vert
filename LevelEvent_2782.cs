using Miniscript;
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

        public bool show;

        public Interpreter inte;

        public override void OnCreate()
        {
            base.OnCreate();
            this.id = System.Guid.NewGuid().ToString();
        }

        public override string Encode()
        {
            return base.EncodeBaseProperties(false) + 
                   RDEditorUtils.EncodeBool("show", this.show, false) +
                   RDEditorUtils.EncodeUnicodeString("text", this.text, false) + 
                   RDEditorUtils.EncodeUnicodeString("id", this.id, true);
        }


        public override void Decode(Dictionary<string, object> dict)
        { 
            base.Decode(dict);
            this.text = (dict["text"] as string);
            this.id = dict["id"] as string;
            this.show = (dict.ContainsKey("show") && (bool)dict["show"]);
            this.executionTime = this.show ? LevelEventExecutionTime.OnPrebar : LevelEventExecutionTime.OnBar;
        }

        public override System.Collections.IEnumerator Prepare()
        {
            inte = new Interpreter();
            inte.standardOutput = (string s) => RDVertPlugin.Vert.Log.LogInfo(s);
            inte.implicitOutput = (string s) => RDVertPlugin.Vert.Log.LogDebug(s);
            inte.errorOutput = (string s) => RDVertPlugin.Vert.Log.LogError(s);
            inte.Reset(this.text);
            inte.Compile();
            this.prepared = true;
            yield break;
        }


        public override void Run()
        {
            inte.RunUntilDone(0.01);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
