using System;

namespace NTMiner.Windows {
    public static class Firewall {
        public static bool DisableFirewall() {
            try {
                int exitcode = -1;
                Cmd.RunClose("netsh", "advfirewall set allprofiles state off", ref exitcode);
                bool r = exitcode == 0;
                if (r) {
                    Logger.OkDebugLine("disable firewall ok");
                }
                else {
                    Logger.WarnDebugLine("disable firewall failed, exitcode=" + exitcode);
                }
                return r;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine("disable firewall failed，因为异常", e);
                return false;
            }
        }
    }
}
