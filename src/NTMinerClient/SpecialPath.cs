using NTMiner.Core;
using System.IO;

namespace NTMiner {
    public static class SpecialPath {
        static SpecialPath() {
            string daemonDirFullName = Path.Combine(EntryAssemblyInfo.TempDirFullName, NTKeyword.DaemonDirName);
            if (!Directory.Exists(daemonDirFullName)) {
                Directory.CreateDirectory(daemonDirFullName);
            }
            DaemonFileFullName = Path.Combine(daemonDirFullName, NTKeyword.NTMinerDaemonFileName);
            DevConsoleFileFullName = Path.Combine(daemonDirFullName, NTKeyword.DevConsoleFileName);

            ServerJsonFileFullName = Path.Combine(EntryAssemblyInfo.HomeDirFullName, NTKeyword.ServerJsonFileName);

            LocalJsonFileFullName = Path.Combine(EntryAssemblyInfo.HomeDirFullName, NTKeyword.LocalJsonFileName);
            GpuProfilesJsonFileFullName = Path.Combine(EntryAssemblyInfo.HomeDirFullName, NTKeyword.GpuProfilesFileName);           
            if (VirtualRoot.IsMinerClient && EntryAssemblyInfo.IsLocalHome && !File.Exists(EntryAssemblyInfo.RootLockFileFullName)) {
                #region 迁移
                string sharePackagesDir = Path.Combine(EntryAssemblyInfo.TempDirFullName, NTKeyword.PackagesDirName);
                if (Directory.Exists(sharePackagesDir)) {
                    foreach (var fileFullName in Directory.GetFiles(sharePackagesDir)) {
                        string destFileName = Path.Combine(PackagesDirFullName, Path.GetFileName(fileFullName));
                        if (!File.Exists(destFileName)) {
                            File.Copy(fileFullName, destFileName);
                        }
                    }
                }
                if (DevMode.IsDevMode) {
                    string shareServerDbFileFullName = Path.Combine(EntryAssemblyInfo.TempDirFullName, NTKeyword.ServerDbFileName);
                    if (File.Exists(shareServerDbFileFullName) && !File.Exists(EntryAssemblyInfo.ServerDbFileFullName)) {
                        File.Copy(shareServerDbFileFullName, EntryAssemblyInfo.ServerDbFileFullName);
                    }
                }
                string shareServerJsonFileFullName = Path.Combine(EntryAssemblyInfo.TempDirFullName, NTKeyword.ServerJsonFileName);
                if (File.Exists(shareServerJsonFileFullName) && !File.Exists(ServerJsonFileFullName)) {
                    File.Copy(shareServerJsonFileFullName, ServerJsonFileFullName);
                }
                string shareLocalDbFileFullName = Path.Combine(EntryAssemblyInfo.TempDirFullName, NTKeyword.LocalDbFileName);
                if (File.Exists(shareLocalDbFileFullName) && !File.Exists(EntryAssemblyInfo.LocalDbFileFullName)) {
                    File.Copy(shareLocalDbFileFullName, EntryAssemblyInfo.LocalDbFileFullName);
                }
                string shareLocalJsonFileFullName = Path.Combine(EntryAssemblyInfo.TempDirFullName, NTKeyword.LocalJsonFileName);
                if (File.Exists(shareLocalJsonFileFullName) && !File.Exists(LocalJsonFileFullName)) {
                    File.Copy(shareLocalJsonFileFullName, LocalJsonFileFullName);
                }
                string shareGpuProfilesJsonFileFullName = Path.Combine(EntryAssemblyInfo.TempDirFullName, NTKeyword.GpuProfilesFileName);
                if (File.Exists(shareGpuProfilesJsonFileFullName) && !File.Exists(GpuProfilesJsonFileFullName)) {
                    File.Copy(shareGpuProfilesJsonFileFullName, GpuProfilesJsonFileFullName);
                }
                string shareUpdaterFileFullName = Path.Combine(EntryAssemblyInfo.TempDirFullName, NTKeyword.UpdaterDirName, NTKeyword.NTMinerUpdaterFileName);
                if (File.Exists(shareUpdaterFileFullName) && !File.Exists(UpdaterFileFullName)) {
                    File.Copy(shareUpdaterFileFullName, UpdaterFileFullName);
                }
                #endregion
                File.Move(EntryAssemblyInfo.RootConfigFileFullName, EntryAssemblyInfo.RootLockFileFullName);
            }
        }

