using System.IO;

namespace NTMiner {
    public static class TestUtil {
        public static readonly string RootDirFullName = Directory.GetParent(HomePath.AppDomainBaseDirectory).Parent.FullName;
        public static readonly string DataDirFullName = Path.Combine(RootDirFullName, "data");
        public static readonly string SpeedDataJsonFileFullName = Path.Combine(DataDirFullName, "speedData.json");
    }
}
