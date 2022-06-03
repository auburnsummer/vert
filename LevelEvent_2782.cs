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

        public bool show;

        public override void OnCreate()
        {
            base.OnCreate();
            RDVertPlugin.Vert.Log.LogInfo("ayayayayayayaya");
        }

        public override string Encode()
        {
            return base.EncodeBaseProperties(false) + RDEditorUtils.EncodeBool("show", this.show, false) + RDEditorUtils.EncodeUnicodeString("text", this.text, true);
        }


        public override void Decode(Dictionary<string, object> dict)
        {
            base.Decode(dict);
            this.text = (dict["text"] as string);
            this.show = (dict.ContainsKey("show") && (bool)dict["show"]);
            this.executionTime = this.show ? LevelEventExecutionTime.OnPrebar : LevelEventExecutionTime.OnBar;

        }


        public override void Run()
        {
            RDVertPlugin.Vert.Log.LogInfo("RUNNING 2782 NOW");
            RDVertPlugin.Vert.Log.LogInfo(this.text);

        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
