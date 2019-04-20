using System.Diagnostics;

namespace NTMiner.Windows {
    public static class TaskKill {
        public static void Kill(string processName, bool waitForExit = false) {
            if (string.IsNullOrEmpty(processName)) {
                return;
            }
            Cmd.RunClose($"taskkill /F /T /IM {processName}.exe", string.Empty, waitForExit);
        }

        public static void KillOtherProcess(Process process, bool waitForExit = false) {
            if (process == null) {
                return;
            }
            Cmd.RunClose($"taskkill /F /FI \"pid ne {process.Id}\" /T /IM {process.ProcessName}.exe", string.Empty, waitForExit);
        }
    }
}
