using NTMiner.Notifications;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace NTMiner.Views {
    public partial class NotiCenterWindow : Window {
        public static readonly NotiCenterWindow Instance = new NotiCenterWindow();

        #region 将通知窗口切到活动窗口上面去
        public void ShowWindow() {
            base.Show();
        }

        private void OnLocationChanged(object sender, EventArgs e) {
            Window owner = (Window)sender;
            Left = owner.Left + (owner.Width - Width) / 2;
            Top = owner.Top + 4;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="ownerIsTopMost"></param>
        /// <param name="isNoOtherWindow">如果没有其它窗口就不需要响应窗口激活和非激活状态变更事件了</param>
        public void Bind(Window owner, bool ownerIsTopMost = false, bool isNoOtherWindow = false) {
            if (ownerIsTopMost) {
                if (!isNoOtherWindow) {
                    owner.Activated += TopMostOwner_Activated;
                }
                if (!isNoOtherWindow) {
                    owner.Deactivated += Owner_Deactivated;
                }
            }
            else {
                if (!isNoOtherWindow) {
                    owner.Activated += Owner_Activated;
                }
            }
            owner.LocationChanged += OnLocationChanged;
        }

        private void Owner_Deactivated(object sender, EventArgs e) {
            Window owner = (Window)sender;
            // 解决当主界面上方出现popup层时主窗口下面的控制台窗口可能会被windows绘制到上面的BUG
            if (owner.Topmost) {
                owner.Topmost = false;
            }
        }

        private void Owner_Activated(object sender, EventArgs e) {
            Window owner = (Window)sender;
            OnLocationChanged(sender, e);
            SwitchOwner(owner);
        }

        private void TopMostOwner_Activated(object sender, EventArgs e) {
            Window owner = (Window)sender;
            // 解决当主界面上方出现popup层时主窗口下面的控制台窗口可能会被windows绘制到上面的BUG
            if (!owner.Topmost) {
                owner.Topmost = true;
            }
            Owner_Activated(sender, e);
        }

        private readonly HashSet<Window> _owners = new HashSet<Window>();
        public void SwitchOwner(Window owner) {
            if (this.Owner != owner) {
                bool isOwnerIsTopMost = owner.Topmost;
                if (isOwnerIsTopMost) {
                    owner.Topmost = false;
                }
                this.Owner = owner;
                if (isOwnerIsTopMost) {
                    owner.Topmost = true;
                    this.Topmost = true;
                }
            }
            if (_owners.Contains(owner)) {
                if (owner.IsEnabled) {
                    this.Owner?.Activate();
                }
                else {
                    this.Activate();
                }
            }
            else {
                owner.Closed += Owner_Closed;
                owner.IsVisibleChanged += Owner_IsVisibleChanged;
                owner.StateChanged += Owner_StateChanged;
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
        #endregion

        public NotiCenterWindowViewModel Vm {
            get { return NotiCenterWindowViewModel.Instance; }
        }

        private NotiCenterWindow() {
            this.DataContext = Vm;
            InitializeComponent();
            if (NotiCenterWindowViewModel.IsHotKeyEnabled) {
                HotKeyUtil.RegHotKey = (key) => {
                    if (!RegHotKey(key, out string message)) {
                        VirtualRoot.Out.ShowError(message, autoHideSeconds: 4);
                        return false;
                    }
                    else {
                        VirtualRoot.ThisLocalInfo(nameof(NotiCenterWindow), $"热键Ctrl + Alt + {key.ToString()} 设置成功", OutEnum.Success);
                        return true;
                    }
                };
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
                        .Warning("失败", message)
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
