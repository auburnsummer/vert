using Miniscript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDVertPlugin
{
    public class ExecutorManager : Singleton<ExecutorManager>
    {
        public Dictionary<string, Interpreter> interpreterMap;
        public List<string> activeScriptKeys;

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
                this.interpreterMap[key] = interpreter;
            }
            return this.interpreterMap[key];
        }

        public void PrepareScriptForExecution(string key, string content)
        {
            Interpreter interpreter = GetInterpreter(key);
            interpreter.Reset(content);
            interpreter.Compile();
        }
    }
}
