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
            (this.Runtime as TestGridProgramRuntimeInfo).InitNewRun();
            Main(argument, UpdateType.Terminal);
        }


        /* #override
         * InsertFileName : false
         * TrimComments : false
         */

        #region ingame script start
            
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





        const string LOG_CAT = "gen";

        ParamsRouter paramsRouter;
        TimerController timerController;

        Scheduler sch;
        RuntimeTask runtime;

        const uint major = 2;
        const uint minor = 2;
        const uint patch = 0;

        UpdateType paramertizedTypes = (UpdateType)0x1F; // .Terminal | .Trigger | .Antenna | .Mod | .Script;

        public Program()
        {
            IsolatedRun(() =>
            {
                LogLevels();

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
                        { "ping", WatchDog.Reply },
                    };

                Initialize();

                sch.Run();
            });
        }

        void Initialize()
        {
            Deserializer decoder = TryInitStorageDecoder();
            bool hasStoredData = decoder != null;

            sch = new Scheduler();
            runtime = null;

            if (decoder != null)
            {
                try
                {
                    Log.Deserialize(decoder);
                    timerController = new TimerController(decoder);
                    runtime = new RuntimeTask(decoder, timerController);
                    VariablesStorage.Deserialize(decoder);

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

        Deserializer TryInitStorageDecoder()
        {             
            Deserializer decoder = null;

            if (!string.IsNullOrEmpty(Storage))
            {
                decoder = new Deserializer(Storage);
                uint mj = 0, mn = 0, pt = 0, ver = 0;
                try
                {
                    mj = (uint)decoder.ReadInt();
                    mn = (uint)decoder.ReadInt();
                    pt = (uint)decoder.ReadInt();
                    ver = Version(mj, mn, pt);
                }
                catch
                {
                    mj = 0;
                    mn = 0;
                    pt = 0;
                    ver = 0;
                }

                if (ver != Version(major, minor, patch))
                {
                    decoder.Dispose();
                    decoder = null;

                    Log.WriteFormat(LOG_CAT, LogLevel.Warning, "stored data incompitable format found ({0}.{1}.{2}), skipping state restore", mj, mn, pt);
                }
            }

            return decoder;
        }

        uint Version(uint major, uint minor, uint patch)
        {
            return (major << 20) + (minor << 10) + patch;
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
                        if (runtime.StartProgram("_load", true))
                        {
                            timerController.ScheduleStart(0);
                        }
                    }
                }
            };
            sch.EnqueueTask(parse);
        }

        public void Save()
        {
            IsolatedRun(() =>
            {
                var encoder = new Serializer()
                    .Write((int)major)
                    .Write((int)minor)
                    .Write((int)patch);

                Log.Serialize(encoder);
                timerController.Serialize(encoder);
                runtime.Serialize(encoder);
                VariablesStorage.Shared.Serialize(encoder);

                Storage = encoder.ToString();

                Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "instructions: {0,5}/{1}", Runtime.CurrentInstructionCount,
                    Runtime.MaxInstructionCount);
            });
        }

        public void Main(string argument, UpdateType updateSource)
        {
            IsolatedRun(() =>
            {
                timerController.Update();

                if ((updateSource | paramertizedTypes) != 0)
                {
                    paramsRouter.Route(argument);
                }

                if ((runtime?.HaveWork() ?? false) && timerController.Timeout() && !runtime.IsEnqueued)
                {
                    sch.EnqueueTask(runtime);
                }

                if (sch.HasTasks())
                {
                    sch.Run();
                }
            });
        }

        #endregion // ingame script end
    }

}

