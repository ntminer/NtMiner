using System;
using System.IO;

namespace NTMiner {
    public static class HomePath {
        public const string ServerJsonFileName = "server.json";
        public const string LocalJsonFileName = "local.json";
        public const string GpuProfilesFileName = "gpuProfiles.json";
        public const string PackagesDirName = "Packages";
        public const string UpdaterDirName = "Updater";
        public const string SelfWorkDirName = "SelfWork";
        public const string MineWorkDirName = "MineWork";
        public const string NTMinerUpdaterFileName = "NTMinerUpdater.exe";
        public const string ServerDbFileName = "server.litedb";
        public const string LocalDbFileName = "local.litedb";

        public static readonly string RootLockFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "home.lock");
        public static readonly string RootConfigFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "home.config");
        public static string HomeDirFullName { get; private set; } = EntryAssemblyInfo.TempDirFullName;
        public static readonly string ServerJsonFileFullName;
        public static readonly string SelfWorkServerJsonFileFullName;
        public static readonly string MineWorkServerJsonFileFullName;
        public static readonly string MineWorkLocalJsonFileFullName;
        public static readonly string SelfWorkLocalJsonFileFullName;
        public static readonly string GpuProfilesJsonFileFullName;
        static HomePath() {
            string homeDirFullName = HomeDirFullName;
            if (!File.Exists(RootLockFileFullName)) {
                if (File.Exists(RootConfigFileFullName)) {
                    homeDirFullName = AppDomain.CurrentDomain.BaseDirectory;
                }
            }
            else {
                homeDirFullName = AppDomain.CurrentDomain.BaseDirectory;
            }
            SetHomeDirFullName(homeDirFullName);
            ServerJsonFileFullName = Path.Combine(HomeDirFullName, ServerJsonFileName);
            SelfWorkServerJsonFileFullName = Path.Combine(SelfWorkDirFullName, ServerJsonFileName);
            MineWorkServerJsonFileFullName = Path.Combine(MineWorkDirFullName, ServerJsonFileName);
            SelfWorkLocalJsonFileFullName = Path.Combine(SelfWorkDirFullName, LocalJsonFileName);
            MineWorkLocalJsonFileFullName = Path.Combine(MineWorkDirFullName, LocalJsonFileName);
            GpuProfilesJsonFileFullName = Path.Combine(HomeDirFullName, GpuProfilesFileName);
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
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                if (HomeDirFullName.Length + 1 != baseDir.Length) {
                    return false;
                }
                return HomeDirFullName + "\\" == baseDir;
            }
        }

        private static string _localDbFileFullName = Path.Combine(HomeDirFullName, LocalDbFileName);
        public static string LocalDbFileFullName {
            get {
                return _localDbFileFullName;
            }
        }

        private static string _serverDbFileFullName = Path.Combine(HomeDirFullName, ServerDbFileName);
        public static string ServerDbFileFullName {
            get {
                return _serverDbFileFullName;
            }
        }

        public static void SetHomeDirFullName(string dirFullName) {
            if (dirFullName.EndsWith("\\")) {
                dirFullName = dirFullName.Substring(0, dirFullName.Length - 1);
            }
            HomeDirFullName = dirFullName;
            if (!Directory.Exists(dirFullName)) {
                Directory.CreateDirectory(dirFullName);
            }
            _localDbFileFullName = Path.Combine(HomeDirFullName, LocalDbFileName);
            _serverDbFileFullName = Path.Combine(HomeDirFullName, ServerDbFileName);
        }
        
        private static bool _sIsFirstCallPackageDirFullName = true;
        public static string PackagesDirFullName {
            get {
                string dirFullName = Path.Combine(HomeDirFullName, PackagesDirName);
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
                string dirFullName = Path.Combine(HomeDirFullName, EntryAssemblyInfo.LogsDirName);
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
                string dirFullName = Path.Combine(HomeDirFullName, UpdaterDirName);
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
                string dirFullName = Path.Combine(HomeDirFullName, SelfWorkDirName);
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
                string dirFullName = Path.Combine(HomeDirFullName, MineWorkDirName);
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
                return Path.Combine(UpdaterDirFullName, NTMinerUpdaterFileName);
            }
        }

        public static string ReadServerJsonFile() {
            if (File.Exists(ServerJsonFileFullName)) {
                return File.ReadAllText(ServerJsonFileFullName);
            }

            return string.Empty;
        }

        public static string ReadSelfWorkServerJsonFile() {
            if (File.Exists(SelfWorkServerJsonFileFullName)) {
                return File.ReadAllText(SelfWorkServerJsonFileFullName);
            }
            else {
                return ReadServerJsonFile();
            }
        }

        public static string ReadMineWorkServerJsonFile() {
            if (File.Exists(MineWorkServerJsonFileFullName)) {
                return File.ReadAllText(MineWorkServerJsonFileFullName);
            }
            else {
                return ReadServerJsonFile();
            }
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

        public static string ReadMineWorkLocalJsonFile() {
            if (File.Exists(MineWorkLocalJsonFileFullName)) {
                return File.ReadAllText(MineWorkLocalJsonFileFullName);
            }

            return string.Empty;
        }

        public static string ReadSelfWorkLocalJsonFile() {
            if (File.Exists(SelfWorkLocalJsonFileFullName)) {
                return File.ReadAllText(SelfWorkLocalJsonFileFullName);
            }
            else {
                return ReadMineWorkLocalJsonFile();
            }
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
