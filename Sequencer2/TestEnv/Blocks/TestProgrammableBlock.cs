using System;

using System.Text;
using System.Collections;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.ObjectBuilders;
using VRageMath;

namespace SETestEnv
{
    class TestProgrammableBlock : TestBlock, IMyProgrammableBlock
    {
        public TestProgrammableBlock() : base() { }


        public bool Enabled { get; set; }
        public bool IsRunning { get; set; }
        public string TerminalRunArgument { get; set; }

        public void RequestEnable(bool enable)
        {
            throw new NotImplementedException();
        }

        public bool TryRun(string argument)
        {
            throw new NotImplementedException();
        }
    }

}
