#define NotUgly
#define Simulation

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using VRageMath;
using VRage.Game;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Ingame;
using Sandbox.Game.EntityComponents;
using VRage.Game.Components;
using VRage.Collections;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using SETestEnv;
using System.Linq;

namespace Script
{
    partial class Program : TestGridProgram
    {
        public override void RunMain(string argument)
        {
            (this.Runtime as TestGridProgramRuntimeInfo).SetInstructionCount(0);
            Main(argument);
        }


        #region ingame script start

        public const string TimerName = "Sequencer Timer";

        const string LOG_CAT = "gen";

  

        public static MyGridProgram Current;
        ParamsRouter paramsRouter;
        TimerController timerController;       
         
        Scheduler sch;
        RuntimeTask runtime;

        const int majorVersion = 0;
        const int minorVersion = 4;

        public Program()
        {
            Current = this;

            Log.Categories = new Dictionary<string, LogLevel>
            {
                { Scheduler.LOG_CAT,        LogLevel.Verbose },
                { Parser.LOG_CAT,           LogLevel.Verbose },
                { Program.LOG_CAT,          LogLevel.Verbose },
                { RuntimeTask.LOG_CAT,      LogLevel.Verbose },
                { ImplLogger.LOG_CAT,       LogLevel.Warning },
                { TimerController.LOG_CAT,  LogLevel.Warning },
            };

            Log.NewFrame();

            Log.Write("To recompile sequencer script run the Programable Block with argument \"reset\"");


            paramsRouter = new ParamsRouter();

            paramsRouter.UnknownCase = UnknownCommand;
            paramsRouter.Cases = new Dictionary<string, Action<string>>
            {
                { "start", RunProgram },
                { "stop", StopProgram },
                { "exec", ExecuteLine },
                { "reset", ResetState },
            };

            Initialize();

            sch.Run();

            Current = null;
        }

        private void ExecuteLine(string arg)
        {
            var parser = new Parser();
            if (parser.Parse(arg))
            {
                var prog = parser.Programs.First();
                if (runtime != null)
                {
                    string tempName = "_run_" + runtime.GenerateProgramId().ToString();

                    prog.Commands.Add(new SqCommand { Cmd = "unload", Args = new object[] { tempName }, Impl = ExecFlowCommandImpl.Unload }); // todo:
                    prog.Name = tempName;
                    runtime.RegisterPrograms(new SqProgram[] { prog }); 
                    runtime.StartProgram(tempName);
                }
            }
        }

        private void RunProgram(string arg)
        {
            // todo: is initialized check
            if (runtime != null)
            {
                runtime.StartProgram(arg.Trim());
                if (!runtime.IsEnqueued)
                {
                    sch.EnqueueTask(runtime);
                }
            }
            else
            {
                Log.Write("Script is not initialized");
            }
        }

        private void StopProgram(string arg)
        {
            // todo: is initialized check
            if (runtime != null)
            {
                runtime.StopProgram(arg.Trim());
            }
            else
            {
                Log.Write("Script is not initialized");
            }
        }

        private void ResetState(string arg)
        {
            Storage = "";
            Initialize();
        }

        private void UnknownCommand(string arg)
        {
            Log.WriteFormat(LOG_CAT, LogLevel.Warning, "unknown command recieved \"{0}\"", arg);
        }

        void Initialize()
        {
            bool hasStoredData = false;

            Deserializer decoder = null;

            if (!string.IsNullOrEmpty(Storage))
            {
                decoder = new Deserializer(Storage);
                int major = 0;
                int minor = 0;

                try
                {
                    major = decoder.ReadInt();
                    minor = decoder.ReadInt();
                }
                catch
                {
                    major = 0;
                    minor = 0;
                }

                if (major == majorVersion &&
                    minor == minorVersion)
                {
                    hasStoredData = true;
                }
                else
                {
                    Log.WriteFormat(LOG_CAT, LogLevel.Warning, "stored data incompitable format found ({0}.{1}), skipping state restore", major, minor);
                }
            }

            sch = new Scheduler();
            runtime = null;

            if (hasStoredData)
            {
                try
                {
                    timerController = new TimerController(decoder);
                    runtime = new RuntimeTask(decoder, timerController);
                    VariablesStorage.Deserialize(decoder);
                    // todo: load loglevels
                }
                catch (Exception e)
                {
                    hasStoredData = false;
                    Log.WriteFormat(LOG_CAT, LogLevel.Error, "state restoring failed: {0}", e.ToString());
                }
            }

            if (!hasStoredData)
            {
                timerController = new TimerController();
                var parse = new ParserTask(Current.Me.CustomData);
                parse.Done = r =>
                {
                    if (r.Item1 != null)
                    {
                        runtime = new RuntimeTask(r.Item1, timerController);
                    }
                };
                sch.EnqueueTask(parse);
            }

        }

        public void Save()
        {
            Current = this;
            Log.NewFrame();

            var encoder = new Serializer()
                .Write(majorVersion)
                .Write(minorVersion);

            timerController.Serialize(encoder);
            runtime.Serialize(encoder);
            VariablesStorage.Shared.Serialize(encoder);
            // todo: save loglevels


            Storage = encoder.ToString();

            Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "instructions: {0,5}/{1}", Program.Current.Runtime.CurrentInstructionCount,
                Program.Current.Runtime.MaxInstructionCount);

            Current = null;
        }

        public void Main(string argument)
        {
            Current = this;

            Log.NewFrame();

            timerController.Update();

            paramsRouter.Route(argument);

            if ((runtime?.HaveWork() ?? false) && timerController.Timeout() && !runtime.IsEnqueued)
            {
                sch.EnqueueTask(runtime);
            }

            sch.Run();

            timerController.ContinueWait(sch.HasTasks());

            Current = null;
        }

        #endregion // ingame script end
    }

}




