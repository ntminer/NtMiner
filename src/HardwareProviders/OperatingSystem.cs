using System;
using System.Diagnostics;
using HardwareProviders.Internals;

namespace HardwareProviders {
    public static class OperatingSystem {
        static OperatingSystem() {
            // The operating system doesn't change during execution so let's query it just one time.
            var platform = Environment.OSVersion.Platform;
            IsLinux = platform == PlatformID.Unix || platform == PlatformID.MacOSX;


            if (IntPtr.Size == 8)
                Is64Bit = true;
            else if (!IsLinux)
                try {
                    SystemCall.IsWow64Process(Process.GetCurrentProcess().Handle, out var _);
                    // If we are still here, this is a 64bit windows; 32bit windows does
                    // not provide IsWow64Process.
                    Is64Bit = true;
                }
                catch (EntryPointNotFoundException) {
                    // IsWow64Process is not present on 32 bit:
                    Is64Bit = false;
                }
        }

        public static bool Is64Bit { get; }

        public static bool IsLinux { get; }
    }
}