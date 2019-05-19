using System;
using System.IO;
using System.Reflection;

/// <summary>
/// 注意不要挪动这里的命名空间也不要挪动该代码文件所处的程序集
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
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
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
