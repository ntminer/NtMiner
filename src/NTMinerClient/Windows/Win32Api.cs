using System;
using System.Runtime.InteropServices;

namespace MinerClient.Windows {
    public static class Win32Api {
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
        public static extern IntPtr LoadLibrary(string lpLibFileName);
    }
}
