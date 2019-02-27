namespace NTMiner.Logging {
    public static class LogDir {
        private static string s_logDir;
        public static string Dir {
            get { return s_logDir; }
        }

        public static void SetDir(string fullPath) {
            s_logDir = fullPath;
        }
    }
}
