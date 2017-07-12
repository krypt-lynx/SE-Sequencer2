using System;
using System.Linq;
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
using console;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace SETestEnv
{

    class TestTimerBlock : TestBlock, IMyTimerBlock
    {
        public bool Enabled
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsCountingDown
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool Silent
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public float TriggerDelay
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void RequestEnable(bool enable)
        {
            throw new NotImplementedException();
        }

        public void StartCountdown()
        {
            throw new NotImplementedException();
        }

        public void StopCountdown()
        {
            throw new NotImplementedException();
        }

        public void Trigger()
        {
            throw new NotImplementedException();
        }
    }
}
