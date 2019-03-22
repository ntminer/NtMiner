namespace NTMiner.Windows {
    public static class TaskKill {
        public static void Kill(string processName) {
            if (string.IsNullOrEmpty(processName)) {
                return;
            }
            Cmd.RunClose($"taskkill /F /T /IM {processName}.exe", string.Empty);
        }

        public static void Kill(int processId) {
            if (processId <= 0) {
                return;
            }
            Cmd.RunClose($"taskkill /F /T /PID {processId}", string.Empty);
        }
    }
}
