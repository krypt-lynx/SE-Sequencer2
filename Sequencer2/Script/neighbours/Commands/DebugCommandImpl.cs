using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Script
{

    #region ingame script start

    class DebugCommandImpl
    {
        internal static CommandRef[] Defs()
        {
            return new CommandRef[] {
               new CommandRef("echo", new ParamRef[] {
                    new ParamRef (ParamType.String),
                }, Echo) ,
                new CommandRef("loglevel", new ParamRef[] {
                    new ParamRef (ParamType.String, true, ""),
                    new ParamRef (ParamType.Double, true, -1.0)
                }, LogLevel_),
                new CommandRef("listprops", new ParamRef[] {
                    new ParamRef (ParamType.GroupType, true, MatchingType.Match),
                    new ParamRef (ParamType.String),
                }, ListProps),
                new CommandRef("listactions", new ParamRef[] {
                    new ParamRef (ParamType.GroupType, true, MatchingType.Match),
                    new ParamRef (ParamType.String),
                }, ListActions),
            };
        }

        internal static CommandResult LogLevel_(IList args)
        {
            ImplLogger.LogImpl("loglevel", args);

            string cat = (string)args[0];
            double level = (double)args[1];

            if (string.IsNullOrEmpty((string)args[0]))
            {
                Log.Write("LogLevels:");
                foreach (var kvp in Log.LogLevels)
                {
                    Log.WriteFormat("{0} : {1}", kvp.Key, kvp.Value);
                }
            }
            else
            {
                if (level < 0)
                {
                    Log.WriteFormat("LogLevel for \"{0}\": {1}", cat, Log.LogLevels.ContainsKey(cat) ? Log.LogLevels[cat] : LogLevel.None);
                }
                else
                {
                    if (cat.Equals("all", StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach (var key in new List<string>(Log.LogLevels.Keys))
                        {
                            Log.LogLevels[key] = (LogLevel)level;
                        }
                    }
                    else
                    {
                        Log.LogLevels[cat] = (LogLevel)level;
                    }
                }
            }

            return null;
        }

        internal static CommandResult Echo(IList args)
        {
            ImplLogger.LogImpl("echo", args);

            Log.Write((string)args[0]);

            return null;
        }

        internal static CommandResult ListProps(IList args)
        {
            ImplLogger.LogImpl("listprops", args);

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            BlockSelector.GetBlocksOfTypeWithQuery<IMyTerminalBlock>((MatchingType)args[0], (string)args[1], blocks);
            Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Verbose, "{0} block(s) found", blocks.Count);

            List<ITerminalProperty> props = new List<ITerminalProperty>();
            foreach (var block in blocks)
            {
                Log.WriteFormat("Block \"{0}\" of type \"{1}\" contains properties:", new object[] { block.CustomName, block.GetType().Name });

                block.GetProperties(props);




                foreach (var prop in props)
                {
                    // block.GetValue<object>(prop.Id) - Property is not of Type object <...>
                    // Какая тебе, ***, разница? Просто отдай мне это чёртово свойство!
                    object value = null;

                    switch (prop.TypeName)
                    {
                        case "Boolean":
                            value = block.GetValueBool(prop.Id);
                            break;
                        case "Single":
                            value = block.GetValueFloat(prop.Id);
                            break;
                        case "Color":
                            value = block.GetValueColor(prop.Id);
                            break;
                        case "StringBuilder":
                            value = block.GetValue<StringBuilder>(prop.Id);
                            break;
                        // case "Int64": todo
                        default:
                            throw new Exception(prop.TypeName); // todo: log instead Exception
                    }

                    Log.WriteFormat("\"{0}\" of type \"{1}\", current value is \"{2}\"", new object[] { prop.Id, prop.TypeName, value });
                }
            }

            return null;
        }

        internal static CommandResult ListActions(IList args)
        {
            ImplLogger.LogImpl("listactions", args);

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            BlockSelector.GetBlocksOfTypeWithQuery<IMyTerminalBlock>((MatchingType)args[0], (string)args[1], blocks);
            Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Verbose, "{0} block(s) found", blocks.Count);

            List<ITerminalAction> actions = new List<ITerminalAction>();
            foreach (var block in blocks)
            {
                Log.WriteFormat("Block \"{0}\" of type \"{1}\" contains actions:", new object[] { block.CustomName, block.GetType().Name });

                block.GetActions(actions);

                foreach (var action in actions)
                {
                    Log.WriteFormat("\"{0}\", description: \"{1}\"", new object[] { action.Id, action.Name });
                }
            }

            return null;
        }
    }
 
    #endregion // ingame script end
}
