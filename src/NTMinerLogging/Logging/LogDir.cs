namespace NTMiner.Logging {
    public static class LogDir {
        private static string _logDir;
        public static string Dir {
            get { return _logDir; }
        }

        public static void SetDir(string fullPath) {
            _logDir = fullPath;
        }
    }
}
