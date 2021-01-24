using System;
using System.IO;

namespace NTMiner {
    public static class TempPath {
        public static readonly string TempDirFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), NTKeyword.TempDirName);

        private static bool _sIsFirstCallTempLogsDirFullName = true;
        public static string TempLogsDirFullName {
            get {
                string dirFullName = Path.Combine(TempDirFullName, NTKeyword.LogsDirName);
                if (_sIsFirstCallTempLogsDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallTempLogsDirFullName = false;
                }

                return dirFullName;
            }
        }
    }
}
