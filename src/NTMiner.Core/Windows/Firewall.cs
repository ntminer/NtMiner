using System;

namespace NTMiner.Windows {
    public static class Firewall {
        public static bool DisableFirewall() {
            try {
                int exitcode = -1;
                Cmd.RunClose("netsh", "advfirewall set allprofiles state off", ref exitcode);
                bool r = exitcode == 0;
                if (r) {
                    Global.DebugLine("disable firewall ok", ConsoleColor.Green);
                }
                else {
                    Global.DebugLine("disable firewall failed, exitcode=" + exitcode, ConsoleColor.Red);
                }
                return r;
            }
            catch (Exception e) {
                Global.Logger.Error("disable firewall failed，因为异常", e);
                return false;
            }
        }
    }
}
