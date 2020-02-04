using System;
using System.Runtime.InteropServices;

namespace NTMiner {
    public struct POINT {
        public int X;
        public int Y;
    }

    internal class SafeNativeMethods {
        [DllImport(DllName.User32Dll)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport(DllName.User32Dll, CharSet = CharSet.Auto)]
        internal static extern bool GetCursorPos(out POINT pt);
    }
}
