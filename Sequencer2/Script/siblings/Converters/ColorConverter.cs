using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace Script
{
    struct ColorEntry
    {
        public int match;
        public string tail;
        public string src;
        public uint color;
    }

    #region ingame script start

    public static class ColorConverter
    {
        private static Dictionary<string, Color> ColorDict;

      

        static ColorConverter()
        {
            /*
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
            
            //*/
            /*
            dddd dmmm llll
            ^^^^ ^^^^ ^^^^  
            */

            
            string data =
                "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "" +
                "";
                    

            ColorDict = new Dictionary<string, Color>();
            StringBuilder key = new StringBuilder();
            char[] dt = data.ToCharArray();
            int i = 0;

            while (i < dt.Length)
            {
                uint hi = (uint)dt[i++] - 0xe000; // At 0xe000 Unicode Private Use Area is located. Unicode leaves those chars is as.
                uint lo = (uint)dt[i++] - 0xe000;
                int match = (dt[i] & 0xE0) >> 5;
                int len = ((dt[i] & 0xF00) >> 8);
                key.Remove(match, key.Length - match);

                i += (len + 1) % 2;

                for (; len > 0; len--)
                {
                    key.Append((char)(((len % 2 == 1) ? (dt[i++] & 0x1F) : ((dt[i] & 0xFC0) >> 6)) + 'a'));   
                }
                ColorDict[key.ToString()] = new Color(0xFF000000 + (hi << 12) + lo);
            }
            //*/
            Color test;
            TryParseColor("Red", out test);
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

            if (ColorDict.ContainsKey(str))
            {
                value = ColorDict[str];
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
