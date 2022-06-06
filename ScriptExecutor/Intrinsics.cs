using Miniscript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RDVertPlugin
{
    public static class Intrinsics
    {
        static bool initialized;

        public static void initializeIfNeeded()
        {
            if (initialized)
            {
                return;
            }
            initialized = true;
            Intrinsic f;

            /***
             * 
             * 
             * TIMING
             * 
             * 
             *****/
            // bpm
            // Returns the bpm of the conductor.
            // NOTE: MiniScript has a fun property, where a bare function is treated as a call to the function with no arguments.
            // and if you want to get the function itself, you use the "@" operator.
            // in other words, I can safely make these functions and people can treat it as a variable, with the exception that
            // you cannot do something like "bpm = {this}", you would have to do "setBPM(...)" instead.
            // I think this is fine, in RDCode bpm is a readonly variable already, so people should be used to it.
            f = Intrinsic.Create("bpm");
            f.code = (context, partialResult) =>
            {
                float bpm = scrConductor.instance.bpm;
                return new Intrinsic.Result(bpm);
            };


            /***
             * 
             * EVENTS
             * 
             * 
             ****/

            // statusSign
            // MiniScript interface for "Show Status Sign" event.
            // text (string, default ""): string to show on the sign.
            // duration (float, default 4.0f): duration to show the sign for.
            // color (string, default null): a hex string. the colour of the thingo.
            // narrate (bool, default 0): whether to narrate this sign or not.
            f = Intrinsic.Create("statusSign");
            f.AddParam("text", "");
            f.AddParam("duration", 4.0f);
            f.AddParam("color", null as Value);
            f.AddParam("narrate", 0);

            f.code = (context, partialResult) =>
            {
                string text = context.GetLocalString("text");
                string colorString = context.GetLocalString("color");
                Color? color = colorString == null ? null : colorString.TrimStart('#').HexToColor();
                float duration = context.GetLocalFloat("duration");
                bool narrate = context.GetLocalBool("narrate");
                if (scnGame.instance.hud)
                {
                    scnGame.instance.hud.SetStatusText(text, color, duration, narrate);
                }
                return Intrinsic.Result.Null;
            };

        }
    }
}
