using System.Windows;

namespace NTMiner {
    public static class AppStatic {
        public static bool IsDebugMode {
            get {
                return Design.IsDevMode;
            }
        }

        public static bool IsNotDebugMode => !Design.IsDevMode;

        public static Visibility IsDebugModeVisible {
            get {
                if (Design.IsDevMode) {
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
