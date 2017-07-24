using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    /* #overrides
    * Squeeze : true
    */

    #region ingame script start

    public static class VRageHash // VRage.Utils.MyUtils + VRage.Utils.MyStringHash cocktail
    {
        static Dictionary<string, int> m_stringToHash = new Dictionary<string, int>();
        public static readonly int NullOrEmpty;

        static VRageHash()
        {
            NullOrEmpty = CalcHash("");
        }

        public static int GetHash(string str)
        {
            int result;
            if (str == null)
            {
                result = NullOrEmpty;
            }
            else if (!m_stringToHash.TryGetValue(str, out result))
            {
                result = CalcHash(str);
                m_stringToHash.Add(str, result);
            }

            return result;
        }

        private static int HashStep(int value, int hash)
        {
            hash = hash ^ value;
            hash *= 16777619;
            return hash;
        }

        private static int CalcHash(string str)
        {
            int hash = 0;

            //two chars per int32
            if (str != null)
            {
                int i = 0;
                for (; i < str.Length - 1; i += 2)
                {
                    hash = HashStep(((int)str[i] << 16) + (int)str[i + 1], hash);
                }
                if ((str.Length & 1) != 0)
                {//last char
                    hash = HashStep((int)str[i], hash);
                }
            }
            return hash;
        }
    }

    #endregion // ingame script end
}
