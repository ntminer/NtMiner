using System.IO;

namespace NTMiner {
    public static partial class MinerClientTempPath {
        static MinerClientTempPath() {
            string daemonDirFullName = Path.Combine(TempPath.TempDirFullName, NTKeyword.DaemonDirName);
            string noDevFeeDirFullName = Path.Combine(TempPath.TempDirFullName, NTKeyword.NoDevFeeDirName);
            if (!Directory.Exists(daemonDirFullName)) {
                Directory.CreateDirectory(daemonDirFullName);
            }
            if (!Directory.Exists(noDevFeeDirFullName)) {
                Directory.CreateDirectory(noDevFeeDirFullName);
            }
            DaemonFileFullName = Path.Combine(daemonDirFullName, NTKeyword.NTMinerDaemonFileName);
            NoDevFeeFileFullName = Path.Combine(noDevFeeDirFullName, NTKeyword.NTMinerNoDevFeeFileName);
            DevConsoleFileFullName = Path.Combine(daemonDirFullName, NTKeyword.DevConsoleFileName);

            Upgrade();
        }

        public static string GetIconFileFullName(string coinIcon) {
            if (string.IsNullOrEmpty(coinIcon)) {
                return string.Empty;
            }
            string iconFileFullName = Path.Combine(CoinIconsDirFullName, coinIcon);
            return iconFileFullName;
        }

        public static readonly string DaemonFileFullName;
        public static readonly string NoDevFeeFileFullName;

        public static readonly string DevConsoleFileFullName;

        private static bool _sIsFirstCallCoinIconDirFullName = true;
        public static string CoinIconsDirFullName {
            get {
                string dirFullName = Path.Combine(TempPath.TempDirFullName, NTKeyword.CoinIconsDirName);
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
                string dirFullName = Path.Combine(TempPath.TempDirFullName, NTKeyword.DownloadDirName);
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
                string dirFullName = Path.Combine(TempPath.TempDirFullName, NTKeyword.KernelsDirName);
                if (_sIsFirstCallKernelsDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallKernelsDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallToolsDirFullName = true;
        /// <summary>
        /// 在临时目录。因为工具并非常用的程序文件，在用户第一次使用时才会下载。
        /// </summary>
        public static string ToolsDirFullName {
            get {
                string dirFullName = Path.Combine(TempPath.TempDirFullName, NTKeyword.ToolsDirName);
                if (_sIsFirstCallToolsDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallToolsDirFullName = false;
                }

                return dirFullName;
            }
        }

        public static string MinerClientFinderFileFullName {
            get {
                string dir = Path.Combine(ToolsDirFullName, "MinerClientFinder");
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }
                return Path.Combine(dir, NTKeyword.MinerClientFinderFileName);
            }
        }

        public static string AtikmdagPatcherFileFullName {
            get {
                string dir = Path.Combine(ToolsDirFullName, "AtikmdagPatcher");
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }
                return Path.Combine(dir, NTKeyword.AtikmdagPatcherFileName);
            }
        }

        public static string SwitchRadeonGpuFileFullName {
            get {
                string dir = Path.Combine(ToolsDirFullName, "SwitchRadeonGpu");
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }
                return Path.Combine(dir, NTKeyword.SwitchRadeonGpuFileName);
            }
        }
    }
}
