using System.IO;

namespace NTMiner {
    public static class SpecialPath {
        static SpecialPath() {
            string daemonDirFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "Daemon");
            if (!Directory.Exists(daemonDirFullName)) {
                Directory.CreateDirectory(daemonDirFullName);
            }
        }

        public static string TempDirFullName { get; private set; }


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
    }
}
