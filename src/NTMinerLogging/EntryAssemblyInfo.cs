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
        public static string HomeDirFullName { get; private set; } = TempDirFullName;
        public static readonly string RootLockFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "home.lock");
        public static readonly string RootConfigFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "home.config");

        public static string ServerJsonFileName {
            get {
                return GetServerJsonVersion(CurrentVersion);
            }
        }
        public static string ServerVersionJsonFileFullName {
            get {
                return Path.Combine(HomeDirFullName, ServerJsonFileName);
            }
        }
        public static bool IsLocalHome {
            get {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                if (HomeDirFullName.Length + 1 != baseDir.Length) {
                    return false;
                }
                return HomeDirFullName + "\\" == baseDir;
            }
        }

        public static void SetHomeDirFullName(string dirFullName) {
            if (dirFullName.EndsWith("\\")) {
                dirFullName = dirFullName.Substring(0, dirFullName.Length - 1);
            }
            HomeDirFullName = dirFullName;
            if (!Directory.Exists(dirFullName)) {
                Directory.CreateDirectory(dirFullName);
            }
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
            string homeDirFullName = HomeDirFullName;
            if (!File.Exists(RootLockFileFullName)) {
                if (File.Exists(RootConfigFileFullName)) {
                    homeDirFullName = AppDomain.CurrentDomain.BaseDirectory;
                }
            }
            else {
                homeDirFullName = AppDomain.CurrentDomain.BaseDirectory;
            }
            SetHomeDirFullName(homeDirFullName);
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
