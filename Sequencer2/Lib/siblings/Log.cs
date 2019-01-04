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
        None,
        Error,
        Warning,
        Verbose,
        SpamMeToDeath
    }

    class Log
    {
        public static Dictionary<string, LogLevel> LogLevels = null;
        const string NewFrameSeparator = "--------------------------------------";
        static bool insertNewFrameSeparator = false;

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
            bool isAllowed = LogLevels == null || ((LogLevels?.TryGetValue(logcat, out currentLevel) ?? false) && level <= currentLevel);
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
                    if (insertNewFrameSeparator)
                    {
                        insertNewFrameSeparator = false;
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
            insertNewFrameSeparator = true;
        }

        internal static void Deserialize(Deserializer decoder)
        {
            if (decoder.ReadBool())
            {
                int count = decoder.ReadInt();
                LogLevels = new Dictionary<string, LogLevel>();
                for (int i = 0; i < count; i++)
                {
                    LogLevels[decoder.ReadString()] = (LogLevel)decoder.ReadInt();
                }
            }
        }

        internal static void Serialize(Serializer encoder)
        {
            encoder.Write(LogLevels != null);
            if (LogLevels != null)
            {
                encoder.Write(LogLevels.Count);
                foreach (var kvp in LogLevels)
                {
                    encoder.Write(kvp.Key);
                    encoder.Write((int)kvp.Value);
                }
            }
        }
    }

    #endregion // ingame script end
}
