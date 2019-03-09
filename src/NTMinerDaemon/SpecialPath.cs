using System;

namespace NTMiner {
    public static class SpecialPath {
        public static readonly string NTMinerLocalDbFileFullName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner", "local.litedb");
        public static readonly string NTMinerLocalJsonFileFullName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner", "local.json");
        public static readonly string NTMinerServerJsonFileFullName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner", "server.json");
    }
}
