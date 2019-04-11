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
                    new ParamRef (ParamType.MatchingType, true, MatchingType.Match),
                    new ParamRef (ParamType.String),
                }, ListProps),
                new CommandRef("listactions", new ParamRef[] {
                    new ParamRef (ParamType.MatchingType, true, MatchingType.Match),
                    new ParamRef (ParamType.String),
                }, ListActions),
                new CommandRef("listblocks", new ParamRef[] {
                    new ParamRef (ParamType.MatchingType, true, MatchingType.Match),
                    new ParamRef (ParamType.String),
                }, ListBlocks),
            };
        }

        internal static void LogLevel_(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("loglevel", args);

            string cat = (string)args[0];
            double level = (double)args[1];

            if (string.IsNullOrEmpty(cat))
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
        }

        internal static void Echo(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("echo", args);
            Log.Write((string)args[0]);
        }

        internal static void ListProps(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("listprops", args);

            MatchingType type = (MatchingType)args[0];
            string filter = (string)args[1];

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            BlockSelector.GetBlocksOfTypeWithQuery<IMyTerminalBlock>(type, filter, blocks);
            ImplLogger.LogBlocks(blocks);

            List<ITerminalProperty> props = new List<ITerminalProperty>();
            foreach (var block in blocks)
            {
                Log.WriteFormat("Block \"{0}\" of type \"{1}\" contains properties:", new object[] { block.CustomName, block.GetType().Name });

                block.GetProperties(props);


                var allProps = new HashSet<string>();
                var badProps = new HashSet<string>(); // Termimal Properties can have same ids, which makes them unaccessible

                foreach (var prop in props)
                {
                    if (allProps.Contains(prop.Id))
                    {
                        badProps.Add(prop.Id);
                    }
                    else
                    {
                        allProps.Add(prop.Id);
                    }
                }

                foreach (var prop in props)
                {
                    // block.GetValue<object>(prop.Id) - Property is not of Type object <...>
                    object value = null;
                    try {
                        PropType propType;
                        if (!badProps.Contains(prop.Id) && Enum.TryParse(prop.TypeName, out propType))
                        {
                            switch (propType)
                            {
                                case PropType.Boolean:
                                    value = block.GetValueBool(prop.Id);
                                    break;
                                case PropType.Single:
                                    value = block.GetValueFloat(prop.Id);
                                    break;
                                case PropType.Color:
                                    value = block.GetValueColor(prop.Id);
                                    break;
                                case PropType.StringBuilder:
                                    value = block.GetValue<StringBuilder>(prop.Id);
                                    break;
                                case PropType.String:
                                    value = block.GetValue<string>(prop.Id);
                                    break;
                                case PropType.Int64:
                                    value = block.GetValue<long>(prop.Id);
                                    break;
                            }
                        }
                    }
                    catch
                    {
                        // Looks like some game mod is broken, which is bad. Game breaking bad.
                        Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Warning, $"Error reading property \"{prop.Id}\"");
                    }
                    Log.WriteFormat("\"{0}\" ({1}) = \"{2}\"", new object[] { prop.Id, prop.TypeName, value });
                }
                Log.WriteLine();
            }
        }

        internal static void ListActions(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("listactions", args);

            MatchingType type = (MatchingType)args[0];
            string filter = (string)args[1];

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            BlockSelector.GetBlocksOfTypeWithQuery<IMyTerminalBlock>(type, filter, blocks);
            ImplLogger.LogBlocks(blocks);

            List<ITerminalAction> actions = new List<ITerminalAction>();
            foreach (var block in blocks)
            {
                Log.WriteFormat("block \"{0}\" of type \"{1}\" have actions:", new object[] { block.CustomName, block.GetType().Name });

                block.GetActions(actions);

                foreach (var action in actions)
                {
                    Log.WriteFormat("\"{0}\": {1}", new object[] { action.Id, action.Name });
                }
                Log.WriteLine();
            }            
        }

        private static void ListBlocks(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("listblocks", args);

            MatchingType type = (MatchingType)args[0];
            string filter = (string)args[1];

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            BlockSelector.GetBlocksOfTypeWithQuery<IMyTerminalBlock>(type, filter, blocks);
            ImplLogger.LogBlocks(blocks);


            Log.Write("TypeName/SubtypeName \"Name\" [IntityId]");
            Log.WriteLine();
            foreach (var block in blocks)
            {
                Log.WriteFormat("{0}/{1} \"{2}\" [{3}]", new object[] {
                    block.GetType().Name,
                    block.BlockDefinition.SubtypeName,
                    block.CustomName,
                    block.EntityId} );
            }            
        }

    }

    #endregion // ingame script end
}
