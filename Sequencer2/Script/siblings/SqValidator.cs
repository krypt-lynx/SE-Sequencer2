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
        ControlModule = 4,
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
            //bool hasTimer = (capabilities & SqRequirements.Timer) == SqRequirements.Timer;
            bool hasCM = (capabilities & SqRequirements.ControlModule) == SqRequirements.ControlModule;

            var repeatPos = program.Commands.FindIndex(x => x.Cmd == "repeat");
            if (repeatPos != -1 && repeatPos != program.Commands.Count - 1)
            {
                Log.WriteFormat(Parser.LOG_CAT, LogLevel.Warning, "Unreachable code found in @{0}: all commands after /repeat will never executed", program.Name);
            }

            if (repeatPos != -1 && !program.Commands.Take(repeatPos).Any(x => Commands.CmdDefs[x.Cmd].IsWait))
            {
                Log.WriteFormat(Parser.LOG_CAT, LogLevel.Warning, "Where is no any wait command before /repeat in @{0}. Script will wait 1 tock to prevent \"Script Too Complex\" exception", program.Name);
            }

            /*
            SqCommand cmd = null;
            if (!hasTimer && (cmd = program.Commands.FirstOrDefault(x => (Commands.CmdDefs[x.Cmd].Requirements & SqRequirements.Timer) != 0)) != null)
            {
                Log.WriteFormat(Parser.LOG_CAT, LogLevel.Warning, "@{0} contains /{1} command, but where is no timer to execute it", program.Name, cmd.Cmd);
            }*/

            SqCommand cmd = null;
            if (!hasCM && (cmd = program.Commands.FirstOrDefault(x => (Commands.CmdDefs[x.Cmd].Requirements & SqRequirements.ControlModule) != 0)) != null)
            {
                Log.WriteFormat(Parser.LOG_CAT, LogLevel.Warning, "@{0} contains /{1} command, but Control Module mod is not loaded", program.Name, cmd.Cmd);
            }

            // if (!hasCM && program.Commands)

        }
    }

    #endregion // ingame script end
}
