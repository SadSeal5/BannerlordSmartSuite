using System;
using TaleWorlds.Library;
using BannerlordSmartSuite.Core.Config;

namespace BannerlordSmartSuite.Core.Logging
{
    public static class SuiteLogger
    {
        private const string LogPrefix = "[BannerlordSmartSuite] ";

        public static void Log(string message)
        {
            try
            {
                Debug.Print(LogPrefix + message);
            }
            catch { }
        }

        public static void DebugLog(string message)
        {
            try
            {
                if (Settings.Instance.DebugLogging)
                {
                    Debug.Print(LogPrefix + "[DEBUG] " + message);
                }
            }
            catch { }
        }

        public static void Warn(string message)
        {
            try
            {
                Debug.Print(LogPrefix + "[WARN] " + message);
            }
            catch { }
        }

        public static void Error(string message, Exception ex = null)
        {
            try
            {
                if (ex != null)
                {
                    Debug.Print(LogPrefix + "[ERROR] " + message + " " + ex);
                }
                else
                {
                    Debug.Print(LogPrefix + "[ERROR] " + message);
                }
            }
            catch { }
        }
    }
}
