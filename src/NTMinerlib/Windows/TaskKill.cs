using System.Diagnostics;

namespace NTMiner.Windows {
    public static class TaskKill {
        public static void Kill(string processName) {
            if (string.IsNullOrEmpty(processName)) {
                return;
            }
            Cmd.RunClose($"taskkill /F /T /IM {processName}.exe", string.Empty);
        }

        public static void KillOtherProcess(Process process) {
            if (process == null) {
                return;
            }
            Cmd.RunClose($"taskkill /F /FI \"pid ne {process.Id}\" /T /IM {process.ProcessName}.exe", string.Empty);
        }
    }
}
