using NTMiner.Core;
using System.IO;
using System.Windows;

namespace NTMiner.Vms {
    public static class AppStatic {
        public static bool IsDebugMode {
            get {
                if (Design.IsInDesignMode) {
                    return true;
                }
                return DevMode.IsDebugMode;
            }
        }

        public static bool IsNotDebugMode => !IsDebugMode;

        public static Visibility IsDebugModeVisible {
            get {
                if (IsDebugMode) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public static Visibility IsDevModeVisible {
            get {
                if (DevMode.IsDevMode) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public static string GetIconFileFullName(ICoin coin) {
            if (coin == null || string.IsNullOrEmpty(coin.Icon)) {
                return string.Empty;
            }
            string iconFileFullName = Path.Combine(SpecialPath.CoinIconsDirFullName, coin.Icon);
            return iconFileFullName;
        }
    }
}
