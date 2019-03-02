using System;

namespace NTMiner {
    public static class SpecialPath {
        public static readonly string NTMinerLocalDbFileFullName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner", "local.litedb");
    }
}
