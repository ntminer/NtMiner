using System.IO;

namespace NTMiner {
    public static class AssemblyInfo {
        public const string Version = "1.1.4";
        public const string Copyright = "Copyright ©  2019";
        public const string Tag = "谜麟";
        public static readonly string ServerJsonFileName = $"server{Version}.json";
        public static readonly string LangJsonFileName = $"lang{Version}.json";
        public static string ServerVersionJsonFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, ServerJsonFileName);
        public static readonly string LangVersionJsonFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, LangJsonFileName);
    }
}
