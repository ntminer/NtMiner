using System.IO;

namespace NTMiner {
    public static partial class SpecialPath {
        public static string LocalDbFileFullName { get; private set; } = Path.Combine(HomePath.AppDomainBaseDirectory, NTKeyword.LocalDbFileName);
    }
}