        public static string GetIconFileFullName(ICoin coin) {
            if (coin == null || string.IsNullOrEmpty(coin.Icon)) {
                return string.Empty;
            }
            string iconFileFullName = Path.Combine(CoinIconsDirFullName, coin.Icon);
            return iconFileFullName;
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

        public static readonly string LocalJsonFileFullName;
        public static readonly string GpuProfilesJsonFileFullName;

        public static readonly string ServerJsonFileFullName;

        public static readonly string DaemonFileFullName;

        public static readonly string DevConsoleFileFullName;

        private static bool _sIsFirstCallPackageDirFullName = true;
        public static string PackagesDirFullName {
            get {
                string dirFullName = Path.Combine(EntryAssemblyInfo.HomeDirFullName, NTKeyword.PackagesDirName);
                if (_sIsFirstCallPackageDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallPackageDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallCoinIconDirFullName = true;
        public static string CoinIconsDirFullName {
            get {
                string dirFullName = Path.Combine(EntryAssemblyInfo.TempDirFullName, NTKeyword.CoinIconsDirName);
                if (_sIsFirstCallCoinIconDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallCoinIconDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallDownloadDirFullName = true;
        public static string DownloadDirFullName {
            get {
                string dirFullName = Path.Combine(EntryAssemblyInfo.TempDirFullName, NTKeyword.DownloadDirName);
                if (_sIsFirstCallDownloadDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallDownloadDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallKernelsDirFullName = true;
        public static string KernelsDirFullName {
            get {
                string dirFullName = Path.Combine(EntryAssemblyInfo.TempDirFullName, NTKeyword.KernelsDirName);
                if (_sIsFirstCallKernelsDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallKernelsDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallTempLogsDirFullName = true;
        public static string TempLogsDirFullName {
            get {
                string dirFullName = Path.Combine(EntryAssemblyInfo.TempDirFullName, NTKeyword.LogsDirName);
                if (_sIsFirstCallTempLogsDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallTempLogsDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallHomeLogsDirFullName = true;
        public static string HomeLogsDirFullName {
            get {
                string dirFullName = Path.Combine(EntryAssemblyInfo.HomeDirFullName, NTKeyword.LogsDirName);
                if (_sIsFirstCallHomeLogsDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallHomeLogsDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallToolsDirFullName = true;
        public static string ToolsDirFullName {
            get {
                string dirFullName = Path.Combine(EntryAssemblyInfo.HomeDirFullName, NTKeyword.ToolsDirName);
                if (_sIsFirstCallToolsDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallToolsDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallUpdaterDirFullName = true;
        public static string UpdaterDirFullName {
            get {
                string dirFullName = Path.Combine(EntryAssemblyInfo.HomeDirFullName, NTKeyword.UpdaterDirName);
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
                return Path.Combine(UpdaterDirFullName, NTKeyword.NTMinerUpdaterFileName);
            }
        }

        public static string MinerClientFinderFileFullName {
            get {
                return Path.Combine(EntryAssemblyInfo.TempDirFullName, NTKeyword.MinerClientFinderFileName);
            }
        }

        private static bool _sIsFirstCallServicesDirFullName = true;
        public static string ServicesDirFullName {
            get {
                string dirFullName = Path.Combine(EntryAssemblyInfo.HomeDirFullName, NTKeyword.ServicesDirName);
                if (_sIsFirstCallServicesDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallServicesDirFullName = false;
                }

                return dirFullName;
            }
        }

        public static string ServicesFileFullName {
            get {
                return Path.Combine(ServicesDirFullName, NTKeyword.NTMinerServicesFileName);
            }
        }
    }
}
