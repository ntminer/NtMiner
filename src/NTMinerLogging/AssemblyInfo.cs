using System;
using System.IO;
using System.Reflection;

namespace NTMiner {
    public static class AssemblyInfo {
        public static string GlobalDirFullName { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
        public const string Version = "2.3.1";
        public const string Build = "0";
        public const string Copyright = "Copyright ©  2019";
        public const string Tag = "蛮吉";
        public static readonly string ServerJsonFileName = $"server2.0.0.json";
        public static string ServerVersionJsonFileFullName = Path.Combine(GlobalDirFullName, ServerJsonFileName);
        public static string OfficialServerHost = "server.ntminer.com";
        public static readonly string MinerJsonBucket = "https://minerjson.oss-cn-beijing.aliyuncs.com/";

        static AssemblyInfo() {
            if (!Directory.Exists(GlobalDirFullName)) {
                Directory.CreateDirectory(GlobalDirFullName);
            }
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
