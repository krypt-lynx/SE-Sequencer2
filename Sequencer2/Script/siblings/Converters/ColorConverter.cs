using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace Script
{
    /*
    struct ColorEntry
    {
        public int match;
        public string tail;
        public string src;
        public uint color;
    }*/

    /* #override
     * Minify: true
     */
    #region ingame script start

    public static class ColorConverter
    {
        private static Dictionary<string, Color> Colors;

      

        static ColorConverter()
        {   /*
            // Generator

            ColorDict = typeof(Color).GetProperties(System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.GetProperty |
                System.Reflection.BindingFlags.Public)
              .Where(x => x.PropertyType == typeof(Color)).ToDictionary(k => k.Name.ToLower(), v => (Color)v.GetValue(null)) ;
            var map = "abcdefghijklmnopqrstuvwxyz".Select((c, i) => new Tuple<char, int>(c, i)).ToDictionary(t => t.Item1, t => t.Item2);

            List<ColorEntry> packed = new List<ColorEntry>();

            string prev = "";
            StringBuilder data = new StringBuilder();

            List<string> klist = ColorDict.Where(p => p.Value.PackedValue != 0).Select(p => p.Key).ToList();
            klist.Sort();

            foreach (var key in klist)
            {
                var matchesCount = key
                    .Select((x, i) => new Tuple<char, int>(x, i))
                    .Zip(prev, (l, r) => (l.Item1 == r) && (l.Item2 < 7))
                    .TakeWhile(x => x)
                    .Count();
                prev = key;
                var tail = key.Substring(matchesCount);
                var c = ColorDict[key].PackedValue;
                packed.Add(new ColorEntry {
                    match = matchesCount,
                    tail = tail,
                    src = key,
                    color = c
                    });

           

                // 2296
                // 2043
            };

            // v1:    1295
            // v2:    897
            // v3.-1: 898
            // v3:    820
            foreach (var entry in packed)
            {
                var first = entry.tail.First() - 'a';

                var ch1 = (char)(0xe000 + ((entry.color & 0xFFF000) >> 12));
                var ch2 = (char)(0xe000 + (entry.color & 0x000FFF));
                var ch3 = (char)(0xe000 + first + (entry.match << 5) + (entry.tail.Length << 8));
                
                data.Append(ch1);
                data.Append(ch2);
                data.Append(ch3);
                // data.Append(entry.tail);               


                var pairs = entry.tail
                    .Skip(entry.tail.Length % 2)
                    .Select((c, i) => new Tuple<Char, int>(c, i / 2))
                    .GroupBy(t => t.Item2, t => t.Item1)
                    .Select(g => (g.Count() % 2 == 0 ? g : g.Concat('~'.Yield())).ToArray())
                    .ToArray();

                foreach (var pair in  pairs)
                {
                    var ch = (char)(0xe000 + ((pair[0] - 'a') << 6) + (pair[1] - 'a'));
                 
                    data.Append(ch);
                }
            }
            
                        
            // 70331/100000
            // 69964/100000

            // dddd dmmm llll
            // ^^^^ ^^^^ ^^^^  
            */


            var dt = (
                "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                ""
                    ).Select(x => (uint)x & 0xFFF).ToArray();

            Colors = new Dictionary<string, Color>();
            StringBuilder k = new StringBuilder();
            int i = 0;

            while (i < dt.Length)
            {
                uint c = 0xFF000000 + (dt[i++] << 12) + dt[i++];
                int m = (int)(dt[i] & 0xE0) >> 5;
                int l = (int)dt[i] >> 8;
                k.Remove(m, k.Length - m);
                
                for (i += (l + 1) % 2; l > 0; l--)
                {
                    k.Append((char)(((l % 2 == 1 ? dt[i++] : dt[i] >> 6) & 0x1F) + 'a'));   
                }
                Colors[k.ToString()] = new Color(c);
            }
        }

        static bool TryParseIntGroup(string str, out Color value)
        {
            string[] values = str.Split(" ,\n\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            try
            {
                byte[] bytes = values.Select(x => byte.Parse(x)).ToArray();

                switch (bytes.Length)
                {
                    case 3:
                        value = new Color(bytes[0], bytes[1], bytes[2]);
                        return true;
                    case 4:
                        value = new Color(bytes[0], bytes[1], bytes[2], bytes[3]);
                        return true;
                    default:
                        value = default(Color);
                        return false;
                } 
            }
            catch
            {
                value = default(Color);
                return false;
            }
        }

        public static bool TryParseColor(string str, out Color value)
        {
            bool success = true;
            str = str.ToLower();

            if (Colors.ContainsKey(str))
            {
                value = Colors[str];
            }
            else
            {
                if (!TryParseIntGroup(str, out value))
                {
                    uint p;

                    var style = System.Globalization.NumberStyles.HexNumber;
                    value = default(Color);

                    switch (str.Length)
                    {
                        case 6:
                            if (success = uint.TryParse(str, style, null, out p))
                            {
                                value = new Color(p | 0xFF000000);
                            }
                            break;
                        case 8:
                            if (success = uint.TryParse(str, style, null, out p))
                            {
                                value = new Color(p);
                            }
                            break;
                        default:
                            success = false;
                            break;
                    }
                }
            }

            return success;
        }
    }

    #endregion // ingame script end
}
