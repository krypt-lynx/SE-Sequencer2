using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    #region ingame script start


    public enum MatchingType // lowercase is requared
    {
        match,
        contains,
        head,
        group
    }

    class BlockSelector
    {
        public static void GetBlocksOfTypeWithQuery<T>(MatchingType selectionMode, string query, List<IMyTerminalBlock> blocks) where T : class
        {
            switch (selectionMode)
            {
                case MatchingType.match:
                    {
                        Program.Current.GridTerminalSystem.GetBlocksOfType<T>(blocks, x => x.CustomName.Equals(query));
                        return;
                    }
                case MatchingType.contains:
                    {
                        Program.Current.GridTerminalSystem.GetBlocksOfType<T>(blocks, x => x.CustomName.Contains(query));
                        return;
                    }
                case MatchingType.head:
                    {
                        Program.Current.GridTerminalSystem.GetBlocksOfType<T>(blocks, x => x.CustomName.StartsWith(query));
                        return;
                    }
                case MatchingType.group:
                    {
                        IMyBlockGroup group = Program.Current.GridTerminalSystem.GetBlockGroupWithName(query);
                        blocks.Clear();
                        if (group != null)
                        {
                            List<IMyTerminalBlock> gBlocks = new List<IMyTerminalBlock>();
                            group.GetBlocksOfType<T>(blocks);
                        }
                        return;
                    }
            }
        }
    }

    #endregion // ingame script end
}
