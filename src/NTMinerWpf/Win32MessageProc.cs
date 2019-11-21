using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace NTMiner {
    public class Win32MessageProc {
        private static class SafeNativeMethods {
            #region enum struct class
            [StructLayout(LayoutKind.Sequential)]
            public struct POINT {
                public int X;
                public int Y;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MINMAXINFO {
                public POINT ptReserved;
                public POINT ptMaxSize;
                public POINT ptMaxPosition;
                public POINT ptMinTrackSize;
                public POINT ptMaxTrackSize;
            };

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public class MONITORINFO {
                public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                public RECT rcMonitor = new RECT();
                public RECT rcWork = new RECT();
                public int dwFlags = 0;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT {
                public int Left, Top, Right, Bottom;
            }

            #endregion

            [DllImport(DllName.User32Dll)]
            internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

            [DllImport(DllName.User32Dll)]
            internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
        }

        private readonly Window _window;
        public Win32MessageProc(Window window) {
            _window = window;
        }

        public IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            switch (msg) {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        #region 最大化窗口时避免最大化到Windows任务栏
        private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam) {
            SafeNativeMethods.MINMAXINFO mmi = (SafeNativeMethods.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(SafeNativeMethods.MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = SafeNativeMethods.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero) {
                SafeNativeMethods.MONITORINFO monitorInfo = new SafeNativeMethods.MONITORINFO();
                SafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);
                SafeNativeMethods.RECT rcWorkArea = monitorInfo.rcWork;
                SafeNativeMethods.RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }
        #endregion

    }
}
