using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{

    #region ingame script start

    static class WatchDog
    {
        public static void Reply(string arg)
        {
            long id;

            if (long.TryParse(arg, out id))
            {
                var block = Program.Current.GridTerminalSystem.GetBlockWithId(id);
                if (block != null)
                {
                    block.CustomData = string.Format("pong {0}", Program.Current.Me.EntityId);
                }
            }
        }
    }

    #endregion // ingame script end

}
