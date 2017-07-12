using System;
using System.Collections;
using System.Collections.Generic;

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

        public CommandRef(string name, ParamRef[] arguments, Func<IList, CommandResult> implementation, 
            int optionalCount = 0, bool aggrerative = false,
            SqRequirements requirements = SqRequirements.None, bool isWait = false)
        {
            Name = name;
            Arguments = arguments;
            Implementation = implementation;
            OptionalCount = optionalCount;
            Aggrerative = aggrerative;
            Requirements = requirements;
            IsWait = isWait;
        }
        
    }

    public enum CommandAction
    {
        None,
        Wait,
        Start,
        Stop,
        Repeat
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
            CommandDefinitions = new Dictionary<string, CommandRef>
            {
                { "run", new CommandRef( "run", new ParamRef[] {
                    new ParamRef (ParamType.GroupType, true, MatchingType.match), 
                    new ParamRef (ParamType.String), // name
                    new ParamRef (ParamType.String), // arg
                }, ApiCommandImpl.Run, optionalCount: 1) },
                { "action", new CommandRef( "action", new ParamRef[] {
                    new ParamRef (ParamType.GroupType, true, MatchingType.match),
                    new ParamRef (ParamType.String), // name
                    new ParamRef (ParamType.String), // action
                }, ApiCommandImpl.Action, optionalCount: 1) },
                { "set", new CommandRef( "set", new ParamRef[] {
                    new ParamRef (ParamType.GroupType, true, MatchingType.match),
                    new ParamRef (ParamType.String), // name
                    new ParamRef (ParamType.String), // prop
                    new ParamRef (ParamType.String), // value
                }, ApiCommandImpl.Set, optionalCount: 1) },
                { "text", new CommandRef( "text", new ParamRef[] {
                    new ParamRef (ParamType.GroupType, true, MatchingType.match),
                    new ParamRef (ParamType.String), // name
                    new ParamRef (ParamType.String), // value
                    new ParamRef (ParamType.Bool, true, false), // append
                }, ApiCommandImpl.Text, optionalCount: 2) },

                { "wait", new CommandRef( "wait", new ParamRef[] {
                    new ParamRef (ParamType.Double), // delay
                }, ExecFlowCommandImpl.Wait, requirements: SqRequirements.Timer, isWait: true) },
                { "waitticks", new CommandRef( "wait", new ParamRef[] {
                    new ParamRef (ParamType.Double), // delay
                }, ExecFlowCommandImpl.WaitTicks, requirements: SqRequirements.Timer, isWait: true) },
                { "repeat", new CommandRef( "wait", new ParamRef[] {
                }, ExecFlowCommandImpl.Repeat, requirements: SqRequirements.Wait) },
                { "start", new CommandRef( "start", new ParamRef[] {
                    new ParamRef (ParamType.String), // func
                }, ExecFlowCommandImpl.Start) },
                { "stop", new CommandRef( "stop", new ParamRef[] {
                    new ParamRef (ParamType.String), // func
                }, ExecFlowCommandImpl.Stop) },

                { "setvar", new CommandRef( "setvar", new ParamRef[] {
                    new ParamRef (ParamType.String), // var name
                    new ParamRef (ParamType.Double), // value
                }, ExecFlowCommandImpl.SetVar) },
                { "switch", new CommandRef( "select", new ParamRef[] {
                    new ParamRef (ParamType.String), // case var name
                    new ParamRef (ParamType.String, aggregative: true), // cases
                }, ExecFlowCommandImpl.Switch, aggrerative: true) },

                { "test1", new CommandRef( "test1", new ParamRef[] {
                    new ParamRef (ParamType.String),
                    new ParamRef (ParamType.String),
                }, TestCommandImpl.Test1) },
                { "listprops", new CommandRef( "listprops", new ParamRef[] {
                    new ParamRef (ParamType.GroupType, true, MatchingType.match),
                    new ParamRef (ParamType.String),
                }, TestCommandImpl.ListProps, optionalCount: 1) },
                { "listactions", new CommandRef( "listactions", new ParamRef[] {
                    new ParamRef (ParamType.GroupType, true, MatchingType.match),
                    new ParamRef (ParamType.String),
                }, TestCommandImpl.ListActions, optionalCount: 1) },

                
                /*{ "waituntil", new CommandRef( "waituntil", new ParamRef[] {
                    new ParamRef (ParamType.GroupType, true, MatchingType.match),
                    new ParamRef (ParamType.String, false), // name
                    new ParamRef (ParamType.String, false), // prop
                    new ParamRef (ParamType.String, false), // is/is not/less then/greater then/smaller then/not greater then/not smaller then
                    new ParamRef (ParamType.String, false), // value
                }, ExecFlowCommandImpl.WaitUntil, optionalCount: 1, requirements: SqRequirements.Timer, isWait: true) },*/ // not implemented :(
            };
        }
    }

    #endregion // ingame script end
}
 