using System;
using System.IO;
using System.Reflection;

namespace NTMiner {
    public static class AssemblyInfo {
        public const string Version = "2.6.4";
        public const string Build = "0";
        public const string Tag = "蛮吉";
        public const string MinerJsonBucket = "https://minerjson.oss-cn-beijing.aliyuncs.com/";
        public const string Copyright = "Copyright ©  NTMiner";

        public static readonly string ShareDirFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
        public static readonly string ServerJsonFileName;
        public static readonly string ServerVersionJsonFileFullName;

        public static string OfficialServerHost { get; private set; } = "server.ntminer.com";
        public static string LocalDirFullName { get; private set; } = ShareDirFullName;

        public static void SetOfficialServerHost(string host) {
            OfficialServerHost = host;
        }

        public static void SetLocalDirFullName(string dirFullName) {
            LocalDirFullName = dirFullName;
        }

        static AssemblyInfo() {
            if (!Directory.Exists(LocalDirFullName)) {
                Directory.CreateDirectory(LocalDirFullName);
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
