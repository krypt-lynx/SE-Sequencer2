using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace Script
{
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


            List<Tuple<int, string, uint>> packed = new List<Tuple<int, string, uint>>();

            string prev = "";
            StringBuilder data = new StringBuilder();

            List<string> klist = ColorDict.Where(p => p.Value.PackedValue != 0).Select(p => p.Key).ToList();
            klist.Sort();

            foreach (var key in klist)
            {
                var matchesCount = key.Zip(prev, (l, r) => l == r).TakeWhile(x => x).Count();
                prev = key;
                var tail = key.Substring(matchesCount);
                var c = ColorDict[key].PackedValue;
                packed.Add(new Tuple<int, string, uint>(
                    matchesCount,
                    tail,
                    c
                    ));

                data.Append((char)(0xe000 + ((c & 0xFFF000) >> 12)));
                data.Append((char)(0xe000 + (c & 0x000FFF)));
                data.Append((char)(0xe000 + matchesCount + (tail.Length << 4)));
                data.Append(tail);

                // 2296
            };

            */
            
            string data =
"alicebluentiquewhitequamarinezurebeigeisquelackncheda" +
"lmonduevioletrownurlywoodcadetbluehartreuseocolateoral" +
"nflowerbluesilkrimsonyandarkbluecyangoldenrodrayeenk" +
"hakimagentaolivegreenrangechidredsalmoneagreenlateblue" +
"grayturquoisevioleteeppinkskyblueimgrayodgerbluefirebrick" +
"loralwhiteorestgreenuchsiagainsborohostwhiteoldenrodray" +
"eenyellowhoneydewtpinkindianredgovorykhakilavenderbl" +
"ushwngreenemonchiffonightbluecoralyangoldenrodyellowraye" +
"enpinksalmoneagreenkybluelategrayteelblueyellowmegree" +
"nnenmagentaroonediumaquamarineblueorchidpurpleseagreen" +
"latebluepringgreenturquoisevioletredidnightbluentcreamstyrose" +
"occasinnavajowhiteyoldlaceivedrabrangeredchidpaleg" +
"oldenrodreenturquoisevioletredpayawhipeachpuffruinklum" +
"owderblueurpleredosybrownyalbluesaddlebrownlmonndybrown" +
"eagreenshelliennalverkybluelatebluegraynowpringgreent" +
"eelbluetanealhistleomatourquoisevioletwheatitesmoke" +
"yellowgreen";
            // I can trick keens size calculation even more :) 
            // There is only 26 possible chars; fits into 5 bits. Unicode char can store 12 bits.

            ColorDict = new Dictionary<string, Color>();
            StringBuilder key = new StringBuilder();

            char[] dt = data.ToCharArray();
            int i = 0;

            while (i < dt.Length)
            {
                uint hi = (uint)dt[i] - 0xe000; // At 0xe000 Unicode Private Use Area is located. Unicode leaves those chars is as.
                i++;
                uint lo = (uint)dt[i] - 0xe000;
                i++;
                int match = (dt[i] - 0xe000) % 16;
                int len = (dt[i] - 0xe000) / 16;

                key.Remove(match, key.Length - match);
                while (len > 0)
                {
                    i++;
                    key.Append(dt[i]);
                    len--;
                }
                i++;
                ColorDict[key.ToString()] = new Color(0xFF000000 + (hi << 12) + lo);
            }
        }

        static bool TryParseIntGroup(string str, out Color value)
        {
            string[] values = str.Split(" \n\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
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
                            if (uint.TryParse(str, style, null, out p))
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
