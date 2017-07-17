using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    #region ingame script start

    public enum ParamType
    {
        String,
        Bool,
        Double,
        GroupType
    }

    public struct ParamRef
    {
        public ParamType Type;
        public bool Optional;
        public bool Aggregative;
        public object Default;

        public ParamRef(ParamType type, bool optional = false, object default_ = null, bool aggregative = false)
        {
            Type = type;
            Optional = optional;
            Default = default_;
            Aggregative = aggregative;
        }
    }

    public struct CommandRef
    {
        public string Name;
        public ParamRef[] Arguments;
        public Func<IList, CommandResult> Implementation;
        public int OptionalCount;
        public bool Aggrerative; // arrgigative parameter must be last one
        public SqRequirements Requirements;
        public bool IsWait;
        public bool Hidden;

        public CommandRef(string name, ParamRef[] arguments, Func<IList, CommandResult> implementation,
            SqRequirements requirements = SqRequirements.None, bool isWait = false, bool hidden = false)
        {
            Name = name;
            Arguments = arguments;
            Implementation = implementation;
            OptionalCount = arguments.Count(x => x.Optional);
            Aggrerative = arguments.Any(x => x.Aggregative);
            Requirements = requirements;
            IsWait = isWait;
            Hidden = hidden;
        }
        
    }

    public enum CommandAction
    {
        None,
        Wait,
        Start,
        Stop,
        Repeat,

        AddMethods,
        RemoveMethods
    }

    public class CommandResult
    {
        public CommandAction Action;
        public object Data; 
    }

    static class Commands
    {
        public static Dictionary<string, CommandRef> CommandDefinitions;

        static Commands()
        {
            List<CommandRef> cmdDefs = new List<CommandRef>();

            cmdDefs.AddRange(ExecFlowCommandImpl.Defs());
            cmdDefs.AddRange(ApiCommandImpl.Defs());
            cmdDefs.AddRange(DebugCommandImpl.Defs());
            cmdDefs.AddRange(TestCommandImpl.Defs()); // todo: Remove before release!

            CommandDefinitions = cmdDefs.ToDictionary(x => x.Name);
        }
    }

    #endregion // ingame script end
}
 