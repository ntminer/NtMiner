using System;
using System.IO;
using System.Text;

namespace NTMiner {
    public static class SpecialPath {
        private static readonly string _mineWorkLocalJsonFileFullName;
        private static readonly string _mineWorkServerJsonFileFullName;
        private static readonly string _selfWorkLocalJsonFileFullName;
        private static readonly string _selfWorkServerJsonFileFullName;
        private static readonly string _gpuProfilesJsonFileFullName;

        static SpecialPath() {
            string location = NTMinerRegistry.GetLocation(NTMinerAppType.MinerClient);
            string globalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
            string homeDirFullName;
            if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                try {
                    if (File.Exists(Path.Combine(Path.GetDirectoryName(location), "home.lock"))) {
                        homeDirFullName = Path.GetDirectoryName(location);
                    }
                    else {
                        homeDirFullName = globalPath;
                    }
                }
                catch {
                    homeDirFullName = globalPath;
                }
            }
            else {
                homeDirFullName = globalPath;
            }
            string selfWorkDirFullName = Path.Combine(homeDirFullName, "SelfWork");
            if (!Directory.Exists(selfWorkDirFullName)) {
                Directory.CreateDirectory(selfWorkDirFullName);
            }
            string mineWorkDirFullName = Path.Combine(homeDirFullName, "MineWork");
            if (!Directory.Exists(mineWorkDirFullName)) {
                Directory.CreateDirectory(mineWorkDirFullName);
            }
            _mineWorkServerJsonFileFullName = Path.Combine(mineWorkDirFullName, NTKeyword.ServerJsonFileName);
            _mineWorkLocalJsonFileFullName = Path.Combine(mineWorkDirFullName, NTKeyword.LocalJsonFileName);
            _selfWorkServerJsonFileFullName = Path.Combine(selfWorkDirFullName, NTKeyword.ServerJsonFileName);
            _selfWorkLocalJsonFileFullName = Path.Combine(selfWorkDirFullName, NTKeyword.LocalJsonFileName);
            _gpuProfilesJsonFileFullName = Path.Combine(homeDirFullName, NTKeyword.GpuProfilesFileName);
        }

        public static string ReadMineWorkLocalJsonFile() {
            if (File.Exists(_mineWorkLocalJsonFileFullName)) {
                return File.ReadAllText(_mineWorkLocalJsonFileFullName);
            }
            return string.Empty;
        }

        public static void WriteMineWorkLocalJsonFile(string json) {
            if (json == null) {
                return;
            }
            File.WriteAllBytes(_mineWorkLocalJsonFileFullName, Encoding.UTF8.GetBytes(json));
        }

        public static void WriteMineWorkServerJsonFile(string json) {
            if (json == null) {
                return;
            }
            File.WriteAllBytes(_mineWorkServerJsonFileFullName, Encoding.UTF8.GetBytes(json));
        }

        public static string ReadSelfWorkLocalJsonFile() {
            if (File.Exists(_selfWorkLocalJsonFileFullName)) {
                return File.ReadAllText(_selfWorkLocalJsonFileFullName);
            }
            return string.Empty;
        }

        public static void WriteSelfWorkLocalJsonFile(string json) {
            if (json == null) {
                return;
            }
            File.WriteAllBytes(_selfWorkLocalJsonFileFullName, Encoding.UTF8.GetBytes(json));
        }

        public static string ReadSelfWorkServerJsonFile() {
            if (File.Exists(_selfWorkServerJsonFileFullName)) {
                return File.ReadAllText(_selfWorkServerJsonFileFullName);
            }
            return string.Empty;
        }

        public static void WriteSelfWorkServerJsonFile(string json) {
            if (json == null) {
                return;
            }
            File.WriteAllBytes(_selfWorkServerJsonFileFullName, Encoding.UTF8.GetBytes(json));
        }

        public static string ReadGpuProfilesJsonFile() {
            if (File.Exists(_gpuProfilesJsonFileFullName)) {
                return File.ReadAllText(_gpuProfilesJsonFileFullName);
            }

            return string.Empty;
        }

        public static void SaveGpuProfilesJsonFile(string json) {
            File.WriteAllText(_gpuProfilesJsonFileFullName, json);
        }
    }
}
