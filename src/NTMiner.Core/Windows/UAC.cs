using Microsoft.Win32;
using System;
using System.Linq;

namespace NTMiner.Windows {
    public static class UAC {
        public static bool DisableUAC() {
            try {
                const string subKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
                string[] names = { "ConsentPromptBehaviorAdmin", "EnableLUA", "PromptOnSecureDesktop" };
                if (names.All(a => (int)WinRegistry.GetValue(Registry.LocalMachine, subKey, a) == 0)) {
                    Logger.OkDebugLine("UAC已经处于禁用状态，无需再次禁用");
                    return true;
                }
                else {
                    if (!Role.IsAdministrator) {
                        Logger.WarnDebugLine("禁用UAC失败，因为不是管理员");
                        return false;
                    }
                    foreach (var name in names) {
                        WinRegistry.SetValue(Registry.LocalMachine, subKey, name, 0);
                    }
                    return true;
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine("禁用UAC失败，因为异常", e);
                return false;
            }
        }
    }
}
