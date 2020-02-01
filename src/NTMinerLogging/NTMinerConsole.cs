using System;
using System.Runtime.InteropServices;

namespace NTMiner {
    public static class NTMinerConsole {
        private static class SafeNativeMethods {
            [DllImport(DllName.Kernel32Dll)]
            private static extern bool AllocConsole();
            [DllImport(DllName.Kernel32Dll)]
            internal static extern bool FreeConsole();
            [DllImport(DllName.Kernel32Dll)]
            internal static extern IntPtr GetConsoleWindow();
            [DllImport(DllName.User32Dll, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
            internal static extern void MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

            [DllImport(DllName.Kernel32Dll, SetLastError = true)]
            private static extern IntPtr GetStdHandle(int hConsoleHandle);
            [DllImport(DllName.Kernel32Dll, SetLastError = true)]
            private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint mode);
            [DllImport(DllName.Kernel32Dll, SetLastError = true)]
            private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);

            private static void DisbleQuickEditMode() {
                const int STD_INPUT_HANDLE = -10;
                const uint ENABLE_PROCESSED_INPUT = 0x0001;
                const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
                const uint ENABLE_INSERT_MODE = 0x0020;

                IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
                GetConsoleMode(hStdin, out uint mode);
                mode &= ~ENABLE_PROCESSED_INPUT;//禁用ctrl+c
                mode &= ~ENABLE_QUICK_EDIT_MODE;//移除快速编辑模式
                mode &= ~ENABLE_INSERT_MODE;    //移除插入模式
                SetConsoleMode(hStdin, mode);
            }

            internal static IntPtr GetOrAlloc(bool disableQuickEditMode) {
                IntPtr console = GetConsoleWindow();
                if (console == IntPtr.Zero) {
                    AllocConsole();
                    if (disableQuickEditMode) {
                        DisbleQuickEditMode();
                    }
                    console = GetConsoleWindow();
                }
                return console;
            }
        }

        public static IntPtr GetOrAlloc(bool disableQuickEditMode = true) {
            return SafeNativeMethods.GetOrAlloc(disableQuickEditMode);
        }

        public static void MoveWindow(int x, int y, int nWidth, int nHeight, bool bRepaint) {
            SafeNativeMethods.MoveWindow(SafeNativeMethods.GetConsoleWindow(), x, y, nWidth, nHeight, bRepaint);
        }

        /// <summary>
        /// 如果程序没有控制台窗口调用没有副作用，应在程序生命周期最末尾调用。
        /// 因为控制台窗口是按需创建的，所以不能在顺序不定的AppExit中释放，以免释放后又按需创建。
        /// </summary>
        public static void Free() {
            IntPtr console = SafeNativeMethods.GetConsoleWindow();
            if (console != IntPtr.Zero) {
                SafeNativeMethods.FreeConsole();
            }
        }
    }
}
