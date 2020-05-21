using System.IO;

namespace NTMiner {
    public static class TestUtil {
        public static readonly string DataDirFullName = Path.Combine(Directory.GetParent(HomePath.AppDomainBaseDirectory).Parent.FullName, "data");
    }
}
