using System;

namespace NTMiner.Windows {
    public static class BcdEdit {
        public static bool IgnoreAllFailures() {
            try {
                int exitcode = -1;
                Cmd.RunClose("bcdedit", "/set {current} bootstatuspolicy ignoreallfailures", ref exitcode);
                bool r = exitcode == 0;
                if (r) {
                    Global.Logger.OkDebugLine("bootstatuspolicy ignoreallfailures ok");
                }
                else {
                    Global.Logger.WarnDebugLine("bootstatuspolicy ignoreallfailures faild, exitcode=" + exitcode);
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
