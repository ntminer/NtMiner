using NTMiner.Vms;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace NTMiner.Views.Ucs {
    internal class NativeMethods {
        internal const int GWL_STYLE = -16;
        internal const int WS_VISIBLE = 0x10000000;
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    }

    public partial class Console : UserControl {
        private ConsoleViewModel Vm {
            get {
                return (ConsoleViewModel)this.DataContext;
            }
        }

        public Console() {
            NTMinerRoot.RefreshArgsAssembly.Invoke();
            InitializeComponent();
            this.SizeChanged += Console_SizeChanged;
        }

        private void Console_SizeChanged(object sender, SizeChangedEventArgs e) {
            ReSizeConsoleWindow();
        }

        private bool _isFirst = true;
        private void ReSizeConsoleWindow() {
            IntPtr console = NTMinerConsole.Show();
            Window window = Window.GetWindow(this);
            Point point = this.TransformToAncestor(window).Transform(new Point(0, 0));
            if (_isFirst) {
                IntPtr parent = new WindowInteropHelper(window).Handle;
                NativeMethods.SetParent(console, parent);
                NativeMethods.SetWindowLong(console, NativeMethods.GWL_STYLE, NativeMethods.WS_VISIBLE);
                _isFirst = false;
            }
            int width = (int)this.ActualWidth;
            int height = (int)this.ActualHeight;

            NTMinerConsole.MoveWindow(console, (int)point.X, (int)point.Y, width, height, true);
        }
    }
}
