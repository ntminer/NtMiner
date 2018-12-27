using System.Diagnostics;

namespace NTMiner.Windows {
    public static class App {
        private const string autoRunSubKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        public static void SetAutoBoot(string valueName, bool isAutoBoot, string otherParams = null) {
            string exeFileFullName = Process.GetCurrentProcess().MainModule.FileName;
            if (isAutoBoot == true) {
                string value = exeFileFullName;
                if (!string.IsNullOrEmpty(otherParams)) {
                    value = value + " " + otherParams;
                }
                Registry.SetValue(Microsoft.Win32.Registry.CurrentUser, autoRunSubKey, valueName, value);
            }
            else {
                Registry.DeleteValue(Microsoft.Win32.Registry.CurrentUser, autoRunSubKey, valueName);
            }
        }
    }
}
