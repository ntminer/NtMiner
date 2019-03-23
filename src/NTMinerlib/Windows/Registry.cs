using Microsoft.Win32;

namespace NTMiner.Windows {
    public static class Registry {
        #region Registry
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
                Logger.ErrorDebugLine(e.Message, e);
                return registData;
            }
        }

        public static void SetValue(RegistryKey root, string subkey, string valueName, object value) {
            try {
                RegistryKey registryKey = root.CreateSubKey(subkey);
                registryKey.SetValue(valueName, value);
            }
            catch (System.Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        public static void DeleteValue(RegistryKey root, string subkey, string valueName) {
            try {
                RegistryKey myKey = root.OpenSubKey(subkey, true);
                myKey?.DeleteValue(valueName, throwOnMissingValue: false);
            }
            catch (System.Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
        #endregion
    }
}
