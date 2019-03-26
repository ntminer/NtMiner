using System;
using System.Runtime.InteropServices;

namespace NTMiner.Wpf {
    public struct POINT {
        public int X;
        public int Y;
        public POINT(int x, int y) {
            this.X = x;
            this.Y = y;
        }
    }

    public class NativeMethods {
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetCursorPos(out POINT pt);
    }
}
