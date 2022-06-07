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

            // bar
            // return the current bar of the conductor.
            f = Intrinsic.Create("bar");
            f.code = (context, partialResult) =>
            {
                int bar = scrConductor.instance.barNumber;
                return new Intrinsic.Result(bar);
            };

            // beat
            // return the current beat of the conductor.
            f = Intrinsic.Create("beat");
            f.code = (context, partialResult) =>
            {
                float beat = scrConductor.instance.currentBeat;
                return new Intrinsic.Result(beat);
            };

            // deltaTime
            // same as RDC deltaTime
            f = Intrinsic.Create("deltaTime");
            f.code = (context, partialResult) =>
            {
                return new Intrinsic.Result(Time.deltaTime);
            };

            // unscaledDeltaTime
            // same as RDC unscaledDeltaTime
            f = Intrinsic.Create("unscaledDeltaTime");
            f.code = (context, partialResult) =>
            {
                return new Intrinsic.Result(Time.unscaledDeltaTime);
            };

            // audioPos
            // directly AudioSettings.dspTime. This is ultimately the "source of truth" for all RD events...
            // there are a few variants which incorporate calibration.
            f = Intrinsic.Create("audioPos");
            f.code = (context, partialResult) =>
            {
                return new Intrinsic.Result(scrConductor.instance.audioPos);
            };

            // inputPos
            // dspTime that incorporates the calibration setting.
            f = Intrinsic.Create("inputPos");
            f.code = (context, partialResult) =>
            {
                return new Intrinsic.Result(scrConductor.instance.inputPos_p1);
            };

            // visualPos
            // dspTime that incorporates the visual calibration setting.
            f = Intrinsic.Create("visualPos");
            f.code = (context, partialResult) =>
            {
                return new Intrinsic.Result(scrConductor.instance.visualPos);
            };

            /***
             * 
             * INPUT
             * 
             * 
             *****/

            // button
            // return True if a button is pressed down.
            // NOTE: this returns True _every_ frame it's pressed.
            f = Intrinsic.Create("button");
            f.AddParam("key");
            f.code = (context, partialResult) =>
            {
                string key = context.GetVar("key").ToString();
                if (Input.GetKey(key))
                {
                    return Intrinsic.Result.True;
                }
                return Intrinsic.Result.False;
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
