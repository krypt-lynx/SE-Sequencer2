using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{

    #region ingame script start

    class ParamsRouter
    {
        public Dictionary<string, Action<string>> Cases = null;
        public Action NoArgsCase = null;
        public Action<string> UnknownCase = null;


        public bool Route(string arg)
        {
            arg = arg.TrimStart();
            int firstSpace = arg.IndexOf(' ');
            string command = null;
            string tail = null;
            if (firstSpace != -1)
            {
                command = arg.Substring(0, firstSpace);
                tail = arg.Substring(firstSpace + 1);
            }
            else
            {
                command = arg;
                tail = "";
            }

            if (!string.IsNullOrEmpty(command))
            {
                if (Cases?.ContainsKey(command) ?? false)
                {
                    Cases[command](tail);
                }
                else
                {
                    if (UnknownCase != null)
                    {
                        UnknownCase(arg);
                    }
                }
            }
            else
            {
                if (NoArgsCase != null)
                {
                    NoArgsCase();
                }
            }
           
            return false;

        }
    }

    #endregion // ingame script end
}
