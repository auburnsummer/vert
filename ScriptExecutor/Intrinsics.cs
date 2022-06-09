using Miniscript;
using RDLevelEditor;
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
                int bar = scrConductor.instance.barNumberUpdateOnBar;
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

            // eventBar
            // return the bar of this Run Script event.
            // the difference between "bar" and "eventBar" is that eventBar is where the event is, bar is where the playhead is.
            f = Intrinsic.Create("eventBar");
            f.code = (context, partialResult) =>
            {
                LevelEvent_2782 evt = Singleton<ExecutorManager>.Instance.activeEvent();
                return new Intrinsic.Result(evt.bar);
            };

            // eventBeat
            // return the beat of this Run Script event.
            f = Intrinsic.Create("eventBeat");
            f.code = (context, partialResult) =>
            {
                LevelEvent_2782 evt = Singleton<ExecutorManager>.Instance.activeEvent();
                return new Intrinsic.Result(evt.beat);
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

            // waitUntil
            // pause execution until the specified bar + beat has passed.
            // bar(int, default=1): the bar
            // beat(float, default=0): the beat
            f = Intrinsic.Create("waitUntil");
            f.AddParam("bar", 1f);
            f.AddParam("beat", 0f);  // the first beat in a bar is beat 0
            f.code = (context, partialResult) =>
            {
                int bar = scrConductor.instance.barNumberUpdateOnBar;
                float beat = scrConductor.instance.currentBeat;
                int targetBar = context.GetLocalInt("bar");
                float targetBeat = context.GetLocalFloat("beat");
                if (bar != targetBar || beat < targetBeat)
                {
                    return Intrinsic.Result.Waiting;
                }
                return Intrinsic.Result.Null;
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
             * ACTIONS
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

            /***
             * 
             * DECORATIONS
             * 
             * 
             ****/

            // RD internally calls these 'sprites', but the Miniscript interface is still called decos.

            // decoIds
            // returns a list of the current deco idss.
            f = Intrinsic.Create("decoIds");
            f.code = (context, partialResult) =>
            {
                if (scnGame.instance.currentLevel != null && scnGame.instance.currentLevel is Level_Custom)
                {
                    Level_Custom level = (Level_Custom)scnGame.instance.currentLevel;
                    var sprites = level.sprites;

                    var list = new ValList(sprites.Keys.Select(s => new ValString(s) as Value).ToList());
                    return new Intrinsic.Result(list);
                }
                return Intrinsic.Result.Null;
            };

            // getDeco
            // returns a dictionary of info about a deco.
            f = Intrinsic.Create("getDeco");
            f.AddParam("id", "");
            f.code = (context, partialResult) =>
            {
                var decoMap = new ValMap();
                Level_Custom level = (Level_Custom)scnGame.instance.currentLevel;
                var sprites = level.sprites;
                string id = context.GetLocalString("id");
                if (!sprites.ContainsKey(id))
                {
                    throw new RuntimeException(String.Format("Deco ID {0} not found", id));
                }
                var sprite = sprites[context.GetLocalString("id")];
                Intrinsic f2;
                // x
                // return the x position of the deco.
                f2 = Intrinsic.Create(""); // anonymous.
                f2.code = (context, partialResult) =>
                {
                    return new Intrinsic.Result(sprite.transform.localPosition.x);
                };
                decoMap["x"] = f2.GetFunc();
                // y
                // return the y position of the deco.
                f2 = Intrinsic.Create("");
                f2.code = (context, partialResult) =>
                {
                    return new Intrinsic.Result(sprite.transform.localPosition.y);
                };
                decoMap["y"] = f2.GetFunc();

                // setting x and y just kinda immediately yeets the sprite to the location.
                decoMap.assignOverride = (key, value) =>
                {
                    if (key.ToString() == "x")
                    {
                        // i heard you liked casting
                        sprite.transform.LocalMoveX((float)((ValNumber)value).value);
                    }
                    if (key.ToString() == "y")
                    {
                        sprite.transform.LocalMoveY((float)((ValNumber)value).value);

                    }
                    return true;
                };
                return new Intrinsic.Result(decoMap);
            };
        }
    }
}
