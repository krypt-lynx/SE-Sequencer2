using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace Script
{

    #region ingame script start


    class ExecFlowCommandImpl
    {
        public static CommandResult Wait(IList args)
        {
            ImplLogger.LogImpl("wait", args);
            return new CommandResult { Action = CommandAction.Wait, Data = (float)(double)args[0]};
        }

        public static CommandResult WaitTicks(IList args)
        {
            ImplLogger.LogImpl("waitticks", args);
            return new CommandResult { Action = CommandAction.Wait, Data = (float)((double)args[0] * 60) };
        }

        public static CommandResult Repeat(IList args)
        {
            ImplLogger.LogImpl("repeat", args);
            return new CommandResult { Action = CommandAction.Repeat };
        }

        public static CommandResult Start(IList args)
        {
            ImplLogger.LogImpl("start", args);
            return new CommandResult { Action = CommandAction.Start, Data = (string)args[0] };
        }

        public static CommandResult Stop(IList args)
        {
            ImplLogger.LogImpl("stop", args);
            return new CommandResult { Action = CommandAction.Stop, Data = (string)args[0] };
        }

        public static CommandResult WaitUntil(IList args)
        {
            ImplLogger.LogImpl("waituntil", args);
            return null;
        }

        internal static CommandResult SetVar(IList args)
        {
            ImplLogger.LogImpl("setvar", args);
            VariablesStorage.Shared.SetVariable((string)args[0], (double)args[1]);
            return null;
        }

        internal static CommandResult Switch(IList args)
        {
            ImplLogger.LogImpl("switch", args);
            int var = (int)(VariablesStorage.Shared.GetVariable((string)args[0]) ?? 0);
            IList cases = (IList)args[1];

            if (var < 0 || var >= cases.Count)
            {
                Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Warning, "Value {0} is out of bounds (0..{1})", var, cases.Count);
                return null;
            }
            else
            {
                return new CommandResult { Action = CommandAction.Start, Data = (string)cases[var] };
            }
        }

        internal static CommandResult Load(IList args)
        {
            Parser parser = new Parser();
            if (parser.Parse((string)args[0]))
            {

                return new CommandResult { Action = CommandAction.AddMethods, Data = parser.Programs.Where(x => x.Name != "") };
            }
            else
            {
                Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Warning, "exception during parsing: {0}", parser.ErrorMessage);
                return null;
            }

        }

        internal static CommandResult Unload(IList args)
        {
            return new CommandResult { Action = CommandAction.RemoveMethods, Data = new string[] { (string)args[0] } };
        }
    }

    #endregion // ingame script end
}
