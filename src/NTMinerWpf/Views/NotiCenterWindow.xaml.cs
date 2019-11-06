using NTMiner.Notifications;
using NTMiner.Vms;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace NTMiner.Views {
    public partial class NotiCenterWindow : Window {
        private static NotiCenterWindow _instance;
        public static NotiCenterWindow Instance {
            get {
                if (_instance == null) {
                    _instance = new NotiCenterWindow();
                }
                return _instance;
            }
        }

        public static EventHandler CreateNotiCenterWindowLocationManager(Window window) {
            return (sender, e) => {
                Instance.Left = window.Left + (window.Width - Instance.Width) / 2;
                Instance.Top = window.Top + 10;
            };
        }

        public NotiCenterWindowViewModel Vm {
            get { return NotiCenterWindowViewModel.Instance; }
        }

        private NotiCenterWindow() {
            this.DataContext = Vm;
            InitializeComponent();
            if (NotiCenterWindowViewModel.IsHotKeyEnabled) {
                HotKeyUtil.RegHotKey = (key) => {
                    if (!RegHotKey(key, out string message)) {
                        VirtualRoot.Out.ShowError(message, 4);
                        return false;
                    }
                    else {
                        VirtualRoot.ThisWorkerInfo(nameof(NotiCenterWindow), $"热键Ctrl + Alt + {key.ToString()} 设置成功", OutEnum.Success);
                        return true;
                    }
                };
            }
        }

        public void SwitchOwner(Window window) {
            if (Owner != window) {
                bool isOwnerIsTopMost = window.Topmost;
                if (isOwnerIsTopMost) {
                    window.Topmost = false;
                }
                if (window != null) {
                    window.IsVisibleChanged -= Owner_IsVisibleChanged;
                    window.StateChanged -= Owner_StateChanged;
                }
                Owner = window;
                Owner.IsVisibleChanged += Owner_IsVisibleChanged;
                Owner.StateChanged += Owner_StateChanged;
                Instance.Left = window.Left + (window.Width - Instance.Width) / 2;
                Instance.Top = window.Top + 10;
                if (isOwnerIsTopMost) {
                    window.Topmost = true;
                    this.Topmost = true;
                    Owner.Activate();
                }
                else {
                    this.Activate();
                }
            }
            else {
                bool isOwnerIsTopMost = window.Topmost;
                if (isOwnerIsTopMost) {
                    Owner.Activate();
                }
            }
        }

        private void Owner_StateChanged(object sender, EventArgs e) {
            Window owner = (Window)sender;
            if (this.Owner == owner && owner.WindowState == WindowState.Minimized) {
                this.Owner = null;
                if (!this.IsVisible) {
                    this.Show();
                }
            }
        }

        private void Owner_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            Window owner = (Window)sender;
            if (this.Owner == owner && !owner.IsVisible) {
                this.Owner = null;
            }
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

        private IntPtr _thisWindowHandle;
        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);
            if (NotiCenterWindowViewModel.IsHotKeyEnabled) {
                _thisWindowHandle = new WindowInteropHelper(this).Handle;
                HwndSource hWndSource = HwndSource.FromHwnd(_thisWindowHandle);
                if (hWndSource != null) {
                    hWndSource.AddHook(WndProc);
                }
            }
        }

        protected override void OnContentRendered(EventArgs e) {
            base.OnContentRendered(e);
            if (NotiCenterWindowViewModel.IsHotKeyEnabled) {
                Enum.TryParse(HotKeyUtil.GetHotKey(), out System.Windows.Forms.Keys hotKey);
                if (!RegHotKey(hotKey, out string message)) {
                    NotiCenterWindowViewModel.Instance.Manager
                        .CreateMessage()
                        .Warning(message)
                        .Dismiss().WithButton("忽略", null)
                        .Queue();
                }
            }
        }

        protected override void OnClosed(EventArgs e) {
            if (NotiCenterWindowViewModel.IsHotKeyEnabled) {
                SystemHotKey.UnRegHotKey(_thisWindowHandle, c_hotKeyId);
            }
            base.OnClosed(e);
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
            return IntPtr.Zero;
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void Window_StateChanged(object sender, EventArgs e) {
            if (WindowState != WindowState.Normal) {
                WindowState = WindowState.Normal;
            }
        }
    }
}
