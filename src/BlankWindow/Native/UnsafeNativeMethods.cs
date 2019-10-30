using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows;

namespace NTMiner.Native {
    /// <devdoc>http://msdn.microsoft.com/en-us/library/ms182161.aspx</devdoc>
    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeNativeMethods {
        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/aa969518%28v=vs.85%29.aspx</devdoc>
        [DllImport(DllName.DwmapiDll, PreserveSig = false, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DwmIsCompositionEnabled();

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/aa969512%28v=vs.85%29.aspx</devdoc>
        [DllImport(DllName.DwmapiDll, PreserveSig = true, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Error)]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, [In] ref MARGINS pMarInset);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/aa969524%28v=vs.85%29.aspx</devdoc>
        [DllImport(DllName.DwmapiDll, PreserveSig = true, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        internal static extern int DwmSetWindowAttribute([In] IntPtr hwnd, [In] int attr, [In] ref int attrValue, [In] int attrSize);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms633572%28v=vs.85%29.aspx</devdoc>
        [DllImport(DllName.User32Dll, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr DefWindowProc([In] IntPtr hwnd, [In] int msg, [In] IntPtr wParam, [In] IntPtr lParam);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/dd144901%28v=VS.85%29.aspx</devdoc>
        [DllImport(DllName.User32Dll, EntryPoint = "GetMonitorInfoW", ExactSpelling = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetMonitorInfo([In] IntPtr hMonitor, [Out] MONITORINFO lpmi);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/dd145064%28v=VS.85%29.aspx</devdoc>
        [DllImport(DllName.User32Dll)]
        internal static extern IntPtr MonitorFromWindow([In] IntPtr handle, [In] int flags);

        [DllImport(DllName.User32Dll, SetLastError = true)]
        internal static extern IntPtr MonitorFromPoint(POINT pt, MONITORINFO.MonitorOptions dwFlags);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms633545(v=vs.85).aspx</devdoc>
        [DllImport(DllName.User32Dll)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms647486%28v=vs.85%29.aspx</devdoc>
        [DllImport(DllName.User32Dll, CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "LoadStringW", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int LoadString([In] [Optional] SafeLibraryHandle hInstance, [In] uint uID, [Out] StringBuilder lpBuffer, [In] int nBufferMax);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms633528(v=vs.85).aspx</devdoc>
        [DllImport(DllName.User32Dll, CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool IsWindow([In] [Optional] IntPtr hWnd);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms647985(v=vs.85).aspx</devdoc>
        [DllImport(DllName.User32Dll)]
        internal static extern IntPtr GetSystemMenu([In] IntPtr hWnd, [In] bool bRevert);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms648003(v=vs.85).aspx</devdoc>
        [DllImport(DllName.User32Dll)]
        internal static extern uint TrackPopupMenuEx([In] IntPtr hmenu, [In] uint fuFlags, [In] int x, [In] int y, [In] IntPtr hwnd, [In] [Optional] IntPtr lptpm);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms644944(v=vs.85).aspx</devdoc>
        [DllImport(DllName.User32Dll, EntryPoint = "PostMessage", SetLastError = true)]
        private static extern bool _PostMessage([In] [Optional] IntPtr hWnd, [In] uint Msg, [In] IntPtr wParam, [In] IntPtr lParam);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms648390(v=vs.85).aspx</devdoc>
        [DllImport(DllName.User32Dll)]
        internal static extern bool GetCursorPos([Out] out POINT pt);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms646258(v=vs.85).aspx</devdoc>
        [DllImport(DllName.User32Dll, CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern int GetDoubleClickTime();

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms684175%28v=vs.85%29.aspx</devdoc>
        [DllImport(DllName.Kernel32Dll, CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "LoadLibraryW", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern SafeLibraryHandle LoadLibrary([In] [MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms683152%28v=vs.85%29.aspx</devdoc>
        [DllImport(DllName.Kernel32Dll, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary([In] IntPtr hModule);

        [DllImport(DllName.User32Dll)]
        internal static extern uint SetClassLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport(DllName.User32Dll)]
        internal static extern IntPtr SetClassLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport(DllName.User32Dll, EntryPoint = "GetClassLong")]
        internal static extern uint GetClassLong32(IntPtr hWnd, int nIndex);

        [DllImport(DllName.User32Dll)]
        internal static extern IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex);

        [DllImport(DllName.Gdi32Dll)]
        internal static extern IntPtr CreateSolidBrush(int crColor);

        [DllImport(DllName.Gdi32Dll)]
        internal static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Sets the show state and the restored, minimized, and maximized positions of the specified window.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window.
        /// </param>
        /// <param name="lpwndpl">
        /// A pointer to a WINDOWPLACEMENT structure that specifies the new show state and window positions.
        /// <para>
        /// Before calling SetWindowPlacement, set the length member of the WINDOWPLACEMENT structure to sizeof(WINDOWPLACEMENT). SetWindowPlacement fails if the length member is not set correctly.
        /// </para>
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// <para>
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </para>
        /// </returns>
        [DllImport(DllName.User32Dll, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        /// <summary>
        /// Retrieves the show state and the restored, minimized, and maximized positions of the specified window.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window.
        /// </param>
        /// <param name="lpwndpl">
        /// A pointer to the WINDOWPLACEMENT structure that receives the show state and position information.
        /// <para>
        /// Before calling GetWindowPlacement, set the length member to sizeof(WINDOWPLACEMENT). GetWindowPlacement fails if lpwndpl-> length is not set correctly.
        /// </para>
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// <para>
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </para>
        /// </returns>
        [DllImport(DllName.User32Dll, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        /// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms647636(v=vs.85).aspx</devdoc>
        [DllImport(DllName.User32Dll)]
        internal static extern uint EnableMenuItem(IntPtr hMenu, uint itemId, uint uEnable);

        [DllImport(DllName.User32Dll)]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport(DllName.User32Dll)]
        public static extern bool ReleaseCapture();

        internal static void PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam) {
            if (!_PostMessage(hWnd, Msg, wParam, lParam)) {
                throw new Win32Exception();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point {
            public readonly Int32 X;
            public readonly Int32 Y;
        };

        // See: http://stackoverflow.com/questions/7913325/win-api-in-c-get-hi-and-low-word-from-intptr/7913393#7913393
        internal static Point GetPoint(IntPtr ptr) {
            uint xy = unchecked(IntPtr.Size == 8 ? (uint)ptr.ToInt64() : (uint)ptr.ToInt32());
            int x = unchecked((short)xy);
            int y = unchecked((short)(xy >> 16));
            return new Point(x, y);
        }

        //internal static int GET_X_LPARAM(IntPtr lParam)
        //{
        //    return LOWORD(lParam.ToInt32());
        //}

        //internal static int GET_Y_LPARAM(IntPtr lParam)
        //{
        //    return HIWORD(lParam.ToInt32());
        //}

        //private static int HIWORD(long i)
        //{
        //    return (short)(i >> 16);
        //}

        //private static int LOWORD(long i)
        //{
        //    return (short)(i & 0xFFFF);
        //}

        internal const int GWL_STYLE = -16;
        internal const int WS_SYSMENU = 0x80000;
        [DllImport(DllName.User32Dll, SetLastError = true)]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport(DllName.User32Dll)]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [Obsolete("Use NativeMethods.FindWindow instead.")]
        [DllImport(DllName.User32Dll, SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [Obsolete("Use NativeMethods.SHAppBarMessage instead.")]
        [DllImport(DllName.Shell32Dll, CallingConvention = CallingConvention.StdCall)]
        public static extern int SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        [DllImport(DllName.User32Dll)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport(DllName.User32Dll, SetLastError = true)]
        internal static extern bool RedrawWindow(IntPtr hWnd, [In] ref RECT lprcUpdate, IntPtr hrgnUpdate, Constants.RedrawWindowFlags flags);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport(DllName.User32Dll, SetLastError = true)]
        internal static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, Constants.RedrawWindowFlags flags);
    }
}
