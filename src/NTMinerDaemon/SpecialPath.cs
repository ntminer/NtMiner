using System;
using System.IO;

namespace NTMiner {
    public static class SpecialPath {
        public static readonly string LocalJsonFileFullName;
        public static readonly string ServerJsonFileFullName;
        public static readonly string SelfWorkLocalJsonFileFullName;
        public static readonly string SelfWorkServerJsonFileFullName;
        public static readonly string GpuProfilesJsonFileFullName;

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
            LocalJsonFileFullName = Path.Combine(homeDirFullName, HomePath.LocalJsonFileName);
            ServerJsonFileFullName = Path.Combine(homeDirFullName, HomePath.ServerJsonFileName);
            SelfWorkLocalJsonFileFullName = Path.Combine(selfWorkDirFullName, HomePath.LocalJsonFileName);
            SelfWorkServerJsonFileFullName = Path.Combine(selfWorkDirFullName, HomePath.ServerJsonFileName);
            GpuProfilesJsonFileFullName = Path.Combine(homeDirFullName, HomePath.GpuProfilesFileName);
        }

        public static string ReadSelfWorkLocalJsonFile() {
            if (File.Exists(SelfWorkLocalJsonFileFullName)) {
                return File.ReadAllText(SelfWorkLocalJsonFileFullName);
            }
            else if (File.Exists(LocalJsonFileFullName)) {
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
