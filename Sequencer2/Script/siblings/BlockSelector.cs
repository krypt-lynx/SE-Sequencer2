using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    #region ingame script start


    public enum MatchingType
    {
        Match,
        Contains,
        Head,
        Group,
        Type,
    }

    class BlockSelector
    {
        public static void GetBlocksOfTypeWithQuery<T>(MatchingType selectionMode, string query, List<T> blocks) where T : class, IMyTerminalBlock
        {
            switch (selectionMode)
            {
                case MatchingType.Match:
                    {
                        Program.Current.GridTerminalSystem.GetBlocksOfType<T>(blocks, x => x.CustomName.Equals(query));
                        return;
                    }
                case MatchingType.Contains:
                    {
                        Program.Current.GridTerminalSystem.GetBlocksOfType<T>(blocks, x => x.CustomName.Contains(query));
                        return;
                    }
                case MatchingType.Head:
                    {
                        Program.Current.GridTerminalSystem.GetBlocksOfType<T>(blocks, x => x.CustomName.StartsWith(query));
                        return;
                    }
                case MatchingType.Group:
                    {
                        IMyBlockGroup group = Program.Current.GridTerminalSystem.GetBlockGroupWithName(query);
                        blocks.Clear();
                        if (group != null)
                        {
                            List<T> gBlocks = new List<T>();
                            group.GetBlocksOfType<T>(blocks);
                        }
                        return;
                    }
                case MatchingType.Type:
                    {
                        Program.Current.GridTerminalSystem.GetBlocksOfType<T>(blocks, x => x. .StartsWith(query));
                        return;
                    }
            }
        }
    }

    #endregion // ingame script end
}
