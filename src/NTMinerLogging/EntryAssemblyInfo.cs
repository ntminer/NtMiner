using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NTMiner {
    public static class EntryAssemblyInfo {
        public const string Version = "2.7.0";
        public const string Build = "0";
        public const string Tag = "蛮吉";
        public const string Copyright = "Copyright ©  NTMiner";

        public static readonly Version CurrentVersion;
        public static readonly string CurrentVersionTag = string.Empty;
        public static readonly string TempDirFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
        public static readonly string ServerJsonFileName;
        public static readonly string ServerVersionJsonFileFullName;

        public static string HomeDirFullName { get; private set; } = TempDirFullName;
        public static readonly string RootLockFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "home.lock");
        public static readonly string RootConfigFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "home.config");
        public static readonly bool IsLocalHome;

        public static void SetHomeDirFullName(string dirFullName) {
            HomeDirFullName = dirFullName;
        }

        public static string GetServerJsonVersion(Version version) {
            return $"server{CurrentVersion.Major.ToString()}.0.0.json";
        }

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
            ServerJsonFileName = GetServerJsonVersion(CurrentVersion);
            if (!File.Exists(RootLockFileFullName)) {
                if (File.Exists(RootConfigFileFullName)) {
                    HomeDirFullName = AppDomain.CurrentDomain.BaseDirectory;
                    IsLocalHome = true;
                }
                else if (!Directory.Exists(HomeDirFullName)) {
                    Directory.CreateDirectory(HomeDirFullName);
                }
            }
            else {
                HomeDirFullName = AppDomain.CurrentDomain.BaseDirectory;
                IsLocalHome = true;
            }
            if (IsLocalHome) {
                if (HomeDirFullName.EndsWith("\\")) {
                    HomeDirFullName = HomeDirFullName.Substring(0, HomeDirFullName.Length - 1);
                }
            }
            ServerVersionJsonFileFullName = Path.Combine(HomeDirFullName, ServerJsonFileName);
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
