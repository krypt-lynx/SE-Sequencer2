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

            List<IMyMotorStator> rotors = new List<IMyMotorStator>();
            BlockSelector.GetBlocksOfTypeWithQuery(MatchingType.Match, (string)args[0], rotors);
            IMyMotorStator Rotor = rotors.FirstOrDefault();

            List<IMyTextPanel> lcds = new List<IMyTextPanel>();
            BlockSelector.GetBlocksOfTypeWithQuery(MatchingType.Match, (string)args[1], lcds);
            IMyTextPanel Text = lcds.FirstOrDefault();

            Text?.WritePublicText(Rotor?.Angle.ToString() ?? "<null>", true);

            return null;
        }
    }

    #endregion // ingame script end
}
