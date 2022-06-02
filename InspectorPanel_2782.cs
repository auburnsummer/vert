using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDLevelEditor
{
    public class InspectorPanel_2782 : InspectorPanel
    {
        public new void Awake()
        {
            base.Awake();
            RDVertPlugin.Vert.Log.LogInfo("whoa okay now we're in here");
        }
    }
}
