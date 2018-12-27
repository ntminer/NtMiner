namespace NTMiner.Windows {
    public static class TaskKill {
        public static void Kill(string processName) {
            Cmd.RunClose($"taskkill /F /T /IM {processName}.exe", string.Empty);
        }

        public static void Kill(int processId) {
            Cmd.RunClose($"taskkill /F /T /PID {processId}", string.Empty);
        }
    }
}
