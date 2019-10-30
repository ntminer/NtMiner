using System;
using System.IO;

namespace NTMiner {
    public static class SpecialPath {
        private static readonly string _homeDirFullName;
        public static readonly string NTMinerLocalJsonFileFullName;
        public static readonly string NTMinerServerJsonFileFullName;
        public static readonly string GpuProfilesJsonFileFullName;

        static SpecialPath() {
            string location = NTMinerRegistry.GetLocation();
            if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                try {
                    if (File.Exists(Path.Combine(Path.GetDirectoryName(location), "home.lock"))) {
                        _homeDirFullName = Path.GetDirectoryName(location);
                    }
                    else {
                        _homeDirFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
                    }
                }
                catch {
                    _homeDirFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
                }
            }
            else {
                _homeDirFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NTMiner");
            }
            NTMinerLocalJsonFileFullName = Path.Combine(_homeDirFullName, NTKeyword.LocalJsonFileName);
            NTMinerServerJsonFileFullName = Path.Combine(_homeDirFullName, NTKeyword.ServerJsonFileName);
            GpuProfilesJsonFileFullName = Path.Combine(_homeDirFullName, NTKeyword.GpuProfilesFileName);
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
