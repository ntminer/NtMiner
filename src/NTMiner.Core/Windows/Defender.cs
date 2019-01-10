using System;

namespace NTMiner.Windows {
    public static class Defender {
        public static void DisableAntiSpyware() {
            try {
                const string subKey = @"SOFTWARE\Policies\Microsoft\Windows Defender";
                var v = Registry.GetValue(Microsoft.Win32.Registry.LocalMachine, subKey, "DisableAntiSpyware");
                if (v == null || v.ToString() != "1") {
                    Registry.SetValue(Microsoft.Win32.Registry.LocalMachine, subKey, "DisableAntiSpyware", 1);
                    Global.Logger.OkDebugLine("Windows Defender禁用成功");
                }
                else {
                    Global.Logger.WarnDebugLine("Windows Defender已经处于禁用状态，无需再次禁用");
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine("Windows Defender禁用失败，因为异常", e);
            }
        }
    }
}
