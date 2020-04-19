using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace NTMiner.Views {
    public partial class ConsoleWindow : Window {
        public static readonly ConsoleWindow Instance = new ConsoleWindow();

        private HwndSource hwndSource;
        private ConsoleWindow() {
            this.Width = AppRoot.MainWindowWidth;
            this.Height = AppRoot.MainWindowHeight;
            InitializeComponent();
            this.Loaded += (sender, e) => {
                IntPtr parent = new WindowInteropHelper(this).Handle;
                IntPtr console = NTMinerConsole.GetOrAlloc();
                SafeNativeMethods.SetParent(console, parent);
                SafeNativeMethods.SetWindowLong(console, SafeNativeMethods.GWL_STYLE, SafeNativeMethods.WS_VISIBLE);
                hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
                hwndSource.AddHook(new HwndSourceHook(Win32Proc.WindowProc));
            };
        }

        protected override void OnClosed(EventArgs e) {
            hwndSource?.Dispose();
            hwndSource = null;
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private int _marginLeft, _marginTop, _height, _width;

        public void MoveWindow(int marginLeft, int marginTop, int width, int height) {
            if (!this.IsLoaded) {
                return;
            }
            if (width < 0) {
                width = 0;
            }
            if (_marginLeft == marginLeft && _marginTop == marginTop && _height == height && _width == width) {
                return;
            }
            _marginLeft = marginLeft;
            _marginTop = marginTop;
            _height = height;
            _width = width;
            // 如果没有ConsoleBgRectangle的话鼠标会点击到桌面上
            if (ConsoleBgRectangle.Width != width) {
                ConsoleBgRectangle.Width = width;
            }
            if (ConsoleBgRectangle.Height != height) {
                ConsoleBgRectangle.Height = height;
            }
            if ((int)ConsoleBgRectangle.Margin.Top != marginTop) {
                ConsoleBgRectangle.Margin = new Thickness(0, marginTop, 1, 0);
            }
            NTMinerConsole.MoveWindow(marginLeft, marginTop, width, height, true);
        }
    }
}
