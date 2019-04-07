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

        internal static CommandRef[] Defs()
        {
            return new CommandRef[] {
                new CommandRef("wait", new ParamRef[] {
                    new ParamRef (ParamType.Double), // delay
                }, Wait, requirements: SqRequirements.Timer, isWait: true),
                new CommandRef("waitticks", new ParamRef[] {
                    new ParamRef (ParamType.Double), // delay
                }, WaitTicks, requirements: SqRequirements.Timer, isWait: true),
                new CommandRef("repeat", new ParamRef[] {
                }, Repeat, requirements: SqRequirements.Wait),
                new CommandRef("start", new ParamRef[] {
                    new ParamRef (ParamType.String, true, ""), // func
                }, Start),
                new CommandRef("stop", new ParamRef[] {
                    new ParamRef (ParamType.String, true, ""), // func
                }, Stop),
                new CommandRef("load", new ParamRef[] {
                    new ParamRef (ParamType.String), // code
                }, Load) ,
                new CommandRef("unload", new ParamRef[] {
                    new ParamRef (ParamType.String, true, ""), // func
                }, Unload),
                new CommandRef("setvar", new ParamRef[] {
                    new ParamRef (ParamType.String), // var name
                    new ParamRef (ParamType.Double), // value
                }, SetVar),
                 new CommandRef("switch", new ParamRef[] {
                    new ParamRef (ParamType.String), // case var name
                    new ParamRef (ParamType.String, aggregative: true), // cases
                }, Switch),
            };
        }


        public static void Wait(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("wait", args);
            context.Wait((double)args[0]);
        }

        public static void WaitTicks(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("waitticks", args);
            context.Wait(((double)args[0])/ 60);
        }

        public static void Repeat(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("repeat", args);
            context.Goto(0);
        }

        public static void Start(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("start", args);
            context.Runtime.StartProgram((string)args[0]);            
        }

        public static void Stop(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("stop", args);
            context.Runtime.StopProgram((string)args[0]);
        }

        public static void SetVar(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("setvar", args);
            context.Set((string)args[0], (double)args[1]);
        }

        public static void Switch(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("switch", args);
            int var = (int)(context.Get((string)args[0]));
            IList cases = (IList)args[1];

            if (var < 0 || var >= cases.Count)
            {
                Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Warning, "Value {0} is out of bounds (0..{1})", var, cases.Count);
            }
            else
            {
                context.Runtime.StartProgram((string)cases[var]);
            }
        }

        internal static void Load(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("load", args);
            Parser parser = new Parser((string)args[0]);
            parser.Parse(() => false);

            if (parser.Finalize())
            {
                context.Runtime.RegisterPrograms(parser.Programs);
            }
            else
            {
                Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Warning, "exception during parsing: {0}", parser.ErrorMessage);
            }
        }

        internal static void Unload(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("unload", args);
            context.Runtime.UnloadProgram((string)args[0]);        
        }
    }

    #endregion // ingame script end
}
