using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    partial class Program
    {

        /* #override
         * InsertFileName : false
         * TrimComments : false
         */

        #region ingame script start

        private void ShowStatus(string obj)
        {
            Log.Write("Status:");
            Log.Write("Scheduller tasks:");
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
            foreach (var programName in runtime.StartedPrograms())
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


                    prog.Commands.Add(new SqCommand("unload", new object[] { tempName }, ExecFlowCommandImpl.Unload)); // todo:
                    prog.Name = tempName;
                    runtime.RegisterPrograms(new SqProgram[] { prog });
                    runtime.StartProgram(tempName);
                }
            };
            sch.EnqueueTask(parse);
        }

        private void StartProgram(string arg)
        {
            runtime.StartProgram(arg.Trim());
        }

        private void StopProgram(string arg)
        {
            runtime.StopProgram(arg.Trim());
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

        #endregion // ingame script end
    }
}
