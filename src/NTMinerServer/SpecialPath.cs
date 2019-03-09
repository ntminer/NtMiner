using System;
using System.IO;

namespace NTMiner {
    public static class SpecialPath {
        static SpecialPath() {
            MineWorksDirFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MineWorks");
            if (!Directory.Exists(MineWorksDirFullName)) {
                Directory.CreateDirectory(MineWorksDirFullName);
            }
            LocalDbFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "local.litedb");
        }

        public static string GetMineWorkDbFileFullName(Guid workId) {
            return Path.Combine(MineWorksDirFullName, workId + ".litedb");
        }
        public static string GetMineWorkLocalJsonFileFullName(Guid workId) {
            return Path.Combine(MineWorksDirFullName, workId + ".local");
        }
        public static string GetMineWorkServerJsonFileFullName(Guid workId) {
            return Path.Combine(MineWorksDirFullName, workId + ".server");
        }
        public static string MineWorksDirFullName { get; private set; }
        public static string LocalDbFileFullName { get; private set; }
    }
}
