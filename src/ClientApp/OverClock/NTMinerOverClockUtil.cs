using System;
using System.IO;
using System.Reflection;

namespace NTMiner.OverClock {
    public static class NTMinerOverClockUtil {
        public static void ExtractResource() {
            try {
                if (File.Exists(SpecialPath.NTMinerOverClockFileFullName)) {
                    return;
                }
                Type type = typeof(NTMinerOverClockUtil);
                Assembly assembly = type.Assembly;
                using (var stream = assembly.GetManifestResourceStream(type, Path.GetFileName(SpecialPath.NTMinerOverClockFileFullName))) {
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    File.WriteAllBytes(SpecialPath.NTMinerOverClockFileFullName, data);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
