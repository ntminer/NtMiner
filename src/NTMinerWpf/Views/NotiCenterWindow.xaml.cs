using NTMiner.Notifications;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace NTMiner.Views {
    public partial class NotiCenterWindow : Window {
        private static readonly NotiCenterWindow _instance = new NotiCenterWindow();

        public static void ShowWindow() {
            _instance.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="ownerIsTopMost"></param>
        /// <param name="isNoOtherWindow">如果没有其它窗口就不需要响应窗口激活和非激活状态变更事件了</param>
        public static void Bind(Window owner, bool ownerIsTopMost = false, bool isNoOtherWindow = false) {
            void handler(object sender, EventArgs e) {
                _instance.Left = owner.Left + (owner.Width - _instance.Width) / 2;
                _instance.Top = owner.Top + 10;
            }
            if (ownerIsTopMost) {
                if (!isNoOtherWindow) {
                    owner.Activated += (sender, e) => {
                        // 解决当主界面上方出现popup层时主窗口下面的控制台窗口可能会被windows绘制到上面的BUG
                        if (!owner.Topmost) {
                            owner.Topmost = true;
                        }
                        handler(sender, e);
                        _instance.SwitchOwner(owner);
                    };
                }
                if (!isNoOtherWindow) {
                    owner.Deactivated += (sender, e) => {
                        // 解决当主界面上方出现popup层时主窗口下面的控制台窗口可能会被windows绘制到上面的BUG
                        if (owner.Topmost) {
                            owner.Topmost = false;
                        }
                    };
                }
            }
            else {
                if (!isNoOtherWindow) {
                    owner.Activated += (sender, e) => {
                        handler(sender, e);
                        _instance.SwitchOwner(owner);
                    };
                }
            }
            owner.LocationChanged += handler;
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
                        VirtualRoot.ThisLocalInfo(nameof(NotiCenterWindow), $"热键Ctrl + Alt + {key.ToString()} 设置成功", OutEnum.Success);
                        return true;
                    }
                };
            }
        }

        private readonly HashSet<Window> _owners = new HashSet<Window>();
        public void SwitchOwner(Window owner) {
            if (this.Owner != owner) {
                bool isOwnerIsTopMost = owner.Topmost;
                if (isOwnerIsTopMost) {
                    owner.Topmost = false;
                }
                if (owner != null) {
                    owner.IsVisibleChanged -= Owner_IsVisibleChanged;
                    owner.StateChanged -= Owner_StateChanged;
                }
                this.Owner = owner;
                this.Owner.IsVisibleChanged += Owner_IsVisibleChanged;
                this.Owner.StateChanged += Owner_StateChanged;
                _instance.Left = owner.Left + (owner.Width - _instance.Width) / 2;
                _instance.Top = owner.Top + 4;
                if (isOwnerIsTopMost) {
                    owner.Topmost = true;
                    this.Topmost = true;
                }
            }
            if (_owners.Contains(owner)) {
                this.Owner?.Activate();
            }
            else {
                owner.Closed += Owner_Closed;
                _owners.Add(owner);
                this.Activate();
            }
        }

        private void Owner_Closed(object sender, EventArgs e) {
            _owners.Remove((Window)sender);
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
