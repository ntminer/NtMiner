using System;
using System.Runtime.InteropServices;

namespace NTMiner {
    public static class NTMinerConsole {
        private static class SafeNativeMethods {
            [DllImport(DllName.Kernel32Dll)]
            internal static extern bool AllocConsole();
            [DllImport(DllName.Kernel32Dll)]
            internal static extern bool FreeConsole();
            [DllImport(DllName.Kernel32Dll)]
            internal static extern IntPtr GetConsoleWindow();

            [DllImport(DllName.Kernel32Dll, SetLastError = true)]
            internal static extern IntPtr GetStdHandle(int hConsoleHandle);
            [DllImport(DllName.Kernel32Dll, SetLastError = true)]
            internal static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint mode);
            [DllImport(DllName.Kernel32Dll, SetLastError = true)]
            internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);
            [DllImport(DllName.User32Dll, SetLastError = true)]
            internal static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        }

        private static void DisbleQuickEditMode() {
            const int STD_INPUT_HANDLE = -10;
            const uint ENABLE_PROCESSED_INPUT = 0x0001;
            const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
            const uint ENABLE_INSERT_MODE = 0x0020;

            IntPtr hStdin = SafeNativeMethods.GetStdHandle(STD_INPUT_HANDLE);
            SafeNativeMethods.GetConsoleMode(hStdin, out uint mode);
            mode &= ~ENABLE_PROCESSED_INPUT;//禁用ctrl+c
            mode &= ~ENABLE_QUICK_EDIT_MODE;//移除快速编辑模式
            mode &= ~ENABLE_INSERT_MODE;    //移除插入模式
            SafeNativeMethods.SetConsoleMode(hStdin, mode);
        }

        public static IntPtr Alloc() {
            IntPtr console = SafeNativeMethods.GetConsoleWindow();
            if (console == IntPtr.Zero) {
                SafeNativeMethods.AllocConsole();
                DisbleQuickEditMode();
                console = SafeNativeMethods.GetConsoleWindow();
                SafeNativeMethods.ShowWindow(console, 0);
            }
            return console;
        }

        public static IntPtr GetIntPtr() {
            return SafeNativeMethods.GetConsoleWindow();
        }

        public static IntPtr Show() {
            _isHided = false;
            IntPtr console = GetIntPtr();
            if (console != IntPtr.Zero) {
                SafeNativeMethods.ShowWindow(console, 1);
            }
            return console;
        }

        private static bool _isHided = false;
        public static void Hide() {
            if (_isHided) {
                return;
            }
            _isHided = true;
            IntPtr console = SafeNativeMethods.GetConsoleWindow();
            if (console != IntPtr.Zero) {
                SafeNativeMethods.ShowWindow(console, 0);
            }
        }

        public static void Free() {
            IntPtr console = SafeNativeMethods.GetConsoleWindow();
            if (console != IntPtr.Zero) {
                SafeNativeMethods.FreeConsole();
            }
        }
    }
}
