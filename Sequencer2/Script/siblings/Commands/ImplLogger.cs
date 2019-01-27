using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    #region ingame script start

    static class ImplLogger
    {
        public const string LOG_CAT = "imp";

        public static void LogImpl(string name, IList args)
        {
            Log.Write(LOG_CAT, LogLevel.Verbose, "--------");
            Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "/{0}", name);
            Log.Write(LOG_CAT, LogLevel.Verbose, string.Join(" ", args.Cast<object>().Select(x => string.Format("\"{0}\"", x.ToString().Replace("\"", "\"\"")))));
        }

        public static void LogBlocks(IList blocks)
        {
            Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "{0} block(s) found", blocks.Count);
        }
    }

    #endregion // ingame script end
}
