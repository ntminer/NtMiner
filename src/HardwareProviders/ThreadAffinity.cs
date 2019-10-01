using System;
using System.Runtime.InteropServices;

namespace HardwareProviders {
    public static class ThreadAffinity {
        public static ulong Set(ulong mask) {
            if (mask == 0)
                return 0;

            if (OperatingSystem.IsLinux) {
                // Unix
                ulong result = 0;
                if (NativeMethods.sched_getaffinity(0, (IntPtr)Marshal.SizeOf(result),
                        ref result) != 0)
                    return 0;
                if (NativeMethods.sched_setaffinity(0, (IntPtr)Marshal.SizeOf(mask),
                        ref mask) != 0)
                    return 0;
                return result;
            } // Windows

            UIntPtr uIntPtrMask;
            try {
                uIntPtrMask = (UIntPtr)mask;
            }
            catch (OverflowException) {
                throw new ArgumentOutOfRangeException(nameof(mask));
            }

            return (ulong)NativeMethods.SetThreadAffinityMask(
                NativeMethods.GetCurrentThread(), uIntPtrMask);
        }

        private static class NativeMethods {
            private const string KERNEL = "kernel32.dll";

            private const string LIBC = "libc";

            [DllImport(KERNEL, CallingConvention = CallingConvention.Winapi)]
            public static extern UIntPtr
                SetThreadAffinityMask(IntPtr handle, UIntPtr mask);

            [DllImport(KERNEL, CallingConvention = CallingConvention.Winapi)]
            public static extern IntPtr GetCurrentThread();

            [DllImport(LIBC)]
            public static extern int sched_getaffinity(int pid, IntPtr maskSize,
                ref ulong mask);

            [DllImport(LIBC)]
            public static extern int sched_setaffinity(int pid, IntPtr maskSize,
                ref ulong mask);
        }
    }
}