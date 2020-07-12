using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace NTMiner.Views {
    public partial class ConsoleWindow : Window {
        public static ConsoleWindow Instance { get; private set; } = new ConsoleWindow();

        private IntPtr _thisWindowHandle;
        private HwndSource hwndSource;
        private ConsoleWindow() {
            this.Width = AppRoot.MainWindowWidth;
            this.Height = AppRoot.MainWindowHeight;
            InitializeComponent();
            this.Loaded += (sender, e) => {
                _thisWindowHandle = new WindowInteropHelper(this).Handle;
                IntPtr console = NTMinerConsole.GetOrAlloc();
                SafeNativeMethods.SetWindowLong(console, SafeNativeMethods.GWL_STYLE, SafeNativeMethods.WS_VISIBLE);
                SafeNativeMethods.SetParent(console, _thisWindowHandle);
                hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
                hwndSource.AddHook(new HwndSourceHook(Win32Proc.WindowProc));
            };
            // 延迟展示从而避免不需要展示红字的时候看到红字
            VirtualRoot.AddOnecePath<HasBoot1SecondEvent>("启动一会后显式指引解决WindowsZoomBug的一行红字", LogEnum.None, action: message => {
                LbtnWindowsZoomBug.Visibility = Visibility.Visible;
            }, pathId: PathId.Empty, this.GetType());
        }

        protected override void OnClosed(EventArgs e) {
            hwndSource?.Dispose();
            hwndSource = null;
            base.OnClosed(e);
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
            NTMinerConsole.MoveWindow(marginLeft, marginTop, width, height, true);
        }
    }
}
