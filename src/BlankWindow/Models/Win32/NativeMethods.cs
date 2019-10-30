using System;
using System.Runtime.InteropServices;

namespace NTMiner.Models.Win32 {
    internal static class SafeNativeMethods {
        public static WS GetWindowLong(this IntPtr hWnd) {
            return (WS)GetWindowLong(hWnd, (int)GWL.STYLE);
        }
        public static WSEX GetWindowLongEx(this IntPtr hWnd) {
            return (WSEX)GetWindowLong(hWnd, (int)GWL.EXSTYLE);
        }

        [DllImport(DllName.User32Dll, EntryPoint = "GetWindowLongA", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public static WS SetWindowLong(this IntPtr hWnd, WS dwNewLong) {
            return (WS)SetWindowLong(hWnd, (int)GWL.STYLE, (int)dwNewLong);
        }
        public static WSEX SetWindowLongEx(this IntPtr hWnd, WSEX dwNewLong) {
            return (WSEX)SetWindowLong(hWnd, (int)GWL.EXSTYLE, (int)dwNewLong);
        }

        [DllImport(DllName.User32Dll)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport(DllName.User32Dll)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP flags);

        [DllImport(DllName.User32Dll, SetLastError = true)]
        public static extern bool PostMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);
    }
}
