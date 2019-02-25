using System;
using System.IO;
using System.Reflection;

namespace NTMiner.OverClock {
    public static class NTMinerOverClockUtil {
        private static string processName = "NTMinerOverClock";
        private static string tempDir = SpecialPath.TempDirFullName;
        private static string fileFullName = Path.Combine(tempDir, processName + ".exe");

        public static void ExtractResource() {
            try {
                if (File.Exists(fileFullName)) {
                    return;
                }
                Type type = typeof(NTMinerOverClockUtil);
                Assembly assembly = type.Assembly;
                using (var stream = assembly.GetManifestResourceStream(type, processName + ".exe")) {
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    File.WriteAllBytes(fileFullName, data);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
