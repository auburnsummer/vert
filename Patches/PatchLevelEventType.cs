using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDVertPlugin
{
    // LevelEventType is an enum...
    // we can't really patch it, it just gets converted into ints at runtime.
    // so since LevelEventType is really just an int...
    // anytime we need to pass one, we can use an arbitrary integer and do this:
    //  (LevelEventType)PatchLevelEventType.RunScript
    // and it will work. :okay:

    // jade (0x0ade) taught me this technique originally. 
    // every time I need to use it, i set the arbitrary int to 0x0ade in her honour.
    public static class PatchLevelEventType
    {
        public static int RunScript = 0x0ade;
    }
}
