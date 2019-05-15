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
            }
            else {
                if (isToggle) {
                    window.WindowState = WindowState.Minimized;
                }
                else {
                    var oldState = window.WindowState;
                    window.WindowState = WindowState.Minimized;
                    window.WindowState = oldState;
                }
            }
            if (!isToggle) {
                window.Activate();
            }
        }
    }
}
