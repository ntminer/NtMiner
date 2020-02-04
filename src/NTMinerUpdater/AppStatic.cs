using System.Windows;

namespace NTMiner {
    public static class AppStatic {
        public static bool IsDevMode {
            get {
                return WpfUtil.IsDevMode;
            }
        }

        public static bool IsNotDevMode => !WpfUtil.IsDevMode;

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
