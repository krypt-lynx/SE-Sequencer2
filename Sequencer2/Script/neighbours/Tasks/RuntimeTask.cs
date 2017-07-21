using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    #region ingame script start


    class RuntimeTask : FastTask<Stub>
    {
        public const string LOG_CAT = "exe";

        private Dictionary<string, SqProgram> Programs = new Dictionary<string, SqProgram>();

        private List<string> scheduledPrograms = new List<string>();
        private Queue<SqProgram> replecments = new Queue<SqProgram>();

        private TimerController timerController;

        int lastProgramId = 0;

        public void Serialize(Serializer encoder)
        {
            encoder.Write(lastProgramId);
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

        public int GenerateProgramId()
        {
            return ++lastProgramId;
        }

        public RuntimeTask(TimerController timerController) : base("Runtime")
        {
            this.timerController = timerController;
        }

        public RuntimeTask(Deserializer decoder, TimerController timerController) : base("Runtime")
        {
            this.timerController = timerController;
            lastProgramId = decoder.ReadInt();
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
            return 15000; 
        }

        public override bool DoWork()
        {
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

            RetryRegisterPrograms();

            ScheduleWaitIfNeeded();

            return true;
        }

        private void RetryRegisterPrograms()
        {
            List<SqProgram> temp = new List<SqProgram>(replecments);
            replecments.Clear();
            RegisterPrograms(temp); // todo: fix messages
        }

        public bool HaveWork()
        {
            return scheduledPrograms.Count > 0;
        }

        void ExecuteProgram(SqProgram program)
        {
            if (program.TimeToWait > TimerController.IgnoreDelayLessThen)
            {
                return;
            }

            Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "executing \"{0}\"", program.Name);

            program._cycle ++; // Breaking OOP a bit

            while (program.currentCommand < program.Commands.Count)
            {
                var cmd = program.Commands[program.currentCommand];
                if (cmd._cycle == program._cycle)
                {
                    Log.WriteFormat(LOG_CAT, LogLevel.Warning, "Program \"{0}\" is not sleeping. Forcing break.", program.Name);
                    goto pause;
                }
                else
                {
                    cmd._cycle = program._cycle;
                }

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
                    case CommandAction.RemoveMethods:
                        foreach (var name in (string[])result.Data)
                        {
                            StopProgram(name);
                            Programs.Remove(name);
                        }
                        program.currentCommand++;
                        break;
                    case CommandAction.AddMethods:
                        RegisterPrograms((IEnumerable<SqProgram>)result.Data);
                        program.currentCommand++;
                        break;
                    case CommandAction.None:
                        program.currentCommand++;
                        break;
                    case CommandAction.Wait:
                        float waitseconds = (float)result.Data;
                        program.currentCommand++;
                        if (waitseconds > 0)
                        {
                            program.TimeToWait = waitseconds;
                            goto pause; // achievement unlocked: use goto
                        }

                        break;
                    case CommandAction.Repeat:
                        program.currentCommand = 0;
                        break;
                }
            }

            // "else" for condition of while loop
            scheduledPrograms.Remove(program.Name);

            pause:

            ;
        }

        public void RegisterPrograms(IEnumerable<SqProgram> programs)
        {
            foreach (var prog in programs)
            {
                if (scheduledPrograms.Contains(prog.Name))
                {
                    Log.WriteFormat(LOG_CAT, LogLevel.Error, "Unable to replace program \"{0}\" because it executing. It will updated after termination", prog.Name);
                    replecments.Enqueue(prog);
                }
                else 
                {
                    if (Programs.ContainsKey(prog.Name))
                    {
                        Log.WriteFormat(LOG_CAT, LogLevel.Warning, "Program \"{0}\" was replaced", prog.Name);
                    }
                    else
                    {
                        Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "Program \"{0}\" was added", prog.Name);
                    }
                    Programs[prog.Name] = prog;
                }

            }
        }

        private void ScheduleWaitIfNeeded()
        {
            if (scheduledPrograms.Count == 0)
            {
                timerController.CancelStart();
                return;
            }
            else
            {
                var waitseconds = scheduledPrograms.Select(x => Programs[x]).Min(x => x.TimeToWait);

                timerController.ScheduleStart(waitseconds);
            }
        }

        public void StartProgram(string arg, bool silent = false)
        {
            if (Programs.ContainsKey(arg))
            {
                if (!scheduledPrograms.Contains(arg))
                {
                    Programs[arg].currentCommand = 0;
                    scheduledPrograms.Add(arg);
                }
                else if (!silent)
                {
                    Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "attempt to start already started program \"{0}\", ignoring", arg);
                }
            }
            else
            {
                if (!silent)
                {
                    Log.WriteFormat(LOG_CAT, LogLevel.Warning, "attempt to start unknown program \"{0}\", ignoring", arg);
                }
            }
        }

         public void StopProgram(string arg)
        {
            if (Programs.ContainsKey(arg))
            {
                Programs[arg].TimeToWait = 0;
                scheduledPrograms.Remove(arg);

                RetryRegisterPrograms();
                ScheduleWaitIfNeeded();
            }
            else
            {
                Log.WriteFormat(LOG_CAT, LogLevel.Warning, "attempt to stop unknown program \"{0}\", ignoring", arg);
            }
        }
    }

    
    #endregion // ingame script end
}
