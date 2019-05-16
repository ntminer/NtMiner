using System.Windows;

namespace NTMiner {
    public static class WindowExtension {
        public static void ShowWindow(this Window window, bool isToggle) {
            if (!isToggle) {
                window.Show();
            }
            window.ShowInTaskbar = true;
            if (window.WindowState == WindowState.Minimized) {
                window.WindowState = WindowState.Normal;
                window.Activate();
            }
            else if (isToggle) {
                window.WindowState = WindowState.Minimized;
            }
        }
    }
}
