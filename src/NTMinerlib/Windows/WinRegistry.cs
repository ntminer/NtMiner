using Microsoft.Win32;

namespace NTMiner.Windows {
    public static class WinRegistry {
        public static bool GetIsValueNameExist(RegistryKey root, string subkey, string valueName) {
            try {
                using (RegistryKey registryKey = root.OpenSubKey(subkey, true)) {
                    if (registryKey != null) {
                        var registData = registryKey.GetValue(valueName);
                        registryKey.Close();
                        return registData != null;
                    }
                    return false;
                }
            }
            catch (System.Exception e) {
                Logger.ErrorDebugLine(e);
                return true;
            }
        }

        public static object GetValue(RegistryKey root, string subkey, string valueName) {
            object registData = "";
            try {
                using (RegistryKey registryKey = root.OpenSubKey(subkey, true)) {
                    if (registryKey != null) {
                        registData = registryKey.GetValue(valueName);
                        registryKey.Close();
                    }
                    return registData;
                }
            }
            catch (System.Exception e) {
                Logger.ErrorDebugLine(e);
                return registData;
            }
        }

        public static void SetValue(RegistryKey root, string subkey, string valueName, object value) {
            try {
                using (RegistryKey registryKey = root.CreateSubKey(subkey)) {
                    registryKey.SetValue(valueName, value);
                    registryKey.Close();
                }
            }
            catch (System.Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        public static void DeleteValue(RegistryKey root, string subkey, string valueName) {
            try {
                using (RegistryKey registryKey = root.OpenSubKey(subkey, true)) {
                    if (registryKey != null) {
                        registryKey.DeleteValue(valueName, throwOnMissingValue: false);
                        registryKey.Close();
                    }
                }
            }
            catch (System.Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
