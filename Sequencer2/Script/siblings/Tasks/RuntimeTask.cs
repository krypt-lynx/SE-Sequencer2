using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    #region ingame script start


    interface IMethodContext
    {
        void Wait(double seconds);
        void Goto(int line);
        void Set(string name, double value);
        double Get(string name);

        RuntimeTask Runtime { get; }
    }

    class RuntimeTask : InterruptibleTask<Stub>, ISerializable
    {
        public const string LOG_CAT = "exe";


        private TimerController timerController;


        //*** Serialization

        public void Serialize(Serializer encoder)
        {
            encoder.Write(lastProgramId);
            encoder.Write(Programs, i => encoder.Write(i.Key).Write(i.Value));
            encoder.Write(scheduledPrograms, i => encoder.Write(i));
            encoder.Write(replacements, (SqProgram p) => encoder.Write(p));
        }
        
        public void Deserialize(Deserializer dec)
        {
            lastProgramId = dec.ReadInt();
            dec.ReadCollection(() => Programs, () => {
                var key = dec.ReadString();
                var value = dec.ReadObject<SqProgram>();
                value.Runtime = this;
                return new KeyValuePair<string, SqProgram>(key, value);
            });
            dec.ReadCollection(() => scheduledPrograms, () => dec.ReadString());
            dec.ReadCollection(() => replacements, (c) => c.Enqueue(dec.ReadObject<SqProgram>()));
        }

        //*** Scheduller Task


        public RuntimeTask(TimerController timerController) : base("Runtime")
        {
            priority = 3;
            this.timerController = timerController;
        }

        IEnumerator<bool> work = null;

        public override bool DoWork()
        {
            if (work == null)
            {
                work = Runloop();
            }

            bool done = false;

            while (!Timeout() && !done)
            {
                done = !work.MoveNext();
            }

            if (done)
            {
                work.Dispose();
                work = null;
            }

            UpdateTimer();

            return done;
        }

        public IEnumerator<bool> Runloop()
        {
            Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "Time passed: {0}", timerController.TimePassed());
            Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "Have {0} program(s) to run", scheduledPrograms.Count);

            foreach (var key in scheduledPrograms)
            {
                var program = Programs[key];
                program.TimeToWait = Math.Max(0, program.TimeToWait - timerController.TimePassed());
                // attempting to substract passed time from just added task. Can be a problem in future.
            }

            foreach (var key in new List<string>(scheduledPrograms))
            {
                var program = Programs[key];

                foreach (var stub in ExecuteProgram(program))
                {
                    yield return false;
                }
            }

            RetryRegisterPrograms();

            //ScheduleWaitIfNeeded();

            yield return true;
        }
        
        //*** Runtime

        int lastProgramId = 0;

        private Dictionary<string, SqProgram> Programs = new Dictionary<string, SqProgram>();

        private List<string> scheduledPrograms = new List<string>();
        private Queue<SqProgram> replacements = new Queue<SqProgram>();

        public int GenerateProgramId()
        {
            return ++lastProgramId;
        }

        private void RetryRegisterPrograms()
        {
            List<SqProgram> temp = new List<SqProgram>(replacements);
            replacements.Clear();
            RegisterPrograms(temp); // todo: fix messages
        }

        public bool HaveWork()
        {
            return scheduledPrograms.Count > 0;
        }

        IEnumerable<bool> ExecuteProgram(SqProgram program)
        {
            if (program.TimeToWait > TimerController.IgnoreDelayLessThen)
            {
                yield break;
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

                /*var result =*/
                cmd.Run(program);
                program.currentCommand++;

                if (program.TimeToWait > TimerController.IgnoreDelayLessThen)
                {
                    goto pause; // achievement unlocked: use goto
                }

                yield return false;
            }

            // "else" for condition of while loop
            scheduledPrograms.Remove(program.Name);

            pause:

            yield return true;
        }

        public void RegisterPrograms(IEnumerable<SqProgram> programs)
        {
            foreach (var prog in programs)
            {
                if (scheduledPrograms.Contains(prog.Name))
                {
                    Log.WriteFormat(LOG_CAT, LogLevel.Error, "Unable to replace program \"{0}\" because it executing. It will updated after termination", prog.Name);
                    replacements.Enqueue(prog);
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
                    prog.Runtime = this;
                }
            }            
        }

        private void UpdateTimer()
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

        public bool StartProgram(string arg, bool silent = false)
        {
            if (Programs.ContainsKey(arg))
            {
                if (!scheduledPrograms.Contains(arg))
                {
                    Programs[arg].currentCommand = 0;
                    scheduledPrograms.Add(arg);
                    UpdateTimer();
                    return true;
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
            return false;
        }

        public void StopProgram(string arg)
        {
            if (Programs.ContainsKey(arg))
            {
                Programs[arg].TimeToWait = 0;
                scheduledPrograms.Remove(arg);

                RetryRegisterPrograms();
                UpdateTimer();
            }
            else
            {
                Log.WriteFormat(LOG_CAT, LogLevel.Warning, "attempt to stop unknown program \"{0}\", ignoring", arg);
            }
        }

        public void UnloadProgram(string name)
        {
            StopProgram(name);
            Programs.Remove(name);            
        }

        public IEnumerable<string> StoredPrograms()
        {
            return Programs.Values.Select(x => x.Name);
        }

        public IEnumerable<string> StartedPrograms()
        {
            return scheduledPrograms;
        }
    }

    
    #endregion // ingame script end
}
