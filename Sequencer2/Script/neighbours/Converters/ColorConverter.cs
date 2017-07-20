using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace Script
{
    #region ingame script start

    public static class ColorConverter
    {
        public static bool TryParseColor(string str, out Color value) // todo
        {
            value = Color.Orange;
            return true;
        }
    }

    #endregion // ingame script end
}
