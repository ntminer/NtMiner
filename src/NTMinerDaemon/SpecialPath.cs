using System;
using System.IO;
using System.Text;

namespace NTMiner {
    public static class SpecialPath {
        private static readonly string _localJsonFileFullName;
        private static readonly string _serverJsonFileFullName;
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
            _localJsonFileFullName = Path.Combine(homeDirFullName, HomePath.LocalJsonFileName);
            _serverJsonFileFullName = Path.Combine(homeDirFullName, HomePath.ServerJsonFileName);
            _selfWorkLocalJsonFileFullName = Path.Combine(selfWorkDirFullName, HomePath.LocalJsonFileName);
            _selfWorkServerJsonFileFullName = Path.Combine(selfWorkDirFullName, HomePath.ServerJsonFileName);
            _gpuProfilesJsonFileFullName = Path.Combine(homeDirFullName, HomePath.GpuProfilesFileName);
        }

        public static string ReadLocalJsonFile() {
            if (File.Exists(_localJsonFileFullName)) {
                return File.ReadAllText(_localJsonFileFullName);
            }
            return string.Empty;
        }

        public static void WriteLocalJsonFile(string json) {
            if (json == null) {
                return;
            }
            File.WriteAllBytes(_localJsonFileFullName, Encoding.UTF8.GetBytes(json));
        }

        public static string ReadServerJsonFile() {
            if (File.Exists(_serverJsonFileFullName)) {
                return File.ReadAllText(_serverJsonFileFullName);
            }
            return string.Empty;
        }

        public static void WriteServerJsonFile(string json) {
            if (json == null) {
                return;
            }
            File.WriteAllBytes(_serverJsonFileFullName, Encoding.UTF8.GetBytes(json));
        }

        public static string ReadSelfWorkLocalJsonFile() {
            if (File.Exists(_selfWorkLocalJsonFileFullName)) {
                return File.ReadAllText(_selfWorkLocalJsonFileFullName);
            }
            else {
                return ReadLocalJsonFile();
            }
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
            else {
                return ReadServerJsonFile();
            }
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
