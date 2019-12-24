using System;
using System.Runtime.InteropServices;

namespace NTMiner {
    public static class Win32Proc {
        public static class SafeNativeMethods {
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
            internal struct APPBARDATA {
                /// <summary>
                /// initialize this field using: Marshal.SizeOf(typeof(APPBARDATA));
                /// </summary>
                public int cbSize;
                public IntPtr hWnd;
                public int uCallbackMessage;
                public int uEdge;
                public RECT rc;
                public bool lParam;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT {
                public int Left, Top, Right, Bottom;
            }

            #endregion

            [DllImport(DllName.User32Dll, SetLastError = true)]
            internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            [DllImport(DllName.Shell32Dll, CallingConvention = CallingConvention.StdCall)]
            internal static extern uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

            [DllImport(DllName.User32Dll)]
            internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

            [DllImport(DllName.User32Dll)]
            internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
        }

        public enum WindowsTaskbarEdge {
            Left = 0, 
            Top = 1, 
            Right = 2, 
            Bottom = 3
        }

        /// <summary>
        /// 获取windows任务栏的方位和工作区距离屏幕边缘的距离。
        /// </summary>
        /// <param name="margin">任务栏与工作区的接触边距离屏幕边的距离</param>
        /// <returns></returns>
        public static WindowsTaskbarEdge GetWindowsTaskbarEdge(out double margin) {
            IntPtr hwnd = SafeNativeMethods.FindWindow("Shell_TrayWnd", null);
            var abd = new SafeNativeMethods.APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = hwnd;
            SafeNativeMethods.SHAppBarMessage(5, ref abd);
            switch (abd.uEdge) {
                case (int)WindowsTaskbarEdge.Left:
                    margin = Math.Abs(abd.rc.Left - abd.rc.Right);
                    return WindowsTaskbarEdge.Left;
                case (int)WindowsTaskbarEdge.Top:
                    margin = Math.Abs(abd.rc.Top - abd.rc.Bottom);
                    return WindowsTaskbarEdge.Top;
                case (int)WindowsTaskbarEdge.Right:
                    margin = Math.Abs(abd.rc.Left - abd.rc.Right);
                    return WindowsTaskbarEdge.Right;
                case (int)WindowsTaskbarEdge.Bottom:
                    margin = Math.Abs(abd.rc.Top - abd.rc.Bottom);
                    return WindowsTaskbarEdge.Bottom;
                default:
                    margin = 0;
                    return WindowsTaskbarEdge.Bottom;
            }
        }

        public static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            switch (msg) {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        #region 最大化窗口时避免最大化到Windows任务栏
        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam) {
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
