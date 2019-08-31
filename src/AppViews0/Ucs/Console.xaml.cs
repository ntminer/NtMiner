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
        public static void ShowWindow() {
            var win = ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "日志窗口",
                IconName = "Icon_Calc",
                Width = 640,
                Height = 400,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => {
                var uc = new Console();
                NTMinerConsole.ShowWindow(NTMinerConsole.Show(), 1);
                return uc;
            }, fixedSize: false);
            win.Closing += (object sender, System.ComponentModel.CancelEventArgs e)=> {
                e.Cancel = true;
                win.Hide();
            };
        }

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
            Point point = Rectangle.TransformToAncestor(window).Transform(new Point(0, 0));
            if (_isFirst) {
                IntPtr parent = new WindowInteropHelper(window).Handle;
                NativeMethods.SetParent(console, parent);
                NativeMethods.SetWindowLong(console, NativeMethods.GWL_STYLE, NativeMethods.WS_VISIBLE);
                _isFirst = false;
            }
            int width = (int)Rectangle.ActualWidth;
            int height = (int)Rectangle.ActualHeight;

            NTMinerConsole.MoveWindow(console, (int)point.X, (int)point.Y, width, height, true);
        }
    }
}
