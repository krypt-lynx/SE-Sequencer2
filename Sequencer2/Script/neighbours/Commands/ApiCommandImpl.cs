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

        internal static CommandRef[] Defs()
        {
            return new CommandRef[] {
                new CommandRef("run", new ParamRef[] {
                    new ParamRef (ParamType.GroupType, true, MatchingType.Match),
                    new ParamRef (ParamType.String), // name
                    new ParamRef (ParamType.String), // arg
                }, Run),
                new CommandRef("action", new ParamRef[] {
                    new ParamRef (ParamType.GroupType, true, MatchingType.Match),
                    new ParamRef (ParamType.String), // name
                    new ParamRef (ParamType.String), // action
                }, Action),
                new CommandRef("set", new ParamRef[] {
                    new ParamRef (ParamType.GroupType, true, MatchingType.Match),
                    new ParamRef (ParamType.String), // name
                    new ParamRef (ParamType.String), // prop
                    new ParamRef (ParamType.String), // value
                }, Set),
                 new CommandRef("text", new ParamRef[] {
                    new ParamRef (ParamType.GroupType, true, MatchingType.Match),
                    new ParamRef (ParamType.String), // name
                    new ParamRef (ParamType.Bool, true, false), // append
                    new ParamRef (ParamType.String), // value
                }, Text),
                new CommandRef("transmit", new ParamRef[] {
                    new ParamRef (ParamType.GroupType, true, MatchingType.Match),
                    new ParamRef (ParamType.String), // name
                    new ParamRef (ParamType.String, true, "default"), // MyTransmitTarget
                    new ParamRef (ParamType.String), // value
                }, Transmit),
            };
        }

        public static CommandResult Run(IList args)
        {
            ImplLogger.LogImpl("run", args);

            List<IMyProgrammableBlock> blocks = new List<IMyProgrammableBlock>();
            BlockSelector.GetBlocksOfTypeWithQuery<IMyProgrammableBlock>((MatchingType)args[0], (string)args[1], blocks);
            Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Verbose, "{0} block(s) found", blocks.Count);

            foreach (var block in blocks)
            {
                block .TryRun((string)args[2]);
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

                PropType propType;

                if (propDef != null && Enum.TryParse(propDef.TypeName, out propType))
                {
                    switch (propType)
                    {
                        case PropType.Boolean:
                            {
                                bool b;
                                if (bool.TryParse(value, out b))
                                {
                                    block.SetValue(prop, b);
                                }
                                break;
                            }
                        case PropType.StringBuilder:
                            {
                                block.SetValue(prop, new StringBuilder(value));
                                break;
                            }
                        case PropType.Single:
                            {
                                float s;
                                if (float.TryParse(value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out s))
                                {
                                    block.SetValue(prop, s);
                                }
                            }
                            break;
                        case PropType.Int64:
                            {
                                long i;

                                if (ListConverter.ResolveListProperty(prop, value, out i))
                                {
                                    block.SetValue(prop, i);
                                }
                            }
                            break;
                        case PropType.Color:
                            {
                                Color c;

                                if (ColorConverter.TryParseColor(value, out c))
                                {
                                    block.SetValueColor(prop, c); 
                                }
                                else
                                {
                                    Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Warning, "\"{0}\" is not a valid color", value);
                                }
                            }
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

            List<IMyTextPanel> blocks = new List<IMyTextPanel>();
            BlockSelector.GetBlocksOfTypeWithQuery<IMyTextPanel>((MatchingType)args[0], (string)args[1], blocks);
            Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Verbose, "{0} block(s) found", blocks.Count);

            bool append = (bool)args[2];
            string text = (string)args[3];

            foreach (var block in blocks)
            {
                block.WritePublicText(text, append);
            }

            return null;
        }

        public static CommandResult Transmit(IList args_)
        {
            ImplLogger.LogImpl("transmit", args_);

            MatchingType matchingType = (MatchingType)args_[0];
            string filter = (string)args_[1];
            string targetString = (string)args_[2];
            string message = (string)args_[3];

            List<IMyTerminalBlock> antennas = new List<IMyTerminalBlock>();

            List<IMyRadioAntenna> radioAntennas = new List<IMyRadioAntenna>();
            BlockSelector.GetBlocksOfTypeWithQuery<IMyRadioAntenna>(matchingType, filter, radioAntennas);
            Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Verbose, "{0} block(s) found", radioAntennas.Count);

            //get most powerful radio antenna
            IMyRadioAntenna mostPowerfulAntenna = null;

            //get radio antenna with longest radius that's enabled and broadcasting
            foreach (IMyRadioAntenna antenna in radioAntennas)
            {
                if (antenna.Enabled && antenna.GetValueBool("EnableBroadCast")
                    && (mostPowerfulAntenna == null || antenna.Radius > mostPowerfulAntenna.Radius))
                {
                    mostPowerfulAntenna = antenna;
                }
            }

            if (mostPowerfulAntenna != null)
            {
                antennas.Add(mostPowerfulAntenna);
            }

            //--------get all laser antennas
            List<IMyLaserAntenna> laserAntennas = new List<IMyLaserAntenna>();
            BlockSelector.GetBlocksOfTypeWithQuery<IMyLaserAntenna>(matchingType, filter, laserAntennas);
            Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Verbose, "{0} block(s) found", laserAntennas.Count);

            foreach (IMyLaserAntenna antenna in laserAntennas)
            {
                if (antenna.Status == MyLaserAntennaStatus.Connected)
                {
                    antennas.Add(antenna);
                }
            }

            //-----check whether at least one valid antenna was found
            if (antennas.Count != 0)
            {
                var transmitter = new Transmitter(antennas);
                transmitter.Transmit(message, targetString);
            }
            else
            {
                string warning;
                switch (matchingType)
                {
                    default:
                    case MatchingType.Match:
                        warning = string.Format("No antennas called \"{0}\" are currently able to transmit.", filter);
                        break;
                    case MatchingType.Contains:
                        warning = string.Format("No antennas containing \"{0}\" are currently able to transmit.", filter);
                        break;
                    case MatchingType.Head:
                        warning = string.Format("No antennas starting with \"{0}\" are currently able to transmit.", filter);
                        break;
                    case MatchingType.Group:
                        warning = string.Format("No antennas in group \"{0}\" are currently able to transmit.", filter);
                        break;
                }

                Log.WriteFormat(ImplLogger.LOG_CAT, LogLevel.Warning, warning);
            }

            return null;
        }
    }

    #endregion // ingame script end
}
