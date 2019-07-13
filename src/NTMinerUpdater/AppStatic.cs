using System.Windows;

namespace NTMiner {
    public static class AppStatic {
        public static bool IsDebugMode {
            get {
                return Design.IsDebugMode;
            }
        }

        public static bool IsNotDebugMode => !Design.IsDebugMode;

        public static Visibility IsDebugModeVisible {
            get {
                if (Design.IsDebugMode) {
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
