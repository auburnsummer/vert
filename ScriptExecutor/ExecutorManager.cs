using Miniscript;
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
        public List<string> activeScriptKeys;

        float TIME_ALLOWANCE = 0.008f; // 8ms of each 16ms frame is the maximum allotted time for MiniScript. 

        public override void Awake()
        {
            this.interpreterMap = new Dictionary<string, Interpreter>();
            this.activeScriptKeys = new List<string>();
        }

        public Interpreter GetInterpreter(string key)
        {
            if (!this.interpreterMap.ContainsKey(key))
            {
                Interpreter interpreter = new Interpreter();
                interpreter.standardOutput = (string s) => RDVertPlugin.Vert.Log.LogInfo(s);
                interpreter.implicitOutput = (string s) => RDVertPlugin.Vert.Log.LogDebug(s);
                interpreter.errorOutput = (string s) => RDVertPlugin.Vert.Log.LogError(s);
                this.interpreterMap[key] = interpreter;
            }
            return this.interpreterMap[key];
        }

        public void ActivateScript(string key)
        {
            this.activeScriptKeys.Add(key);
        }

        // Not called Update() on purpose. This gets injected into scrConductor's update, ExecutorManager doesn't have an Update
        public void DoUpdate()
        {
            float timeLimit = Time.maximumDeltaTime + TIME_ALLOWANCE;
            // iterate backwards, so that when we remove keys we don't skip by accident.
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
