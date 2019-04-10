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
        public static void GetBlocksOfTypeWithQuery<T>(MatchingType selectionMode, string query, List<T> blocks) where T : class
        {
            switch (selectionMode)
            {
                case MatchingType.Match:
                    {
                        Program.Current.GridTerminalSystem.GetBlocksOfType(blocks, x => (x as IMyTerminalBlock)?.CustomName?.Equals(query) ?? false);
                        return;
                    }
                case MatchingType.Contains:
                    {
                        Program.Current.GridTerminalSystem.GetBlocksOfType(blocks, x => (x as IMyTerminalBlock)?.CustomName?.Contains(query) ?? false);
                        return;
                    }
                case MatchingType.Head:
                    {
                        Program.Current.GridTerminalSystem.GetBlocksOfType(blocks, x => (x as IMyTerminalBlock)?.CustomName?.StartsWith(query) ?? false);
                        return;
                    }
                case MatchingType.Group:
                    {
                        IMyBlockGroup group = Program.Current.GridTerminalSystem.GetBlockGroupWithName(query);
                        blocks.Clear();
                        if (group != null)
                        {
                            group.GetBlocksOfType<T>(blocks);
                        }
                        return;
                    }
                case MatchingType.Type:
                    {
                        string[] parts = query.Split("|/:, ".ToCharArray()).Select(x => x.Trim()).ToArray();

                        bool allTypes = parts[0] == "" || parts[0] == "*";
                        string type = "My" + parts[0];

                        bool allSubtypes = true;
                        string subtype = "";

                        if (parts.Length > 1)
                        {
                            allSubtypes = parts[1] == "*";
                            subtype = parts[1];
                        }

                        Program.Current.GridTerminalSystem.GetBlocksOfType(blocks, block =>
                        {
                            return (allTypes || block.GetType().Name == type) &&
                                   (allSubtypes || (block as IMyTerminalBlock)?.BlockDefinition.SubtypeName == subtype);
                        });

                        return;
                    }
            }
        }
    }

    #endregion // ingame script end
}
