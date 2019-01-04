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


        /* #override
         * InsertFileName : false
         * TrimComments : false
         */

        #region ingame script start

        // Name of the Timer, used by script.
        public const string TimerName = "Sequencer Timer";

        // Logging levels for all used categories. Those values is used if was not overrided using /loglevel command
        private static void LogLevels()
        {
            Log.LogLevels = new Dictionary<string, LogLevel>
            {
                { Scheduler.LOG_CAT,        LogLevel.Warning },
                { Parser.LOG_CAT,           LogLevel.Warning },
                { Program.LOG_CAT,          LogLevel.Warning },
                { RuntimeTask.LOG_CAT,      LogLevel.Warning },
                { ImplLogger.LOG_CAT,       LogLevel.Warning },
                { TimerController.LOG_CAT,  LogLevel.Warning },
            };
        }







        //---------------------------------------------
        // Do not report issues if code below is modified
  





        public static MyGridProgram Current;
        ParamsRouter paramsRouter;
        TimerController timerController;       
         
        Scheduler sch;
        RuntimeTask runtime;

        const uint major = 2;
        const uint minor = 1;
        const uint patch = 0;

        Exception lastException = null;
        int lastLine = -1;
        public Program()
        {
            var test = Enum.GetValues(typeof(MatchingType)).Cast<MatchingType>();


            if (lastException == null)
            {
                try
                {
                    Current = this;

                    LogLevels();

                    Log.NewFrame();

                    Log.Write("To recompile sequencer script run the Programable Block with argument \"parse\"");
                    Log.Write("To reset sequencer script run the Programable Block with argument \"reset\"");

                    paramsRouter = new ParamsRouter();

                    paramsRouter.UnknownCase = UnknownCommand;
                    paramsRouter.Cases = new Dictionary<string, Action<string>>
                    { 
                        { "start", StartProgram },
                        { "stop", StopProgram },
                        { "exec", ExecuteLine },
                        { "parse", ReloadScript },
                        { "reset", ResetState },
                        { "status", ShowStatus },
                        { "ping", Pong },
                    };

                    Initialize();

                    sch.Run();

                    Current = null;
                }
                catch (Exception e)
                {
                    lastException = e;
                }
            }

            if (lastException != null)
            {
                EchoException();
                return;
            }
        }

        private void Pong(string arg)
        {
            long id;

            if (long.TryParse(arg, out id))
            {
                var block = GridTerminalSystem.GetBlockWithId(id);
                if (block != null)
                {
                    block.CustomData = "pong";
                }
            }
        }

        private void EchoException()
        {
            Echo("Exception handled!");
            Echo("Please, make screenshot of this message and report the issue to developer");
            Echo("To recover recompile the script");
            Echo(lastException.Message);
            Echo(lastException.StackTrace);
            Echo(lastException.Message);
            Echo("Last Line: " + lastLine.ToString());
        }

        private void ShowStatus(string obj)
        {
            Log.Write("Status:");
            Log.Write("Sheduller tasks:");
            foreach (var task in sch.AllTasks())
            {
                Log.Write(task.DisplayName());
            }
            Log.Write("Stored methods:");
            foreach (var programName in runtime.StoredPrograms())
            {
                Log.WriteFormat("\"{0}\"", programName);
            }
            Log.Write("Started methods:");
            foreach (var programName in runtime.Startedrograms())
            {
                Log.WriteFormat("\"{0}\"", programName);
            }
        }

        private void ReloadScript(string arg)
        {
            ScheduleParse(false);
            Log.Write("Parsing scheduled"); // force log cleanup
        }

        private void ExecuteLine(string arg)
        {
            var parse = new ParserTask(arg);
            parse.Done = r =>
            {
                SqProgram prog = r.Item1?.FirstOrDefault();

                if (r.Item1 != null)
                {
                    string tempName = "_run_" + runtime.GenerateProgramId().ToString();

                    prog.Commands.Add(new SqCommand { Cmd = "unload", Args = new object[] { tempName }, Impl = ExecFlowCommandImpl.Unload }); // todo:
                    prog.Name = tempName;
                    runtime.RegisterPrograms(new SqProgram[] { prog });
                    StartProgram(tempName);
                }

            };
            sch.EnqueueTask(parse);
        }

        private void StartProgram(string arg)
        {
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
            Log.Write("Script was reseted");
        }

        private void UnknownCommand(string arg)
        {
            if (arg.FirstOrDefault() == '/')
            {
                ExecuteLine(arg);
            }
            else
            {
                Log.WriteFormat(LOG_CAT, LogLevel.Warning, "unknown command recieved \"{0}\"", arg);
            }
        }

        void Initialize()
        {
            bool hasStoredData = false;

            Deserializer decoder = null;

            if (!string.IsNullOrEmpty(Storage))
            {
                decoder = new Deserializer(Storage);
                uint mj = 0;
                uint mn = 0;
                uint pt = 0;
                uint ver = 0;
                try
                {
                    mj = (uint)decoder.ReadInt();
                    mn = (uint)decoder.ReadInt();
                    pt = (uint)decoder.ReadInt();
                    ver = (mj << 20) + (mn << 10) + pt;
                }
                catch
                {
                    mj = 0;
                    mn = 0;
                    pt = 0;
                    ver = 0;
                }

                if (ver == (major << 20) + (minor << 10) + patch)
                {
                    hasStoredData = true;
                }
                else
                {
                    Log.WriteFormat(LOG_CAT, LogLevel.Warning, "stored data incompitable format found ({0}.{1}.{2}), skipping state restore", mj, mn, pt);
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
                    Log.Deserialize(decoder);

                    if (!runtime.StoredPrograms().Any())
                    {
                        ScheduleParse(true);
                    }
                }
                catch (Exception e)
                {
                    hasStoredData = false;
                    Log.WriteFormat(LOG_CAT, LogLevel.Error, "state restoring failed: {0}", e.ToString());
                }
            }

            if (decoder != null)
            {
                decoder.Dispose();
            }

            if (!hasStoredData)
            {
                LogLevels();
                timerController = new TimerController();
                runtime = new RuntimeTask(timerController);
                VariablesStorage.Clear();

                ScheduleParse(true);
            }

        }

        private void ScheduleParse(bool runLoad)
        {
            var parse = new ParserTask(Current.Me.CustomData);
            parse.Done = r =>
            {
                if (r.Item1 != null)
                {
                    Log.Write(LOG_CAT, LogLevel.Verbose, "Parsing done");
                    runtime.RegisterPrograms(r.Item1);
                    if (runLoad)
                    {
                        runtime.StartProgram("_load", true);
                    }
                }
            };
            sch.EnqueueTask(parse);
        }

        public void Save()
        {
            if (lastException == null)
            {
                try
                {
                    lastLine = 0;
                    Current = this;
                    lastLine = 1;
                    Log.NewFrame();
                    lastLine = 2;

                    var encoder = new Serializer()
                        .Write((int)major)
                        .Write((int)minor)
                        .Write((int)patch);
                    lastLine = 3;

                    timerController.Serialize(encoder);
                    lastLine = 4;
                    runtime.Serialize(encoder);
                    lastLine = 5;
                    VariablesStorage.Shared.Serialize(encoder);
                    lastLine = 6;
                    Log.Serialize(encoder);
                    lastLine = 7;


                    Storage = encoder.ToString();
                    lastLine = 8;

                    Echo((Program.Current == null).ToString());
                    lastLine = 9;

                    Echo((Program.Current.Runtime == null).ToString());
                    lastLine = 10;

                    Echo((Program.Current.Runtime.CurrentInstructionCount == 0).ToString());
                    lastLine = 11;

                    Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "instructions: {0,5}/{1}", Program.Current.Runtime.CurrentInstructionCount,
                        Program.Current.Runtime.MaxInstructionCount);
                    lastLine = 12;

                    Current = null;
                }
                catch (Exception e)
                {
                    lastException = e;
                }
            }

            if (lastException != null)
            {
                EchoException();
                return;
            }
        }

        const string LOG_CAT = "gen";

        public void Main(string argument)
        {
            if (lastException == null)
            {
                try
                {
                    Current = this;

                    Log.NewFrame();

                    timerController.Update();

                    paramsRouter.Route(argument);

                    if ((runtime?.HaveWork() ?? false) && timerController.Timeout() && !runtime.IsEnqueued)
                    {
                        sch.EnqueueTask(runtime);
                    }

                    if (sch.HasTasks())
                    {
                        sch.Run();
                    }

                    timerController.ContinueWait(sch.HasTasks());

                    Current = null;
                }
                catch (Exception e)
                {
                    lastException = e;
                }
            }

            if (lastException != null)
            {
                EchoException();
                return;
            }
        }

        #endregion // ingame script end
    }

}




