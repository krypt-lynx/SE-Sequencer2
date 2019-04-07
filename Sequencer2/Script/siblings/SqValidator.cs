using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    /* #override
     * TrimComments : false
     */

    #region ingame script start

    public enum SqCapablities
    {
        None = 0,
        Timer = 1,
        ControlModule = 2,
    }


    public enum SqRequirements
    {
        None = 0,
        Timer = 1,
        Wait = 2,
        ControlModule = 3,
    }

    class SqValidator
    {
        internal void Validate(List<SqProgram> programs, SqRequirements capabilities)
        {
            List<string> messages = new List<string>();

            foreach (var program in programs)
            {
                Validate(program, capabilities, messages);
            }
        }

        private void Validate(SqProgram program, SqRequirements capabilities, List<string> messages)
        {
            bool hasTimer = (capabilities & SqRequirements.Timer) == SqRequirements.Timer;

            var repeatPos = program.Commands.FindIndex(x => x.Cmd == "repeat");
            if (repeatPos != -1 && repeatPos != program.Commands.Count - 1)
            {
                Log.WriteFormat(Parser.LOG_CAT, LogLevel.Warning, "Unreachable code found in @{0}: all commands after /repeat will never executed", program.Name);
            }

            if (repeatPos != -1 && !program.Commands.Take(repeatPos).Any(x => Commands.CmdDefs[x.Cmd].IsWait))
            {
                Log.WriteFormat(Parser.LOG_CAT, LogLevel.Warning, "Where is no any wait command before /repeat in @{0}. Script will wait 1 tock to prevent \"Script Too Complex\" exception", program.Name);
            }

            if (!hasTimer && program.Commands.Any(x => Commands.CmdDefs[x.Cmd].IsWait))
            {
                Log.WriteFormat(Parser.LOG_CAT, LogLevel.Warning, "@{0} contains wait command, but where is no timer to execute it", program.Name);
            }
        }
    }

    #endregion // ingame script end
}
