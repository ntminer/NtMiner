using System;

namespace NTMiner.Windows {
    public static class BcdEdit {
        public static bool IgnoreAllFailures() {
            try {
                int exitcode = -1;
                Cmd.RunClose("bcdedit", "/set {current} bootstatuspolicy ignoreallfailures", ref exitcode);
                bool r = exitcode == 0;
                if (r) {
                    Global.DebugLine("bootstatuspolicy ignoreallfailures ok", ConsoleColor.Green);
                }
                else {
                    Global.DebugLine("bootstatuspolicy ignoreallfailures faild, exitcode=" + exitcode, ConsoleColor.Red);
                }
                return r;
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine("bootstatuspolicy ignoreallfailures failed，因为异常", e);
                return false;
            }
        }
    }
}
