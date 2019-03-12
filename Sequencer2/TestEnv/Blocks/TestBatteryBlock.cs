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
    class TestBatteryBlock : TestBlock, IMyBatteryBlock
    {
        public ChargeMode ChargeMode
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

        public float CurrentInput
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float CurrentOutput
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float CurrentStoredPower
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool Enabled
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool HasCapacityRemaining
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsCharging
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float MaxInput
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float MaxOutput
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float MaxStoredPower
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool OnlyDischarge
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

        public bool OnlyRecharge
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

        public bool SemiautoEnabled
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

        bool IMyFunctionalBlock.Enabled
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
    }
}
