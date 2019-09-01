using System;
using System.Runtime.InteropServices;

namespace NTMiner {
    public static class NTMinerConsole {
        private const string Kernel32DllName = "kernel32.dll";
        [DllImport(Kernel32DllName)]
        private static extern bool AllocConsole();
        [DllImport(Kernel32DllName)]
        private static extern bool FreeConsole();
        [DllImport(Kernel32DllName)]
        private static extern IntPtr GetConsoleWindow();

        const int STD_INPUT_HANDLE = -10;
        const uint ENABLE_PROCESSED_INPUT = 0x0001;
        const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
        const uint ENABLE_INSERT_MODE = 0x0020;
        [DllImport(Kernel32DllName, SetLastError = true)]
        private static extern IntPtr GetStdHandle(int hConsoleHandle);
        [DllImport(Kernel32DllName, SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint mode);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        [DllImport(Kernel32DllName, SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        public static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        private static void DisbleQuickEditMode() {
            IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
            uint mode;
            GetConsoleMode(hStdin, out mode);
            mode &= ~ENABLE_PROCESSED_INPUT;//禁用ctrl+c
            mode &= ~ENABLE_QUICK_EDIT_MODE;//移除快速编辑模式
            mode &= ~ENABLE_INSERT_MODE;    //移除插入模式
            SetConsoleMode(hStdin, mode);
        }

        public static IntPtr Show() {
            IntPtr console = GetConsoleWindow();
            if (console == IntPtr.Zero) {
                AllocConsole();
                DisbleQuickEditMode();
                console = GetConsoleWindow();
                MoveWindow(console, -1000, 0, 0, 0, false);
                ShowWindow(console, 0);
            }
            return console;
        }

        public static void Hide() {
            IntPtr console = GetConsoleWindow();
            if (console != IntPtr.Zero) {
                FreeConsole();
            }
        }
    }
}
