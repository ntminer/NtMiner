using System;

namespace NTMiner.Windows {
    public static class Defender {
        public static void DisableAntiSpyware() {
            try {
                const string subKey = @"SOFTWARE\Policies\Microsoft\Windows Defender";
                var v = Registry.GetValue(Microsoft.Win32.Registry.LocalMachine, subKey, "DisableAntiSpyware");
                if (v == null || v.ToString() != "1") {
                    Registry.SetValue(Microsoft.Win32.Registry.LocalMachine, subKey, "DisableAntiSpyware", 1);
                    Global.DebugLine("Windows Defender禁用成功", ConsoleColor.Green);
                }
                else {
                    Global.DebugLine("Windows Defender已经处于禁用状态，无需再次禁用", ConsoleColor.Green);
                }
            }
            catch (Exception e) {
                Global.Logger.Error("Windows Defender禁用失败，因为异常", e);
            }
        }
    }
}
