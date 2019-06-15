using Microsoft.Win32;

namespace NTMiner.Windows {
    public static class WinRegistry {
        public static object GetValue(RegistryKey root, string subkey, string valueName) {
            object registData = "";
            try {
                RegistryKey myKey = root.OpenSubKey(subkey, true);
                if (myKey != null) {
                    registData = myKey.GetValue(valueName);
                }

                return registData;
            }
            catch (System.Exception e) {
                Logger.ErrorDebugLine(e);
                return registData;
            }
        }

        public static void SetValue(RegistryKey root, string subkey, string valueName, object value) {
            try {
                RegistryKey registryKey = root.CreateSubKey(subkey);
                registryKey.SetValue(valueName, value);
                registryKey.Close();
            }
            catch (System.Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        public static void DeleteValue(RegistryKey root, string subkey, string valueName) {
            try {
                RegistryKey myKey = root.OpenSubKey(subkey, true);
                if (myKey != null) {
                    myKey.DeleteValue(valueName, throwOnMissingValue: false);
                    myKey.Close();
                }
            }
            catch (System.Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
