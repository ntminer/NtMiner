using System.IO;

namespace NTMiner {
    public static class AssemblyInfo {
        public const string Version = "2.0.0";
        public const string Build = "49001";
        public const string Copyright = "Copyright ©  2019";
        public const string Tag = "蛮吉";
        public static readonly string ServerJsonFileName = $"server{Version}.json";
        public static string ServerVersionJsonFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, ServerJsonFileName);
        public static string OfficialServerHost = "server.ntminer.com";
        public static readonly string MinerJsonBucket = "https://minerjson.oss-cn-beijing.aliyuncs.com/";
    }
}
