using Microsoft.Win32;
using System;

namespace NTMiner.Windows {
    public static class Crash {
        public static void SetAutoReboot(bool autoReboot) {
            int value = autoReboot ? 1 : 0;
            try {
                const string subKey = @"SYSTEM\CurrentControlSet\Control\CrashControl";
                WinRegistry.SetValue(Registry.LocalMachine, subKey, "AutoReboot", value);
                Logger.OkDebugLine($"{(autoReboot ? "启用" : "禁用")}蓝屏自动重启成功");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine("蓝屏自动重启设置失败，因为异常", e);
            }
        }

        public static bool GetAutoReboot() {
            try {
                const string subKey = @"SYSTEM\CurrentControlSet\Control\CrashControl";
                var v = WinRegistry.GetValue(Registry.LocalMachine, subKey, "AutoReboot");
                return v != null && v.ToString() == "1";
            }
            catch (Exception e) {
                Logger.ErrorDebugLine("读取蓝屏自动重启设置失败，因为异常", e);
                return false;
            }
        }
    }
}
