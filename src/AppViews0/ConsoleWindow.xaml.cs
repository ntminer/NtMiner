using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace NTMiner.Views {
    internal class SafeNativeMethods {
        internal const int GWL_STYLE = -16;
        internal const int WS_VISIBLE = 0x10000000;
        [DllImport(DllName.User32Dll)]
        internal static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
        [DllImport(DllName.User32Dll, SetLastError = true)]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport(DllName.User32Dll, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        internal static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
    }

    public partial class ConsoleWindow : Window {
        public static readonly ConsoleWindow Instance = new ConsoleWindow();

        private HwndSource hwndSource;
        private ConsoleWindow() {
            this.Width = AppStatic.MainWindowWidth;
            this.Height = AppStatic.MainWindowHeight;
            InitializeComponent();
            this.Loaded += (sender, e) => {
                IntPtr parent = new WindowInteropHelper(this).Handle;
                IntPtr console = NTMinerConsole.Alloc();
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

        public void MoveWindow(int marginLeft, int marginTop, int height) {
            if (!this.IsLoaded) {
                return;
            }
            const int paddingLeft = 4;
            const int paddingRight = 5;
            int width = (int)this.ActualWidth - paddingLeft - paddingRight - marginLeft;
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
            IntPtr console = NTMinerConsole.GetIntPtr();
            SafeNativeMethods.MoveWindow(console, paddingLeft + marginLeft, marginTop, width, height, true);
        }
    }
}
