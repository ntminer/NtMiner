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
                assembly.ExtractManifestResource(type, Path.GetFileName(SpecialPath.NTMinerOverClockFileFullName), SpecialPath.NTMinerOverClockFileFullName);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
