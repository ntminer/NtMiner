using System;
using System.IO;

namespace NTMiner {
    public static class SpecialPath {
        public static string GlobalDirFullName { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
        public static readonly string NTMinerLocalJsonFileFullName = Path.Combine(GlobalDirFullName, "local.json");
        public static readonly string NTMinerServerJsonFileFullName = Path.Combine(GlobalDirFullName, "server.json");
    }
}
