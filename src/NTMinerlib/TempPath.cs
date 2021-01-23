using System;
using System.IO;

namespace NTMiner {
    public static class TempPath {
        public static readonly string TempDirFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), NTKeyword.TempDirName);

        private static bool _sIsFirstCallLogsDirFullName = true;
        public static string LogsDirFullName {
            get {
                string dirFullName = Path.Combine(TempDirFullName, NTKeyword.LogsDirName);
                if (_sIsFirstCallLogsDirFullName) {
                    if (!Directory.Exists(dirFullName)) {
                        Directory.CreateDirectory(dirFullName);
                    }
                    _sIsFirstCallLogsDirFullName = false;
                }

                return dirFullName;
            }
        }

        public static string WebSocketSharpMinerClientLogFileFullName {
            get {
                return Path.Combine(LogsDirFullName, NTKeyword.WebSocketSharpMinerClientLogFileName);
            }
        }

        public static string WebSocketSharpMinerStudioLogFileFullName {
            get {
                return Path.Combine(LogsDirFullName, NTKeyword.WebSocketSharpMinerStudioLogFileName);
            }
        }
    }
}
