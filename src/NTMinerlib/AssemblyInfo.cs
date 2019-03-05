using System.IO;

namespace NTMiner {
    public static class AssemblyInfo {
        public const string VERSION = "1.1.4";
        public const string COPYRIGHT = "Copyright ©  2019";
        public const string Tag = "谜麟";
        public static readonly string ServerJsonFileName = $"server{VERSION}.json";
        public static readonly string LangJsonFileName = $"lang{VERSION}.json";
        public static string ServerVersionJsonFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, ServerJsonFileName);
        public static readonly string LangVersionJsonFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, LangJsonFileName);

        public static readonly string ServerJsonFileUrl = "https://minerjson.oss-cn-beijing.aliyuncs.com/" + ServerJsonFileName;
        public static readonly string LangJsonFileUrl = "https://minerjson.oss-cn-beijing.aliyuncs.com/" + LangJsonFileName;
    }
}
