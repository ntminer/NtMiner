using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace NTMiner {
    public static class TopWindow {
        private static class NativeMethods {
            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();
        }

        public static Window GetTopWindow() {
            IntPtr hwnd = NativeMethods.GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
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
    }
}
