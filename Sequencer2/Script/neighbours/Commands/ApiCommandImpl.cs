using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace Script
{

    #region ingame script start

    class ApiCommandImpl
    {
        public static CommandResult Run(IList args)
        {
            ImplLogger.LogImpl("run", args);

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            BlockSelector.GetBlocksOfTypeWithQuery<IMyProgrammableBlock>((MatchingType)args[0], (string)args[1], blocks);
            Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Verbose, "{0} block(s) found", blocks.Count);

            foreach (var block in blocks)
            {
                (block as IMyProgrammableBlock).TryRun((string)args[2]);
            }

            return null;
        }

        public static CommandResult Action(IList args)
        {
            ImplLogger.LogImpl("action", args);

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            BlockSelector.GetBlocksOfTypeWithQuery<IMyTerminalBlock>((MatchingType)args[0], (string)args[1], blocks);
            Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Verbose, "{0} block(s) found", blocks.Count);

            string action = (string)args[2];

            foreach (var block in blocks)
            {
                if (block.HasAction(action))
                {
                    block.ApplyAction(action);
                }
                else
                {
                    Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Warning, "block \"{0}\" does not support action \"{1}\", ignoring", block.CustomName, action);
                }
            }

            return null;
        }

        public static CommandResult Set(IList args)
        {
            ImplLogger.LogImpl("set", args);

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            BlockSelector.GetBlocksOfTypeWithQuery<IMyTerminalBlock>((MatchingType)args[0], (string)args[1], blocks);
            Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Verbose, "{0} block(s) found", blocks.Count);

            string prop = (string)args[2];
            string value = (string)args[3];

            // Boolean
            // StringBuilder
            // Single
            // Int64 (list item hash, collent key-hash pairs?)
            // Color (need color parser)

            foreach (var block in blocks)
            {
                // todo: redo
                var propDef = block.GetProperty(prop);

                List<ITerminalProperty> props = new List<ITerminalProperty>();
                block.GetProperties(props);

                if (propDef != null)
                {
                    switch (propDef.TypeName) // todo
                    {
                        case "Boolean":
                            {
                                bool b;
                                if (bool.TryParse(value, out b))
                                {
                                    block.SetValue(prop, b);
                                }
                                break;
                            }
                        case "StringBuilder":
                            {
                                block.SetValue(prop, new StringBuilder(value));
                                break;
                            }
                        case "Single":
                            {
                                float s;
                                if (float.TryParse(value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out s))
                                {

                                    block.SetValue(prop, s);
                                }
                            }
                            break;
                        case "Int64":
                            {
                                long i;
                                if (long.TryParse(value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out i))
                                {
                                    block.SetValue(prop, i);
                                }
                            }
                            break;
                        case "Color":
                            Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Warning, "Color property parsing is not impelemented. Now color is orange :)");

                            block.SetValueColor(prop, Color.OrangeRed); // todo
                            break;
                    }
                }
                else
                {
                    Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Warning, "block \"{0}\" does not have property \"{1}\", ignoring", block.CustomName, prop);
                }
            }

            return null;
        }

        internal static CommandResult Text(IList args)
        {
            ImplLogger.LogImpl("text", args);

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            BlockSelector.GetBlocksOfTypeWithQuery<IMyTextPanel>((MatchingType)args[0], (string)args[1], blocks);
            Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Verbose, "{0} block(s) found", blocks.Count);

            foreach (var block in blocks)
            {
                ((IMyTextPanel)block).WritePublicText((string)args[2], (bool)args[3]);
            }

            return null;
        }
    }

    #endregion // ingame script end
}
