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
    public class ExecutorManager : Singleton<ExecutorManager>
    {
        public Dictionary<string, Interpreter> interpreterMap;
        public Dictionary<string, LevelEvent_2782> levelEventMap;

        public Dictionary<string, ValFunction> sharedMap;

        public List<string> activeScriptKeys;
        public string runningScript;

        float TIME_ALLOWANCE = 0.008f; // 8ms of each 16ms frame is the maximum allotted time for MiniScript. 

        public override void Awake()
        {
            this.interpreterMap = new Dictionary<string, Interpreter>();
            this.activeScriptKeys = new List<string>();
            this.levelEventMap = new Dictionary<string, LevelEvent_2782>();
            this.sharedMap = new Dictionary<string, ValFunction>();
        }

        public Interpreter GetInterpreter(string key)
        {
            if (!this.interpreterMap.ContainsKey(key))
            {
                Interpreter interpreter = new Interpreter();
                interpreter.standardOutput = (string s) => RDVertPlugin.Vert.Log.LogInfo(s);
                interpreter.implicitOutput = (string s) => RDVertPlugin.Vert.Log.LogDebug(s);
                interpreter.errorOutput = (string s) =>
                {
                    if (scnGame.instance != null)
                    {
                        scnGame.instance.hud.SetStatusText(s, Color.red, 4f);
                    }
                    RDVertPlugin.Vert.Log.LogError(s);
                };
                this.interpreterMap[key] = interpreter;
            }
            return this.interpreterMap[key];
        }

        public void Reset()
        {
            this.sharedMap.Clear();
            this.interpreterMap.Clear();
            this.levelEventMap.Clear();
            this.activeScriptKeys.RemoveAll((s) => true);
        }

        public void ActivateScript(string key)
        {
            this.activeScriptKeys.Add(key);
        }

        public void DeactivateScript(string key)
        {
            this.activeScriptKeys.Remove(key);
        }

        public LevelEvent_2782 activeEvent()
        {
            if (this.runningScript == null)
            {
                return null;
            }
            return this.levelEventMap[this.runningScript];
        }

        // Not called Update() on purpose. This gets injected into scrConductor's update, ExecutorManager doesn't have an Update
        // why? because I want more control over when scripts gets run in RD's event lifecycle. 
        public void DoUpdate()
        {
            float timeLimit = Time.maximumDeltaTime + TIME_ALLOWANCE;
            // iterate backwards, so that when we remove keys we don't skip by accident.
            // scheduling: we have a total allowed time. each script gets allotted that time / number of scripts left.
            // so if a script exits earlier, this gives the next scripts more time to run.
            // todo: add a RD surgery setting to allow scripts to bypass the scheduler
            for (int i = activeScriptKeys.Count - 1; i >= 0; i--)
            {
                string key = activeScriptKeys[i];
                Interpreter interpreter = this.interpreterMap[key];
                float timeLimitForThisScript = (timeLimit - Time.maximumDeltaTime) / activeScriptKeys.Count;
                if (timeLimitForThisScript < 0)
                {
                    timeLimitForThisScript = 0.001f; // 1 ms
                    RDVertPlugin.Vert.Log.LogWarning(String.Format("We ran out of time, frame drops may occur... {0}", key));
                }
                this.runningScript = key;
                interpreter.RunUntilDone(timeLimitForThisScript);
                if (interpreter.done)
                {
                    RDVertPlugin.Vert.Log.LogInfo(String.Format("Script finished: {0}", key));
                    this.activeScriptKeys.RemoveAt(i);
                }
            }
        }

        public void PrepareScriptForExecution(string key, string content)
        {
            Interpreter interpreter = GetInterpreter(key);
            interpreter.Reset(content);
            interpreter.Compile();
        }

        public void RunScript(string key, double timeLimit)
        {
            Interpreter interpreter = GetInterpreter(key);
            interpreter.RunUntilDone(timeLimit);
        }
    }
}
