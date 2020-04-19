using System;
using System.IO;

namespace NTMiner {
    public static class HomePath {
        public const string ServerJsonFileName = "server.json";
        public const string LocalJsonFileName = "local.json";
        public const string GpuProfilesFileName = "gpuProfiles.json";
        public const string PackagesDirName = "Packages";
        public const string UpdaterDirName = "Updater";
        public const string NTMinerUpdaterFileName = "NTMinerUpdater.exe";
        public const string ServicesDirName = "Services";
        public const string ServerDbFileName = "server.litedb";
        public const string LocalDbFileName = "local.litedb";

        public static readonly string RootLockFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "home.lock");
        public static readonly string RootConfigFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "home.config");
        public static string HomeDirFullName { get; private set; } = EntryAssemblyInfo.TempDirFullName;
        public static readonly string ServerJsonFileFullName;
        public static readonly string LocalJsonFileFullName;
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
            LocalJsonFileFullName = Path.Combine(HomeDirFullName, LocalJsonFileName);
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

        public static string UpdaterFileFullName {
            get {
                return Path.Combine(UpdaterDirFullName, NTMinerUpdaterFileName);
            }
        }

        private static bool _sIsFirstCallServicesDirFullName = true;
        public static string ServicesDirFullName {
            get {
                string dirFullName = Path.Combine(HomeDirFullName, ServicesDirName);
                if (_sIsFirstCallServicesDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallServicesDirFullName = false;
                }

                return dirFullName;
            }
        }

        public static string ReadServerJsonFile() {
            if (File.Exists(ServerJsonFileFullName)) {
                return File.ReadAllText(ServerJsonFileFullName);
            }

            return string.Empty;
        }

        public static void WriteServerJsonFile(string json) {
            File.WriteAllText(ServerJsonFileFullName, json);
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

        public static void WriteGpuProfilesJsonFile(string json) {
            File.WriteAllText(GpuProfilesJsonFileFullName, json);
        }
    }
}
