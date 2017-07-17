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

    class TestCommandImpl
    {
        internal static CommandRef[] Defs()
        {
            return new CommandRef[] {
                new CommandRef("test1", new ParamRef[] {
                    new ParamRef (ParamType.String),
                    new ParamRef (ParamType.String),
                }, Test1),
            };
        }


        internal static CommandResult Test1(IList args)
        {
            ImplLogger.LogImpl("test1", args);

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            BlockSelector.GetBlocksOfTypeWithQuery<IMyMotorStator>(MatchingType.match, (string)args[0], blocks);
            IMyMotorStator Rotor = blocks.FirstOrDefault() as IMyMotorStator;

            BlockSelector.GetBlocksOfTypeWithQuery<IMyTextPanel>(MatchingType.match, (string)args[1], blocks);
            IMyTextPanel Text = blocks.FirstOrDefault() as IMyTextPanel;

            Text?.WritePublicText(Rotor?.Angle.ToString() ?? "<null>", true);

            return null;
        }
    }

    #endregion // ingame script end
}
