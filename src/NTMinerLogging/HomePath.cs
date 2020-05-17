using System;
using System.IO;

namespace NTMiner {
    public static class HomePath {
        public static string BaseDirectory { get; private set; } = AppDomain.CurrentDomain.BaseDirectory;
        public static string RootLockFileFullName { get; private set; } = Path.Combine(BaseDirectory, NTKeyword.RootLockFileName);
        public static string RootConfigFileFullName { get; private set; } = Path.Combine(BaseDirectory, NTKeyword.RootConfigFileName);
        public static string HomeDirFullName { get; private set; } = TempPath.TempDirFullName;
        public static string ServerJsonFileFullName { get; private set; }
        public static string SelfWorkServerJsonFileFullName { get; private set; }
        public static string MineWorkServerJsonFileFullName { get; private set; }
        public static string MineWorkLocalJsonFileFullName { get; private set; }
        public static string SelfWorkLocalJsonFileFullName { get; private set; }
        public static string GpuProfilesJsonFileFullName { get; private set; }

        static HomePath() {
            string homeDirFullName = HomeDirFullName;
            if (!File.Exists(RootLockFileFullName)) {
                if (File.Exists(RootConfigFileFullName)) {
                    homeDirFullName = BaseDirectory;
                }
            }
            else {
                homeDirFullName = BaseDirectory;
            }
            SetHomeDirFullName(homeDirFullName);
            ServerJsonFileFullName = Path.Combine(HomeDirFullName, NTKeyword.ServerJsonFileName);
            SelfWorkServerJsonFileFullName = Path.Combine(SelfWorkDirFullName, NTKeyword.ServerJsonFileName);
            MineWorkServerJsonFileFullName = Path.Combine(MineWorkDirFullName, NTKeyword.ServerJsonFileName);
            SelfWorkLocalJsonFileFullName = Path.Combine(SelfWorkDirFullName, NTKeyword.LocalJsonFileName);
            MineWorkLocalJsonFileFullName = Path.Combine(MineWorkDirFullName, NTKeyword.LocalJsonFileName);
            GpuProfilesJsonFileFullName = Path.Combine(HomeDirFullName, NTKeyword.GpuProfilesFileName);
        }

        public static string ExportServerJsonFileName {
            get {
                return GetServerJsonVersion(EntryAssemblyInfo.CurrentVersion);
            }
        }

        public static string GetServerJsonVersion(Version version) {
            return $"server{version.Major.ToString()}.0.0.json";
        }

        public static string ServerVersionJsonFileFullName {
            get {
                return Path.Combine(HomeDirFullName, ExportServerJsonFileName);
            }
        }
        public static bool IsLocalHome {
            get {
                string baseDir = BaseDirectory;
                if (HomeDirFullName.Length + 1 != baseDir.Length) {
                    return false;
                }
                return HomeDirFullName + "\\" == baseDir;
            }
        }

        public static string LocalDbFileFullName { get; private set; } = Path.Combine(HomeDirFullName, NTKeyword.LocalDbFileName);
        public static string ServerDbFileFullName { get; private set; } = Path.Combine(HomeDirFullName, NTKeyword.ServerDbFileName);

        public static void SetHomeDirFullName(string dirFullName) {
            if (dirFullName.EndsWith("\\")) {
                dirFullName = dirFullName.Substring(0, dirFullName.Length - 1);
            }
            HomeDirFullName = dirFullName;
            if (!Directory.Exists(dirFullName)) {
                Directory.CreateDirectory(dirFullName);
            }
            LocalDbFileFullName = Path.Combine(HomeDirFullName, NTKeyword.LocalDbFileName);
            ServerDbFileFullName = Path.Combine(HomeDirFullName, NTKeyword.ServerDbFileName);
        }
        
