﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{

    #region ingame script start

    class SqCommandBuilder
    {
        public static bool TryCreateCommand(string name, List<string> args, out SqCommand command, out string errorMessage) // todo: cleanup
        {
            command = null;
            errorMessage = null;
            string cmd = name.ToLower();

            if (!Commands.CmdDefs.ContainsKey(cmd) || Commands.CmdDefs[cmd].Hidden)
            {
                errorMessage = string.Format(ErrorMessages.UnknownCommand, name);
                return false;
            }

            var def = Commands.CmdDefs[cmd];

            if ((args.Count > def.Arguments.Length) && !def.Aggregative)
            {

                errorMessage = string.Format(ErrorMessages.TooManyArgumentsIn, name,
                    def.OptionalCount == 0 ?
                    def.Arguments.Length.ToString() :
                    string.Format("from {0} to {1}", def.Arguments.Length - def.OptionalCount, def.Arguments.Length),
                    args.Count
                    );
                return false;
            }

            if (args.Count < def.Arguments.Length - def.OptionalCount)
            {
                string countText;
                if (def.Aggregative)
                {
                    countText = string.Format("at least {0}", def.Arguments.Length - def.OptionalCount - 1);
                }
                else if (def.OptionalCount == 0)
                {
                    countText = string.Format("{0}", def.Arguments.Length);
                }
                else
                {
                    countText = string.Format("from {0} to {1}", def.Arguments.Length - def.OptionalCount, def.Arguments.Length);
                }

                errorMessage = string.Format(ErrorMessages.TooFewArgumentsIn, name,
                    countText,
                    args.Count
                    );
                return false;
            }

            command = new SqCommand();
            command.Cmd = cmd;
            command.Args = new List<object>();
            command.Impl = def.Implementation;

            int allowedOptional = args.Count - def.Arguments.Length + def.OptionalCount + (def.Aggregative ? 1 : 0);

            int argIndex = 0;
            for (int i = 0, imax = def.Arguments.Length; i < imax; i++)
            {
                var argDef = def.Arguments[i];

                if (argDef.Aggregative)
                {
                    if (i != imax - 1)
                    {
                        throw new Exception("invalid method definition");
                    }

                    IList argList = new List<object>();
                    while (argIndex < args.Count)
                    {
                        object value;
                        if (TryParseArgument(args[argIndex], argDef.Type, out value))
                        {
                            argList.Add(value);
                        }
                        else
                        {
                            errorMessage = string.Format(ErrorMessages.InvalidTypeValue, args[argIndex], argDef.Type);
                            return false;
                        }

                        argIndex++;
                    }

                    command.Args.Add(argList);
                }
                else
                {
                    if (!argDef.Optional || allowedOptional > 0)
                    {
                        object value;
                        if (TryParseArgument(args[argIndex], argDef.Type, out value))
                        {
                            command.Args.Add(value);
                        }
                        else
                        {
                            errorMessage = string.Format(ErrorMessages.InvalidTypeValue, args[argIndex], argDef.Type);
                            return false;
                        }
                        argIndex++;

                        if (argDef.Optional)
                        {
                            allowedOptional--;
                        }
                    }
                    else
                    {
                        command.Args.Add(argDef.Default);
                    }
                }
            }

            return true;
        }

        public static bool TryParseArgument(string value, ParamType type, out object result)
        {
            switch (type)
            {
                case ParamType.Bool:
                    {
                        bool b;
                        bool success = bool.TryParse(value, out b);
                        result = b;
                        return success;
                    }
                case ParamType.Double:
                    {
                        double d;
                        bool success = double.TryParse(value, System.Globalization.NumberStyles.Number, C.I, out d);
                        result = d;
                        return success;
                    }
                case ParamType.GroupType:
                    {
                        MatchingType g;
                        bool success = Enum.TryParse(value, true, out g);
                        result = g;
                        return success;
                    }
                case ParamType.String:
                    {
                        result = value;
                        return true;
                    }
                default:
                    result = null;
                    return false;

            }

        }
    }


    public class SqCommand : ISerializable
    {
        public string Cmd;
        public IList Args;
        public Func<IList, CommandResult> Impl;

        public int _cycle = 0;

        public SqCommand() { }

        public SqCommand(string cmd, IList args, Func<IList, CommandResult> impl)
        {
            Cmd = cmd;
            Args = args;
            Impl = impl;
        }

        public override string ToString()
        {
            return Cmd + " " + string.Join(" ", Args.Cast<object>().Select(x => String.Format("\"{0}\"", x)));
        }

        public void Serialize(Serializer enc)
        {
            var map = new Dictionary<Type, Action<object>> {
                       { typeof(string), (i) => enc.Write(0).Write((string)i) },
                       { typeof(bool), (i) => enc.Write(1).Write((bool)i) },
                       { typeof(double), (i) => enc.Write(2).Write((double)i) },
                       { typeof(MatchingType), (i) => enc.Write(3).Write((MatchingType)i) },
                   };

            enc.Write(Cmd)
               .Write(_cycle)
               .Write(Args, (object i) => {

                   map[i.GetType()](i);
               });
        }

        public void Deserialize(Deserializer dec)
        {
            var map = new List<Func<object>> {
                       { () => dec.ReadString() },
                       { () => dec.ReadBool() },
                       { () => dec.ReadDouble() },
                       { () => dec.ReadEnum<MatchingType>() },
                    };

            Cmd = dec.ReadString();
            Impl = Commands.CmdDefs[Cmd].Implementation;
            _cycle = dec.ReadInt();
            Args = dec.ReadCollection(() => new List<object>(), () => map[dec.ReadInt()]());
        }

        internal CommandResult Run()
        {
            return Impl(Args);
        }
    }

    #endregion // ingame script end
}
