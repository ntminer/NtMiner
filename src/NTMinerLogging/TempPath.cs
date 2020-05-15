using System;
using System.IO;

namespace NTMiner {
    public static class TempPath {
        public static readonly string TempDirFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
    }
}
