using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NTMiner {
    public static class EntryAssemblyInfo {
        public const string Version = "2.8.0";
        private const string _build = "0";
        public const string VersionBuild = Version + "." + _build;
        public const string ManJiTag = "蛮吉";
        public const string ManXiaoManTag = "蛮小满";
        public const string Copyright = "Copyright ©  NTMiner";
        public const string LogsDirName = "Logs";

        public static readonly Version CurrentVersion;
        public static readonly string CurrentVersionStr;
        public static readonly string CurrentVersionTag = string.Empty;
        public static readonly string TempDirFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
        public static readonly string SwitchRadeonGpuResourceName = "switch-radeon-gpu.exe";
        public static readonly string SwitchRadeonGpuFileFullName = Path.Combine(TempDirFullName, SwitchRadeonGpuResourceName);
        public static readonly string AtikmdagPatcherResourceName = "atikmdag-patcher1.4.7.exe";
        public static readonly string AtikmdagPatcherFileFullName = Path.Combine(TempDirFullName, AtikmdagPatcherResourceName);
        public static readonly string BlockWAUResourceName = "BlockWAU.bat";
        public static readonly string BlockWAUFileFullName = Path.Combine(TempDirFullName, BlockWAUResourceName);

        static EntryAssemblyInfo() {
            if (!DevMode.IsInUnitTest) {
                Assembly mainAssembly = Assembly.GetEntryAssembly();
                CurrentVersion = mainAssembly.GetName().Version;
                var description = (AssemblyDescriptionAttribute)mainAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), inherit: false).FirstOrDefault();
                CurrentVersionTag = description?.Description;
            }
            else {
                CurrentVersion = new Version(2, 0, 0);
                CurrentVersionTag = "UnitTest";
            }
            CurrentVersionStr = CurrentVersion.ToString(4);
        }

        public static void ExtractManifestResource(this Assembly assembly, Type type, string name, string saveFileFuleName) {
            using (var stream = assembly.GetManifestResourceStream(type, name)) {
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                File.WriteAllBytes(saveFileFuleName, data);
            }
        }
    }
}
