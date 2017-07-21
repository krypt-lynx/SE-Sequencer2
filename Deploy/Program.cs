using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Publish
{


    class Program
    {
        static Opt options = new Opt(
            new Dictionary<Opt.Key, Opt.Value>() {
                { Opt.Key.InsertFileName, Opt.Value.Yes },
                { Opt.Key.TrimLeadingSpaces, Opt.Value.Yes },
                { Opt.Key.TrimTrailingSpaces, Opt.Value.Yes },
                { Opt.Key.StripEmptyLines, Opt.Value.No },
                { Opt.Key.TrimComments, Opt.Value.No },
                { Opt.Key.ForceLocalNotImplementedException, Opt.Value.Yes },  // Unaccurate!
                { Opt.Key.Squeeze, Opt.Value.No } // Very unaccurate!
        });

        class Opt
        {
            public enum Key
            {
                InsertFileName,
                TrimLeadingSpaces,
                TrimTrailingSpaces,
                StripEmptyLines,
                TrimComments,
                ForceLocalNotImplementedException,
                Squeeze,
                IgnoreFile
            }

            public enum Value
            {
                Yes,
                No,
                Force
            }

            public Opt(Dictionary<Key, Value> options)
            {
                this.options = options;
            }

            Dictionary<Key, Value> options;


            public bool Allowed(Key option)
            {
                if (!options.ContainsKey(option))
                {
                    return false;
                }
                else
                {
                    return options[option] != Value.No;
                }
            }

            public static Opt Merge(Opt a, Opt b)
            {
                if (b == null)
                {
                    return a;
                }
                var options = new Dictionary<Key, Value>(a.options);

                foreach (var k in b.options.Keys)
                {
                    Value val;
                    if (!options.TryGetValue(k, out val) || val != Value.Force)
                    {
                        options[k] = b.options[k];
                    }
                }

                return new Opt(options);
            }
        }


        static string infoPath = "../../../Sequencer2/Info.txt";
        static string mainPath = "../../../Sequencer2/Sequencer2.cs";
        static string[] rootPaths = {  "../../../Sequencer2/Script/",
                                       "../../../Sequencer2/Lib/" };


        const int logOffset = 2;

        [STAThread]
        static void Main(string[] args)
        {
            string appRoot = AppDomain.CurrentDomain.BaseDirectory;

            StringBuilder scriptBuilder = new StringBuilder();

            Console.WriteLine("Processing Header...");
            if (File.Exists(infoPath))
            {
                scriptBuilder.Append(File.ReadAllText(infoPath));
            }
            scriptBuilder.Append("\n");

            Console.WriteLine("Processing main...");
            scriptBuilder.Append(ProcessFile(mainPath));
            scriptBuilder.Append("\n");

            Console.WriteLine("Processing Program class code...");
            foreach (var root in rootPaths)
            {
                Console.WriteLine(new string(' ', logOffset) + root);
                var files = Directory.EnumerateFiles(root + "nested/", "*.cs", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    Console.Write(new string(' ', logOffset*2) + file);

                    var text = ProcessFile(file);
                    if (text != null)
                    {
                        scriptBuilder.Append(text);
                        scriptBuilder.Append("\n");

                        Console.Write(string.Format(" +{0} symbols", text.Length));
                    }
                    Console.WriteLine();
                }
            }

            scriptBuilder.Append("}\n");

            Console.WriteLine("Processing neighbour classes...");
            foreach (var root in rootPaths)
            {
                Console.WriteLine(new string(' ', logOffset) + root);
                var files = Directory.EnumerateFiles(root + "neighbours/", "*.cs", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    Console.Write(new string(' ', logOffset * 2) + file);

                    var text = ProcessFile(file);
                    if (text != null)
                    {
                        scriptBuilder.Append(text);
                        scriptBuilder.Append("\n");

                        Console.Write(string.Format(" +{0} symbols", text.Length));
                    }
                    Console.WriteLine();
                }
            }

            string script = scriptBuilder.ToString();
            int blockEndPos = script.ToString().LastIndexOf("}");
            script = script.Remove(blockEndPos, 1);


            Console.WriteLine("Done!");
            Console.WriteLine("Script length: {0}/100000", script.Length.ToString());
            System.Windows.Forms.Clipboard.SetText(script);
            Console.ReadKey();
        }

        const string startMark = "#region ingame script start";
        const string endMark = "#endregion // ingame script end";

        const string startOptionsMark = "/* #override";

        const string testenvMark = "//testenv";

        static string ProcessFile(string path)
        {
            string src = File.ReadAllText(path);

            int start = src.IndexOf(startMark);
            int end = src.IndexOf(endMark);

            var fileOptions = GetFileOptions(src);
            var mergedOptions = Opt.Merge(options, fileOptions);

            if (mergedOptions.Allowed(Opt.Key.IgnoreFile))
            {
                return null;
            }

            if (start == -1)
            {
                string message = string.Format("\nCode start not specified in file {0}", path);
                Console.WriteLine(message);
                throw new Exception(message);
            }

            if (end == -1)
            {
                string message = string.Format("Code end not specified in file {0}", path);
                Console.WriteLine(message);
                throw new Exception(message);
            }

            src = src.Substring(start + startMark.Length, end - start - startMark.Length);
            src = src.Replace("\r", "");
            src = src.Trim("\n ".ToCharArray());


            if (mergedOptions.Allowed(Opt.Key.TrimComments))
            {
                // http://stackoverflow.com/questions/3524317/regex-to-strip-line-comments-from-c-sharp
                var blockComments = @"/\*(.*?)\*/";
                var lineComments = @"//(.*?)\r?\n";
                var strings = @"""((\\[^\n]|[^""\n])*)""";
                var verbatimStrings = @"@(""[^""]*"")+";

                src = Regex.Replace(src, 
                    blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
                    me => {
                        if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
                        return me.Value.StartsWith("//") ? Environment.NewLine : "";

                        return me.Value;
                    },
                    RegexOptions.Singleline);
           }



            string[] lines = src.Split('\n');
            List<string> newLines = new List<string>();

            for (int i = 0, imax = lines.Length; i < imax; i++)
            {
                string line = lines[i];

                if (mergedOptions.Allowed(Opt.Key.TrimLeadingSpaces))
                    line = line.TrimStart();

                if (mergedOptions.Allowed(Opt.Key.TrimTrailingSpaces))
                    line = line.TrimEnd();

                if (mergedOptions.Allowed(Opt.Key.StripEmptyLines))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                }

                if (mergedOptions.Allowed(Opt.Key.ForceLocalNotImplementedException))
                {
                    line = line.Replace("System.NotImplementedException", "NotImplementedException");
                }

                newLines.Add(line);
            }

            StringBuilder sb = new StringBuilder();
            if (mergedOptions.Allowed(Opt.Key.InsertFileName))
            {
                sb.Append("\n// ----------\n");
                sb.Append("// " + Path.GetFileName(path) + "\n\n");
            }

            string scriptPart = string.Join("\n", newLines);

            if (mergedOptions.Allowed(Opt.Key.Squeeze)) // todo: regex
            {
                SqueezeWhitespaces(ref scriptPart, " ");
                SqueezeWhitespaces(ref scriptPart, "\n");

                SqueezeWhitespaces(ref scriptPart, "{");
                SqueezeWhitespaces(ref scriptPart, "}");

                SqueezeWhitespaces(ref scriptPart, "(");
                SqueezeWhitespaces(ref scriptPart, ")");

                SqueezeWhitespaces(ref scriptPart, "[");
                SqueezeWhitespaces(ref scriptPart, "]");

                SqueezeWhitespaces(ref scriptPart, "<");
                SqueezeWhitespaces(ref scriptPart, ">");
                SqueezeWhitespaces(ref scriptPart, "+");
                SqueezeWhitespaces(ref scriptPart, "-");
                SqueezeWhitespaces(ref scriptPart, "*");
                SqueezeWhitespaces(ref scriptPart, "/");
                SqueezeWhitespaces(ref scriptPart, ".");

                SqueezeWhitespaces(ref scriptPart, ";");
                SqueezeWhitespaces(ref scriptPart, ",");
                SqueezeWhitespaces(ref scriptPart, ":");
                SqueezeWhitespaces(ref scriptPart, "=");
            }

            sb.Append(scriptPart);

            return sb.ToString();
        }

        static void SqueezeWhitespaces(ref string str, string entry)
        {
            int oldLen;
            do
            {
                oldLen = str.Length;

                str = str.Replace(entry + " ", entry);
                str = str.Replace(entry + "\n", entry);
                str = str.Replace(" " + entry, entry);
                str = str.Replace("\n" + entry, entry);

            } while (oldLen != str.Length);
        }


        static Opt GetFileOptions(string src)
        {
            int start = src.IndexOf(startOptionsMark);
            if (start == -1)
            {
                return null;
            }

            int end = src.IndexOf("*/", start);

            string optionsStr = src.Substring(start + startOptionsMark.Length, end - start - startOptionsMark.Length);
            var optionsDict = optionsStr.Split('\n') 

                                    .Select(s => s.Trim("\n\t\r* ".ToCharArray()))
                                    .Where(s => s.Length != 0)
                                    .Select(s => s.Split(':')
                                        .Select(s2 => s2.Trim())
                                        .ToArray()
                                    )
                                    .SelectMany((string[] arr) =>
                                    {
                                        if (arr.Length < 2)
                                            return new KeyValuePair<Opt.Key, Opt.Value>[] { };

                                        Opt.Key key;
                                        bool value;

                                        if (!Enum.TryParse<Opt.Key>(arr[0], out key))
                                            return new KeyValuePair<Opt.Key, Opt.Value>[] { };
                                        if (!bool.TryParse(arr[1], out value))
                                            return new KeyValuePair<Opt.Key, Opt.Value>[] { };

                                        return new KeyValuePair<Opt.Key, Opt.Value>[] {
                                            new KeyValuePair<Opt.Key, Opt.Value>(key, value?Opt.Value.Yes:Opt.Value.No)
                                        };
                                    })
                                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value); // linq madness

            return new Opt(optionsDict);
        }

    }
}
