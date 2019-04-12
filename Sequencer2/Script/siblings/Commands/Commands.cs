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
        String = 0,
        Bool,
        Double,
        MatchingType,
        DataPermision,
        InputEvent,
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

    class CommandRef
    {
        public string Name;
        public ParamRef[] Arguments;
        public Action<IList, IMethodContext> Implementation;
        public int OptionalCount;
        public bool Aggregative; // aggregative parameter must be last one
        public SqRequirements Requirements;
        public bool IsWait;
        public bool Hidden;

        public CommandRef(string name, ParamRef[] arguments, Action<IList, IMethodContext> implementation,
            SqRequirements requirements = SqRequirements.None, bool isWait = false, bool hidden = false)
        {
            Name = name;
            Arguments = arguments;
            Implementation = implementation;
            OptionalCount = arguments.Count(x => x.Optional);
            Aggregative = arguments.Any(x => x.Aggregative);
            Requirements = requirements;
            IsWait = isWait;
            Hidden = hidden;
        }
        
        public SqCommand Materialize(IList args)
        {
            return new SqCommand(Name, args, Implementation);
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
        public static Dictionary<string, CommandRef> CmdDefs;

        static Commands()
        {
            List<CommandRef> cmdDefs_ = new List<CommandRef>();

            cmdDefs_.AddRange(ExecFlowCommandImpl.Defs());
            cmdDefs_.AddRange(ApiCommandImpl.Defs());
            cmdDefs_.AddRange(DebugCommandImpl.Defs());
            cmdDefs_.AddRange(CMCommandImpl.Defs());
            cmdDefs_.AddRange(TestCommandImpl.Defs()); // todo: Remove before release!

            CmdDefs = cmdDefs_.ToDictionary(x => x.Name);
        }
    }

    #endregion // ingame script end
}
 