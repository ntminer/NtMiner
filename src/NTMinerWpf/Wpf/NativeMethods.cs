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
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);
    }
}
