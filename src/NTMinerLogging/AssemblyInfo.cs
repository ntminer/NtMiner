using System;
using System.IO;
using System.Reflection;

namespace NTMiner {
    public static class AssemblyInfo {
        public const string Version = "2.6.5";
        public const string Build = "0";
        public const string Tag = "蛮吉";
        public const string MinerJsonBucket = "https://minerjson.oss-cn-beijing.aliyuncs.com/";
        public const string Copyright = "Copyright ©  NTMiner";

        public static readonly string TempDirFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
        public static readonly string ServerJsonFileName;
        public static readonly string ServerVersionJsonFileFullName;

        public static string OfficialServerHost { get; private set; } = "server.ntminer.com";
        public static string LocalDirFullName { get; private set; } = TempDirFullName;
        public static readonly string RootLockFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "root.lock");
        public static readonly string RootConfigFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "root.config");
        public static readonly bool IsLocalDir;

        public static void SetOfficialServerHost(string host) {
            OfficialServerHost = host;
        }

        public static void SetLocalDirFullName(string dirFullName) {
            LocalDirFullName = dirFullName;
        }

        static AssemblyInfo() {
            if (!File.Exists(RootLockFileFullName)) {
                if (File.Exists(RootConfigFileFullName)) {
                    LocalDirFullName = AppDomain.CurrentDomain.BaseDirectory;
                    IsLocalDir = true;
                }
                else if (!Directory.Exists(LocalDirFullName)) {
                    Directory.CreateDirectory(LocalDirFullName);
                }
            }
            else {
                LocalDirFullName = AppDomain.CurrentDomain.BaseDirectory;
                IsLocalDir = true;
            }
            if (IsLocalDir) {
                if (LocalDirFullName.EndsWith("\\")) {
                    LocalDirFullName = LocalDirFullName.Substring(0, LocalDirFullName.Length - 1);
                }
            }
            if (!System.Version.TryParse(Version, out System.Version version)) {
                throw new InvalidDataException("版本号格式不正确");
            }
            ServerJsonFileName = $"server{version.Major}.0.0.json";
            ServerVersionJsonFileFullName = Path.Combine(LocalDirFullName, ServerJsonFileName);
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
