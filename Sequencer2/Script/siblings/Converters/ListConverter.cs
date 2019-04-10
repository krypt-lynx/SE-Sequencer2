using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace Script
{
    #region ingame script start

    enum PropType
    {
        Boolean,
        StringBuilder,
        Single,
        Int64,
        Color,
        String,
    }

    public static class ListConverter
    {
        delegate bool TryGet(string str, out long value);

        static Dictionary<string, long> flightModes = new Dictionary<string, long> {
            { "patrol", 0 },
            { "circle", 1 },
            { "oneway", 2 },
        };

        static Dictionary<string, long> filterTypes = new Dictionary<string, long> {
            { "blacklist", 0 },
            { "whitelist", 1 },
        };

        static Dictionary<string, TryGet> knownLists = new Dictionary<string, TryGet>() {
            { "CameraList", TryGetBlockId<IMyCameraBlock> },
            { "FlightMode", (string str, out long value) => flightModes.TryGetValue(str.ToLower(), out value) },
            { "Direction", TryParseEnum<Base6Directions.Direction> },
            { "blacklistWhitelist", (string str, out long value) => filterTypes.TryGetValue(str.ToLower(), out value) },
            { "PBList", TryGetBlockId<IMyProgrammableBlock> },
            { "Font",  (string str, out long value) => { value = VRageHash.GetHash(str); return true; } },
            { "Content", TryParseEnum<ContentType> },
        };

        static bool TryParseEnum<T>(string str, out long value) where T : struct
        {
            T e;
            bool r = Enum.TryParse(str, true, out e);
            value = Convert.ToInt64(e);
            return r;
        }

        static bool TryGetBlockId<T>(string str, out long value) where T : class, IMyTerminalBlock
        {
            List<T> blocks = new List<T>();
            Program.Current.GridTerminalSystem.GetBlocksOfType(blocks, x => x.CustomName.Equals(str));
            long? id = blocks.FirstOrDefault()?.EntityId;

            value = id ?? 0;
            return id.HasValue;
        }
        
        public static bool ResolveListProperty(string prop, string str, out long value)
        {
            if (knownLists.ContainsKey(prop) && knownLists[prop](str, out value))
            {
                return true;
            }

            if (long.TryParse(str, System.Globalization.NumberStyles.Number, C.I, out value))
            {
                return true;
            }

            value = 0;
            return false;
        }
    }

    #endregion // ingame script end
}
