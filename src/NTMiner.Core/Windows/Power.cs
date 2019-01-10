using System;

namespace NTMiner.Windows {
    public static class Power {
        public static void Restart() {
            Cmd.RunClose("shutdown", "-r -t 0");
        }

        public static void Shutdown() {
            Cmd.RunClose("shutdown", "-s -t 0");
        }

        public static bool PowerCfgOff() {
            try {
                int exitcode = -1;
                Cmd.RunClose("powercfg", "-h off", ref exitcode);
                bool r = exitcode == 0;
                if (r) {
                    Global.Logger.OkDebugLine("powercfg -h off ok");
                }
                else {
                    Global.Logger.WarnDebugLine("powercfg -h off failed, exitcode=" + exitcode);
                }
                return r;
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine("powercfg -h off failed，因为异常", e);
                return false;
            }
        }
    }
}
