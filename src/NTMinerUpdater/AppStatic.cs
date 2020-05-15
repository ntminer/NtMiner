using System;
using System.IO;
using System.Windows;

namespace NTMiner {
    public static class AppStatic {
        public static bool IsMinerClient {
            get {
                return !IsMinerStudio;
            }
        }

        #region IsMinerStudio
        /// <summary>
        /// 表示是否是群控客户端。true表示是群控客户端，否则不是。
        /// </summary>
        public static bool IsMinerStudio {
            get {
                // 基于约定，根据主程序集中是否有给定名称的资源文件判断是否是群控客户端
                if (DevMode.IsInUnitTest) {
                    return false;
                }
                return Environment.CommandLine.IndexOf(NTKeyword.MinerStudioCmdParameterName, StringComparison.OrdinalIgnoreCase) != -1;
            }
        }
        #endregion

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

        public static bool IsDevMode {
            get {
                return WpfUtil.IsDevMode;
            }
        }

        public static bool IsNotDevMode => !WpfUtil.IsDevMode;

        public static Visibility IsDevModeVisible {
            get {
                if (DevMode.IsDevMode) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
    }
}
