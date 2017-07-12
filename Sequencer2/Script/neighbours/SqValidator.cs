using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

namespace Script
{
    #region ingame script start


    public enum SqCapablities
    {
        None = 0,
        Timer = 1
    }


    public enum SqRequirements
    {
        None = 0,
        Timer = 1,
        Wait = 2
    }

    class SqValidator
    {
        internal bool Validate(List<SqProgram> programs, SqRequirements capabilities)
        {
            bool allValid = true;
            foreach (var program in programs)
            {
                allValid = allValid && Validate(program, capabilities);
            }
            return allValid;
        }

        private bool Validate(SqProgram program, SqRequirements capabilities)
        {
            // todo: messages

            bool hasWait = false;
            bool hasTimer = (capabilities & SqRequirements.Timer) == SqRequirements.Timer;
            program.IsValid = false;

            foreach (var command in program.Commands)
            {
                var cmdDef = Commands.CommandDefinitions[command.Cmd];

                if ((cmdDef.Requirements & SqRequirements.Timer) == SqRequirements.Timer && !hasTimer)
                {
                    return false;
                }

                if ((cmdDef.Requirements & SqRequirements.Wait) == SqRequirements.Wait && !hasWait)
                {
                    return false;
                }

                hasWait = hasWait || cmdDef.IsWait;

            }

            program.IsValid = true;
            return true;
        }
    }

    #endregion // ingame script end
}
