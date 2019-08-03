using System;
using System.Runtime.InteropServices;

namespace NTMiner.Windows {
    public static class Power {
        [DllImport("kernel32.dll")]
        internal static extern uint SetThreadExecutionState(ExecutionFlag flags);
        [Flags]
        internal enum ExecutionFlag : uint {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            Awaymode = 0x00000040,
            System = 0x00000001,
            Display = 0x00000002,
            Continus = 0x80000000,
        }

        public static void Restart(int delaySeconds = 0) {
            Cmd.RunClose("shutdown", "-r -f -t " + delaySeconds);
        }

        public static void Shutdown(int delaySeconds = 0) {
            Cmd.RunClose("shutdown", "-s -f -t " + delaySeconds);
        }

        /// <summary>
        /// 关闭系统休眠
        /// </summary>
        /// <returns></returns>
        public static bool PowerCfgOff() {
            try {
                int exitcode = -1;
                Cmd.RunClose("powercfg", "-h off", ref exitcode);
                bool r = exitcode == 0;
                if (r) {
                    Logger.OkDebugLine("powercfg -h off ok");
                }
                else {
                    Logger.WarnDebugLine("powercfg -h off failed, exitcode=" + exitcode);
                }
                return r;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine("powercfg -h off failed，因为异常", e);
                return false;
            }
        }
    }
}
