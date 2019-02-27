using System.IO;

namespace NTMiner {
    public static class SpecialPath {
        static SpecialPath() {
            string daemonDirFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "Daemon");
            if (!Directory.Exists(daemonDirFullName)) {
                Directory.CreateDirectory(daemonDirFullName);
            }
            DaemonFileFullName = Path.Combine(daemonDirFullName, "NTMinerDaemon.exe");
            DevConsoleFileFullName = Path.Combine(daemonDirFullName, "DevConsole.exe");

            TempDirFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "Temp");
            if (!Directory.Exists(TempDirFullName)) {
                Directory.CreateDirectory(TempDirFullName);
            }
            NTMinerOverClockFileFullName = Path.Combine(TempDirFullName, "NTMinerOverClock.exe");
            ServerDbFileName = "server.litedb";
            ServerDbFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, ServerDbFileName);
            ServerJsonFileName = "server.json";
            ServerJsonFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, ServerJsonFileName);

            LocalDbFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "local.litedb");
        }

        public static string LocalDbFileFullName { get; private set; }
        public static string ServerDbFileName { get; private set; }
        public static string ServerDbFileFullName { get; private set; }

        public static string ServerJsonFileName { get; private set; }
        public static string ServerJsonFileFullName { get; private set; }

        public static string DaemonFileFullName { get; private set; }

        public static string DevConsoleFileFullName { get; private set; }

        public static string NTMinerOverClockFileFullName { get; private set; }

        public static string TempDirFullName { get; private set; }

        private static bool s_isFirstCallPackageDirFullName = true;
        public static string PackagesDirFullName {
            get {
                string dirFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "Packages");
                if (s_isFirstCallPackageDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    s_isFirstCallPackageDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool s_isFirstCallDownloadDirFullName = true;
        public static string DownloadDirFullName {
            get {
                string dirFullName = Path.Combine(TempDirFullName, "Download");
                if (s_isFirstCallDownloadDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    s_isFirstCallDownloadDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool s_isFirstCallKernelsDirFullName = true;
        public static string KernelsDirFullName {
            get {
                string dirFullName = Path.Combine(TempDirFullName, "Kernels");
                if (s_isFirstCallKernelsDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    s_isFirstCallKernelsDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool s_isFirstCallLogsDirFullName = true;
        public static string LogsDirFullName {
            get {
                string dirFullName = Path.Combine(TempDirFullName, "logs");
                if (s_isFirstCallLogsDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    s_isFirstCallLogsDirFullName = false;
                }

                return dirFullName;
            }
        }
        private static bool s_isFirstCallPicturesDirFullName = true;
        public static string PicturesDirFullName {
            get {
                string dirFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "Pictures");
                if (s_isFirstCallPicturesDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    s_isFirstCallPicturesDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool s_isFirstCallScreenshotsDirFullName = true;
        public static string ScreenshotsDirFullName {
            get {
                string dirFullName = Path.Combine(PicturesDirFullName, "Screenshots");
                if (s_isFirstCallScreenshotsDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    s_isFirstCallScreenshotsDirFullName = false;
                }

                return dirFullName;
            }
        }
    }
}
