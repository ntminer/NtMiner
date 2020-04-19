using System;
using System.IO;

namespace NTMiner.Core.MinerStudio {
    public static class MinerStudioPath {
        public static string MineWorksDirFullName { get; private set; }

        static MinerStudioPath() {
            MineWorksDirFullName = Path.Combine(HomePath.HomeDirFullName, "MineWorks");
            if (!Directory.Exists(MineWorksDirFullName)) {
                Directory.CreateDirectory(MineWorksDirFullName);
            }
        }

        public static string GetMineWorkLocalJsonFileFullName(Guid workId) {
            return Path.Combine(MineWorksDirFullName, workId + ".local");
        }
        public static string GetMineWorkServerJsonFileFullName(Guid workId) {
            return Path.Combine(MineWorksDirFullName, workId + ".server");
        }

        public static void DeleteMineWorkFiles(Guid workId) {
            string[] fileFullNames = new string[] {
                GetMineWorkLocalJsonFileFullName(workId),
                GetMineWorkServerJsonFileFullName(workId)
            };
            foreach (var fileFullName in fileFullNames) {
                File.Delete(fileFullName);
            }
        }

        public static string ReadMineWorkLocalJsonFile(Guid workId) {
            string fileFullName = GetMineWorkLocalJsonFileFullName(workId);
            if (File.Exists(fileFullName)) {
                return File.ReadAllText(fileFullName);
            }

            return string.Empty;
        }

        public static string ReadMineWorkServerJsonFile(Guid workId) {
            string fileFullName = GetMineWorkServerJsonFileFullName(workId);
            if (File.Exists(fileFullName)) {
                return File.ReadAllText(fileFullName);
            }

            return string.Empty;
        }

    }
}
