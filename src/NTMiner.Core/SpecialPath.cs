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
            ServerDbFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "server.litedb");
            ServerJsonFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "server.json");

            LocalDbFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "local.litedb");
            LocalJsonFileFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "local.json");
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

        public static void WriteLocalJsonFile(string json) {
            File.WriteAllText(LocalJsonFileFullName, json);
        }

        public static string LocalDbFileFullName { get; private set; }
        public static string LocalJsonFileFullName { get; private set; }
        public static string ServerDbFileFullName { get; private set; }

        public static string ServerJsonFileFullName { get; private set; }

        public static string DaemonFileFullName { get; private set; }

        public static string DevConsoleFileFullName { get; private set; }

        public static string NTMinerOverClockFileFullName { get; private set; }

        public static string TempDirFullName { get; private set; }

        private static bool _sIsFirstCallPackageDirFullName = true;
        public static string PackagesDirFullName {
            get {
                string dirFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "Packages");
                if (_sIsFirstCallPackageDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallPackageDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallDownloadDirFullName = true;
        public static string DownloadDirFullName {
            get {
                string dirFullName = Path.Combine(TempDirFullName, "Download");
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
                string dirFullName = Path.Combine(TempDirFullName, "Kernels");
                if (_sIsFirstCallKernelsDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallKernelsDirFullName = false;
                }

                return dirFullName;
            }
        }

        private static bool _sIsFirstCallLogsDirFullName = true;
        public static string LogsDirFullName {
            get {
                string dirFullName = Path.Combine(TempDirFullName, "logs");
                if (_sIsFirstCallLogsDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallLogsDirFullName = false;
                }

                return dirFullName;
            }
        }
    }
}
