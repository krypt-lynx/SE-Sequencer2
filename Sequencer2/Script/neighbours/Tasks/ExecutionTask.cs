using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    #region ingame script start


    class ExecutionTask : FastTask<Stub>
    {
        public const string LOG_CAT = "exe";

        public Dictionary<string, SqProgram> Programs;

        private List<string> scheduledPrograms = new List<string>();
        private TimerController timerController;

        public void Serialize(Serializer encoder)
        {
            encoder.Write(scheduledPrograms.Count);
            foreach (var prog in scheduledPrograms)
            {
                encoder.Write(prog);
            }

            encoder.Write(Programs.Count);
            foreach (var prog in Programs.Values)
            {
                prog.Serialize(encoder);
            }
        }

        public ExecutionTask(List<SqProgram> programs, TimerController timerController) : base("Runtime")
        {
            Programs = programs.ToDictionary(x => x.Name);
            this.timerController = timerController;
        }

        public ExecutionTask(Deserializer decoder, TimerController timerController) : base("Runtime")
        {
            this.timerController = timerController;
            int count = decoder.ReadInt();
            scheduledPrograms = new List<string>();
            for (int i = 0; i < count; i++)
            {
                scheduledPrograms.Add(decoder.ReadString());
            }

            count = decoder.ReadInt();
            Programs = new Dictionary<string, SqProgram>();
            for (int i = 0; i < count; i++)
            {
                SqProgram prog = new SqProgram(decoder);
                Programs[prog.Name] = prog;
            }
        }

        public override int InstructionsLimit()
        {
            return 15000; // todo
        }

        public override bool DoWork()
        {
            // time passed sinse last run?
            // Program.Current.Runtime.TimeSinceLastRun
            // but if game reloaded?
            // we can relay only on timer
            // but lets forget about in for now


            Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "Time passed: {0}", timerController.TimePassed());
            Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "Have {0} program(s) to run", scheduledPrograms.Count);

            // LinkedList<>
            foreach (var key in new List<string>(scheduledPrograms))
            {
                var program = Programs[key];

                program.TimeToWait = Math.Max(0, program.TimeToWait - timerController.TimePassed()); 
                    // attempting to substract passed time from just added task. Can be a problem in future.
                ExecuteProgram(program);
            }

            ScheduleWaitIfNeeded();

            return true;
        }

        void ExecuteProgram(SqProgram program)
        {
            if (program.TimeToWait > TimerController.IgnoreDelayLessThen)
            {
                return;
            }

            Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "executing \"{0}\"", program.Name);
            bool stop = false;
            while (program.currentCommand < program.Commands.Count && !stop)
            {
                var cmd = program.Commands[program.currentCommand];
                var result = cmd.Run();

                switch (result?.Action ?? CommandAction.None)
                {
                    case CommandAction.Start:
                        StartProgram((string)result.Data);
                        program.currentCommand++;
                        break;
                    case CommandAction.Stop:
                        StopProgram((string)result.Data);
                        program.currentCommand++;
                        break;
                    case CommandAction.None:
                        program.currentCommand++;
                        break;
                    case CommandAction.Wait:
                        float waitseconds = (float)result.Data;
                        if (waitseconds > 0 && program.currentCommand + 1 < program.Commands.Count)
                        {
                            program.TimeToWait = waitseconds;
                            stop = true;                        
                        }

                        program.currentCommand++;
                        break;
                    case CommandAction.Repeat:
                        program.currentCommand = 0;
                        break;
                }
            }
            

            if (program.currentCommand >= program.Commands.Count)
            {
                scheduledPrograms.Remove(program.Name);
                program.currentCommand = 0;
            }
        }


        private void ScheduleWaitIfNeeded()
        {
            if (scheduledPrograms.Count == 0)
            {
                return;
            }

            var waitseconds = scheduledPrograms.Select(x => Programs[x]).Min(x => x.TimeToWait);

            timerController.ScheduleStart( waitseconds );
        }

        public void StartProgram(string arg)
        {
            if (Programs.ContainsKey(arg))
            {
                if (!scheduledPrograms.Contains(arg))
                {
                    scheduledPrograms.Add(arg);
                }
                else
                {
                    Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "attempt to start already started program \"{0}\", ignoring", arg);
                }
            }
            else
            {
                Log.WriteFormat(LOG_CAT, LogLevel.Warning, "attempt to start unknown program \"{0}\", ignoring", arg);
            }
        }


        public void StopProgram(string arg)
        {
            if (Programs.ContainsKey(arg))
            {
                Programs[arg].TimeToWait = 0;
                Programs[arg].currentCommand = 0;
                scheduledPrograms.Remove(arg);
            }
            else
            {
                Log.WriteFormat(LOG_CAT, LogLevel.Warning, "attempt to stop unknown program \"{0}\", ignoring", arg);
            }
        }
    }

    
    #endregion // ingame script end
}
