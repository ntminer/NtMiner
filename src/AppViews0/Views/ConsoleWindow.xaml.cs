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
                SafeNativeMethods.SetParent(console, _thisWindowHandle);
                SafeNativeMethods.SetWindowLong(console, SafeNativeMethods.GWL_STYLE, SafeNativeMethods.WS_VISIBLE);
                hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
                hwndSource.AddHook(new HwndSourceHook(WndProc));
                if (AppUtil.IsHotKeyEnabled) {
                    HotKeyUtil.RegHotKey = (key) => {
                        if (!RegHotKey(key, out string message)) {
                            VirtualRoot.Out.ShowError(message, autoHideSeconds: 4);
                            return false;
                        }
                        else {
                            VirtualRoot.Out.ShowSuccess($"热键Ctrl + Alt + {key.ToString()} 设置成功");
                            return true;
                        }
                    };
                }
            };
        }

        protected override void OnClosed(EventArgs e) {
            if (AppUtil.IsHotKeyEnabled) {
                SystemHotKey.UnRegHotKey(_thisWindowHandle, c_hotKeyId);
            }
            hwndSource?.Dispose();
            hwndSource = null;
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private bool RegHotKey(System.Windows.Forms.Keys key, out string message) {
            if (!SystemHotKey.RegHotKey(_thisWindowHandle, c_hotKeyId, SystemHotKey.KeyModifiers.Alt | SystemHotKey.KeyModifiers.Ctrl, key, out message)) {
                message = $"Ctrl + Alt + {key.ToString()} " + message;
                return false;
            }
            else {
                return true;
            }
        }

        protected override void OnContentRendered(EventArgs e) {
            base.OnContentRendered(e);
            if (AppUtil.IsHotKeyEnabled) {
                Enum.TryParse(HotKeyUtil.GetHotKey(), out System.Windows.Forms.Keys hotKey);
                if (!RegHotKey(hotKey, out string message)) {
                    VirtualRoot.Out.ShowWarn(message, header: "热键设置失败", toConsole: true);
                }
            }
        }

        private const int WM_HOTKEY = 0x312;
        private const int c_hotKeyId = 1; //热键ID（自定义）
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            switch (msg) {
                case WM_HOTKEY:
                    int tmpWParam = wParam.ToInt32();
                    if (tmpWParam == c_hotKeyId) {
                        VirtualRoot.Execute(new ShowMainWindowCommand(isToggle: true));
                    }
                    break;
                default:
                    break;
            }
            return Win32Proc.WindowProc(hwnd, msg, wParam, lParam, ref handled);
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
