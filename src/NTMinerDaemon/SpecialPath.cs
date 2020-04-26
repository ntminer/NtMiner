using System;
using System.IO;

namespace NTMiner {
    public static class SpecialPath {
        private static readonly string _homeDirFullName;
        public static readonly string LocalJsonFileFullName;
        public static readonly string ServerJsonFileFullName;
        public static readonly string GpuProfilesJsonFileFullName;

        static SpecialPath() {
            string location = NTMinerRegistry.GetLocation(NTMinerAppType.MinerClient);
            string globalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
            if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                try {
                    if (File.Exists(Path.Combine(Path.GetDirectoryName(location), "home.lock"))) {
                        _homeDirFullName = Path.GetDirectoryName(location);
                    }
                    else {
                        _homeDirFullName = globalPath;
                    }
                }
                catch {
                    _homeDirFullName = globalPath;
                }
            }
            else {
                _homeDirFullName = globalPath;
            }
            LocalJsonFileFullName = Path.Combine(_homeDirFullName, HomePath.LocalJsonFileName);
            ServerJsonFileFullName = Path.Combine(_homeDirFullName, HomePath.ServerJsonFileName);
            GpuProfilesJsonFileFullName = Path.Combine(_homeDirFullName, HomePath.GpuProfilesFileName);
        }

        public static string ReadLocalJsonFile() {
            if (File.Exists(LocalJsonFileFullName)) {
                return File.ReadAllText(LocalJsonFileFullName);
            }

            return string.Empty;
        }

        public static string ReadGpuProfilesJsonFile() {
            if (File.Exists(GpuProfilesJsonFileFullName)) {
                return File.ReadAllText(GpuProfilesJsonFileFullName);
            }

            return string.Empty;
        }

        public static void SaveGpuProfilesJsonFile(string json) {
            File.WriteAllText(GpuProfilesJsonFileFullName, json);
        }
    }
}
