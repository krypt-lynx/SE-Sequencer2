using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using VRageMath;
using VRage.Game;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Ingame;
using Sandbox.Game.EntityComponents;
using VRage.Game.Components;
using VRage.Collections;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using SETestEnv;

namespace Script
{
    #region ingame script start

    enum LogLevel
    {
        None = 0,
        Error,
        Warning,
        Verbose,
    }

    class Log
    {
        public class Data : ISerializable
        {
            public Dictionary<string, LogLevel> LogLevels = null;
            public bool insertNewFrameSeparator = false;

            public void Deserialize(Deserializer decoder)
            {
                LogLevels = decoder.ReadCollection(
                    () => LogLevels, 
                    () => new KeyValuePair<string, LogLevel>(decoder.ReadString(), decoder.ReadEnum<LogLevel>())
                );
            }

            public void Serialize(Serializer encoder)
            {
                encoder.Write(LogLevels, i => encoder.Write(i.Key).Write(i.Value));
            }
        }

        public static Data D = new Data();
        const string NewFrameSeparator = "--------------------------------------";

        public static Dictionary<string, LogLevel> LogLevels
        {
            get
            {
                return D.LogLevels;
            }
            set
            {
                D.LogLevels = value;
            }
        }

        public static void Write(string logcat, LogLevel level, object anObject)
        {
            if (IsAllowed(logcat, level))
            {
                string str = string.Format("[{0}] {1}", logcat, anObject);
                WriteInternal(str);
            }
        }

        private static bool IsAllowed(string logcat, LogLevel level)
        {
            LogLevel currentLevel = LogLevel.None;
            bool isAllowed = D.LogLevels == null || ((D.LogLevels?.TryGetValue(logcat, out currentLevel) ?? false) && level <= currentLevel);
            return isAllowed;
        }

        public static void WriteFormat(string logcat, LogLevel level, string format, params object[] args)
        {
            if (IsAllowed(logcat, level))
            {
                string str = string.Format("[{0}] {1}", logcat, string.Format(format, args));
                WriteInternal(str);
            }
        }

        /* not used
        public static void WriteLine(string logcat, LogLevel level)
        {
            if (IsAllowed(logcat, level))
            {
                WriteInternal("");
            }
        }
        */

        public static void Write(object anObject)
        {
            if (anObject == null)
            {
                anObject = "<null>";
            }
            string str = anObject.ToString();

            WriteInternal(str);
        }

        public static void WriteFormat(string format, params object[] args)
        {
            WriteInternal(string.Format(format, args));
        }

        public static void WriteLine()
        {
            WriteInternal("");
        }

        private static void WriteInternal(string str)
        {
            if (Program.Current != null)
            {
                Program.Current.Echo(str);
                var dServer = Program.Current.GridTerminalSystem.GetBlockWithName("DebugSrv") as IMyProgrammableBlock;
                if (dServer != null)
                {
                    if (D.insertNewFrameSeparator)
                    {
                        D.insertNewFrameSeparator = false;
                        dServer.TryRun("L" + NewFrameSeparator);
                    }
                    dServer.TryRun("L" + str);
                }
            }
            else
            {
                throw new InvalidOperationException("Program.Current is not assigned");
            }
        }

        internal static void NewFrame()
        {
            D.insertNewFrameSeparator = true;
        }
    }

    #endregion // ingame script end
}
