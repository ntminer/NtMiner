using System;
using System.Runtime.InteropServices;

namespace HardwareProviders.Internals {
    static class SystemCall {
        private const string KERNEL = "kernel32.dll";

        [DllImport(KERNEL, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);
    }
}
