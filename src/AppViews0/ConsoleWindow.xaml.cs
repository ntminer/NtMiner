using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace NTMiner.Views {
    internal class NativeMethods {
        internal const int GWL_STYLE = -16;
        internal const int WS_VISIBLE = 0x10000000;
        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    }

    public partial class ConsoleWindow : BlankWindow {
        public static readonly ConsoleWindow Instance = new ConsoleWindow();

        public ConsoleWindow() {
            this.Width = AppStatic.MainWindowWidth;
            this.Height = AppStatic.MainWindowHeight;
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e) {
            e.Cancel = true;
            this.Hide();
        }

        private bool _isFirst = true;
        public void ReSizeConsoleWindow(int marginLeft, int marginTop) {
            IntPtr console = NTMinerConsole.Show();
            bool bRepaint = !_isFirst;
            if (_isFirst) {
                IntPtr parent = new WindowInteropHelper(this).Handle;
                NativeMethods.SetParent(console, parent);
                NativeMethods.SetWindowLong(console, NativeMethods.GWL_STYLE, NativeMethods.WS_VISIBLE);
                _isFirst = false;
            }
            const int paddingLeft = 4;
            const int paddingRight = 1;
            int width = (int)this.ActualWidth - paddingLeft - paddingRight - marginLeft;
            if (width < 0) {
                width = 0;
            }
            int height = (int)this.ActualHeight - marginTop;

            NTMinerConsole.MoveWindow(console, paddingLeft + marginLeft, marginTop, width, height, bRepaint);
        }
    }
}
