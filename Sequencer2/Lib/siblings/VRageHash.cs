using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    // check git history for unminified version

    /* #override
     * Minify: true
     */

    #region ingame script start

    public static class VRageHash // VRage.Utils.MyUtils + VRage.Utils.MyStringHash cocktail
    {
        class D : Dictionary<string, int> { }
        static D m = new D(); // string to hash map
        public static readonly int NullOrEmpty = C("");
        
        public static int GetHash(string s)
        {
            int r;
            if (s == null)
            {
                r = NullOrEmpty;
            }
            else if (!m.TryGetValue(s, out r))
            {
                r = C(s);
                m.Add(s, r);
            }

            return r;
        }

        static int S(int v, int h) // hash step
        {
            h = h ^ v;
            h *= 16777619;
            return h;
        }

        static int C(string s) // calc hash
        {
            int h = 0; // hash

            //two chars per int32
            if (s != null)
            {
                int i = 0;
                for (; i < s.Length - 1; i += 2)
                {
                    h = S((s[i] << 16) + s[i + 1], h);
                }
                if ((s.Length & 1) != 0)
                {//last char
                    h = S(s[i], h);
                }
            }
            return h;
        }
    }

    #endregion // ingame script end
}
