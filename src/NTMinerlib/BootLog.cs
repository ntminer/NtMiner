using System;
using System.Collections.Generic;
using System.IO;

namespace NTMiner {
    public static class BootLog {
        private static List<string> lines = new List<string>();
        public static string LogDir;

        static BootLog() {
            if (string.IsNullOrEmpty(LogDir)) {
                LogDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            }
        }

        public static void Log(string s) {
            lines.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}    {s}");
        }

        private static bool _isFirst = true;
        public static void SyncToDisk() {
            bool isFirst = _isFirst;
            if (isFirst) {
                _isFirst = false;
                if (!Directory.Exists(LogDir)) {
                    Directory.CreateDirectory(LogDir);
                }
            }
            var list = lines;
            lines = new List<string>();
            if (isFirst) {
                File.WriteAllLines(Path.Combine(LogDir, "boot.log"), list.ToArray());
            }
            else {
                File.AppendAllLines(Path.Combine(LogDir, "boot.log"), list.ToArray());
            }
        }
    }
}
