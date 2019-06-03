using Microsoft.Win32;
using System;

namespace NTMiner {
    public static class HotKeyUtil {
        public static Func<System.Windows.Forms.Keys, bool> RegHotKey;
        #region HotKey
        public static string GetHotKey() {
            object value = Windows.Registry.GetValue(global::Microsoft.Win32.Registry.Users, Registry.NTMinerRegistrySubKey, "HotKey");
            if (value == null) {
                return "X";
            }
            return value.ToString();
        }

        public static bool SetHotKey(string value) {
            if (RegHotKey == null) {
                return false;
            }
            if (string.IsNullOrEmpty(value)) {
                return false;
            }
            if (Enum.TryParse(value, out System.Windows.Forms.Keys key) && key >= System.Windows.Forms.Keys.A && key <= System.Windows.Forms.Keys.Z) {
                if (RegHotKey.Invoke(key)) {
                    Windows.Registry.SetValue(global::Microsoft.Win32.Registry.Users, Registry.NTMinerRegistrySubKey, "HotKey", value);
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
