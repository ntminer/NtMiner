using Microsoft.Win32;

namespace NTMiner {
    public static partial class NTMinerRegistry {
        #region IsOuterUserEnabled
        public static bool GetIsOuterUserEnabled() {
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.IsOuterUserEnabledRegistryKey);
            if (value == null) {
                return false;
            }
            string str = value.ToString();
            if (!bool.TryParse(str, out bool isOuterUserEnabled)) {
                return false;
            }
            return isOuterUserEnabled;
        }
        #endregion

        #region GetOuterUserId
        public static string GetOuterUserId() {
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.OuterUserIdRegistryKey);
            return (value ?? string.Empty).ToString();
        }
        #endregion

        #region WorkType
        public static WorkType GetWorkType() {
            object value = Windows.WinRegistry.GetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.WorkTypeRegistryKey);
            if (value != null && value.ToString().TryParse(out WorkType workType)) {
                return workType;
            }
            return WorkType.None;
        }

        public static void SetWorkType(WorkType workType) {
            Windows.WinRegistry.SetValue(Registry.Users, NTMinerRegistrySubKey, NTKeyword.WorkTypeRegistryKey, workType.ToString());
        }
        #endregion
    }
}
