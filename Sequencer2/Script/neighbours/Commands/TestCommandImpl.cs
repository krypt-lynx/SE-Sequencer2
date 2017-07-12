﻿using Sandbox.ModAPI.Ingame;
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

    class TestCommandImpl
    {
        internal static CommandResult Test1(IList args)
        {
            ImplLogger.LogImpl("test1", args);

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            BlockSelector.GetBlocksOfTypeWithQuery<IMyMotorStator>(MatchingType.match, (string)args[0], blocks);
            IMyMotorStator Rotor = blocks.FirstOrDefault() as IMyMotorStator;

            BlockSelector.GetBlocksOfTypeWithQuery<IMyTextPanel>(MatchingType.match, (string)args[1], blocks);
            IMyTextPanel Text = blocks.FirstOrDefault() as IMyTextPanel;

            Text.WritePublicText(Rotor.Angle.ToString(), true);

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
                        default:
                            throw new Exception(prop.TypeName);
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
