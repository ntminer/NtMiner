using System.Diagnostics;

namespace NTMiner.Windows {
    public static class TaskKill {
        /// <summary>
        /// 不会抛出异常，因为吞掉了异常
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="waitForExit"></param>
        public static void Kill(string processName, bool waitForExit = false) {
            try {
                if (string.IsNullOrEmpty(processName)) {
                    return;
                }
                string args = $"/F /T /IM {processName}.exe";
                Cmd.RunClose("taskkill", args, waitForExit);
                Write.DevDebug(args);
            }
            catch {
            }
        }

        /// <summary>
        /// 不会抛出异常，因为吞掉了异常
        /// </summary>
        /// <param name="process"></param>
        /// <param name="waitForExit"></param>
        public static void KillOtherProcess(Process process, bool waitForExit = false) {
            if (process == null) {
                return;
            }
            try {
                string args = $"/F /FI \"pid ne {process.Id}\" /T /IM {process.ProcessName}.exe";
                Cmd.RunClose("taskkill", args, waitForExit);
                Write.DevDebug(args);
            }
            catch {
            }
        }
    }
}
