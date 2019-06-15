using Microsoft.Win32;
using System;

namespace NTMiner.Windows {
    public static class Defender {
        public static void DisableAntiSpyware() {
            try {
                const string subKey = @"SOFTWARE\Policies\Microsoft\Windows Defender";
                var v = WinRegistry.GetValue(Registry.LocalMachine, subKey, "DisableAntiSpyware");
                if (v == null || v.ToString() != "1") {
                    WinRegistry.SetValue(Registry.LocalMachine, subKey, "DisableAntiSpyware", 1);
                    Logger.OkDebugLine("Windows Defender禁用成功");
                }
                else {
                    Logger.OkDebugLine("Windows Defender已经处于禁用状态，无需再次禁用");
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine("Windows Defender禁用失败，因为异常", e);
            }
        }
    }
}
