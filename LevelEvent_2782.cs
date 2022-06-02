using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDLevelEditor
{
    public class LevelEvent_2782 : LevelEvent_Base
    {
        public override void OnCreate()
        {
            base.OnCreate();
            RDVertPlugin.Vert.Log.LogInfo("ayayayayayayaya");
        }

        public override string Encode()
        {
            return base.EncodeBaseProperties(false); 
        }

        public override void Decode(Dictionary<string, object> dict)
        {
            base.Decode(dict);
            // this.executionTime = LevelEventExecutionTime.OnBar;
        }

        public override void Run()
        {
            RDVertPlugin.Vert.Log.LogInfo("RUNNING 2782 NOW");
        }
    }
}
