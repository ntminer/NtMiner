using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace NTMiner {
    public static class TopWindow {
        public static Window GetTopWindow() {
            var hwnd = GetForegroundWindow();
            if (hwnd == null)
                return null;

            return GetWindowFromHwnd(hwnd);
        }

        private static Window GetWindowFromHwnd(IntPtr hwnd) {
            HwndSource hds = HwndSource.FromHwnd(hwnd);
            if (hds == null) {
                return Application.Current.MainWindow;
            }
            if (hds.RootVisual is Window) {
                return (Window)hds.RootVisual;
            }
            return Application.Current.MainWindow;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
    }
}
