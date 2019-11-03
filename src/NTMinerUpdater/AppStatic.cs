using System.Windows;

namespace NTMiner {
    public static class AppStatic {
        public static bool IsDebugMode {
            get {
                return WpfUtil.IsDevMode;
            }
        }

        public static bool IsNotDebugMode => !WpfUtil.IsDevMode;

        public static Visibility IsDebugModeVisible {
            get {
                if (WpfUtil.IsDevMode) {
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
    }
}
