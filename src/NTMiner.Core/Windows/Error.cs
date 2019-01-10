using System;

namespace NTMiner.Windows {
    public static class Error {
        public static bool DisableWindowsErrorUI() {
            try {
                const string subKey = @"Software\Microsoft\Windows\Windows Error Reporting";
                string[] keys = { "Disabled", "DontShowUI", "LoggingDisabled", "DontSendAdditionalData" };
                foreach (var key in keys) {
                    Registry.SetValue(Microsoft.Win32.Registry.CurrentUser, subKey, key, 1);
                }
                Global.DebugLine("disable windows erro ok", ConsoleColor.Green);
                return true;
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine("disable windows erro failed，因为异常", e);
                return false;
            }
        }
    }
}