        private static bool _sIsFirstCallPackageDirFullName = true;
        public static string PackagesDirFullName {
            get {
                string dirFullName = Path.Combine(HomeDirFullName, NTKeyword.PackagesDirName);
                if (_sIsFirstCallPackageDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallPackageDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallHomeLogsDirFullName = true;
        public static string HomeLogsDirFullName {
            get {
                string dirFullName = Path.Combine(HomeDirFullName, NTKeyword.LogsDirName);
                if (_sIsFirstCallHomeLogsDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallHomeLogsDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallUpdaterDirFullName = true;
        /// <summary>
        /// 在家目录
        /// </summary>
        public static string UpdaterDirFullName {
            get {
                string dirFullName = Path.Combine(HomeDirFullName, NTKeyword.UpdaterDirName);
                if (_sIsFirstCallUpdaterDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallUpdaterDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallSelfWorkDirFullName = true;
        public static string SelfWorkDirFullName {
            get {
                string dirFullName = Path.Combine(HomeDirFullName, NTKeyword.SelfWorkDirName);
                if (_sIsFirstCallSelfWorkDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallSelfWorkDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallMineWorkDirFullName = true;
        public static string MineWorkDirFullName {
            get {
                string dirFullName = Path.Combine(HomeDirFullName, NTKeyword.MineWorkDirName);
                if (_sIsFirstCallMineWorkDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallMineWorkDirFullName = false;
                }

                return dirFullName;
            }
        }

        public static string UpdaterFileFullName {
            get {
                return Path.Combine(UpdaterDirFullName, NTKeyword.NTMinerUpdaterFileName);
            }
        }

        public static string ReadServerJsonFile(WorkType workType) {
            switch (workType) {
                case WorkType.SelfWork:
                    return ReadSelfWorkServerJsonFile();
                case WorkType.MineWork:
                    return ReadMineWorkServerJsonFile();
                default:
                    if (File.Exists(ServerJsonFileFullName)) {
                        return File.ReadAllText(ServerJsonFileFullName);
                    }
                    break;
            }

            return string.Empty;
        }

        private static string ReadSelfWorkServerJsonFile() {
            if (File.Exists(SelfWorkServerJsonFileFullName)) {
                return File.ReadAllText(SelfWorkServerJsonFileFullName);
            }
            else if (File.Exists(ServerJsonFileFullName)) {
                return File.ReadAllText(ServerJsonFileFullName);
            }
            return string.Empty;
        }

        private static string ReadMineWorkServerJsonFile() {
            if (File.Exists(MineWorkServerJsonFileFullName)) {
                return File.ReadAllText(MineWorkServerJsonFileFullName);
            }
            else if (File.Exists(ServerJsonFileFullName)) {
                return File.ReadAllText(ServerJsonFileFullName);
            }
            return string.Empty;
        }

        public static void WriteServerJsonFile(string json) {
            File.WriteAllText(ServerJsonFileFullName, json);
        }

        public static void WriteSelfWorkServerJsonFile(string json) {
            File.WriteAllText(SelfWorkServerJsonFileFullName, json);
        }

        public static void WriteMineWorkServerJsonFile(string json) {
            File.WriteAllText(MineWorkServerJsonFileFullName, json);
        }

        public static string ReadLocalJsonFile(WorkType workType) {
            switch (workType) {
                case WorkType.SelfWork:
                    return ReadSelfWorkLocalJsonFile();
                case WorkType.MineWork:
                    return ReadMineWorkLocalJsonFile();
                default:
                    return string.Empty;
            }
        }

        private static string ReadMineWorkLocalJsonFile() {
            if (File.Exists(MineWorkLocalJsonFileFullName)) {
                return File.ReadAllText(MineWorkLocalJsonFileFullName);
            }

            return string.Empty;
        }

        private static string ReadSelfWorkLocalJsonFile() {
            if (File.Exists(SelfWorkLocalJsonFileFullName)) {
                return File.ReadAllText(SelfWorkLocalJsonFileFullName);
            }

            return string.Empty;
        }

        public static string ReadGpuProfilesJsonFile() {
            if (File.Exists(GpuProfilesJsonFileFullName)) {
                return File.ReadAllText(GpuProfilesJsonFileFullName);
            }

            return string.Empty;
        }

        public static void WriteGpuProfilesJsonFile(string json) {
            File.WriteAllText(GpuProfilesJsonFileFullName, json);
        }
    }
}
