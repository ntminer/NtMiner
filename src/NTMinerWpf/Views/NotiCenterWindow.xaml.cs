using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class NotiCenterWindow : Window {
        private static NotiCenterWindow _instance = null;
        public static NotiCenterWindow Instance {
            get { return _instance; }
        }

        public static void ShowWindow() {
            if (_instance == null) {
                _instance = new NotiCenterWindow();
            }
            _instance.Show();
        }

        #region 将通知窗口切到活动窗口上面去
        /// <summary>
        /// 将通知窗口切到活动窗口上面去
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="ownerIsTopMost"></param>
        /// <param name="isNoOtherWindow">如果没有其它窗口就不需要响应窗口激活和非激活状态变更事件了</param>
        public static void Bind(Window owner, bool ownerIsTopMost = false, bool isNoOtherWindow = false) {
            if (_instance == null) {
                return;
            }
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

        private static void OnLocationChanged(object sender, EventArgs e) {
            if (_instance == null) {
                return;
            }
            Window owner = (Window)sender;
            _instance.Left = owner.Left + (owner.Width - _instance.Width) / 2;
            _instance.Top = owner.Top + 4;
        }

        private static void Owner_Deactivated(object sender, EventArgs e) {
            Window owner = (Window)sender;
            // 解决当主界面上方出现popup层时主窗口下面的控制台窗口可能会被windows绘制到上面的BUG
            if (owner.Topmost) {
                owner.Topmost = false;
            }
        }

        private static void Owner_Activated(object sender, EventArgs e) {
            Window owner = (Window)sender;
            OnLocationChanged(sender, e);
            SwitchOwner(owner);
        }

        private static void TopMostOwner_Activated(object sender, EventArgs e) {
            Window owner = (Window)sender;
            // 解决当主界面上方出现popup层时主窗口下面的控制台窗口可能会被windows绘制到上面的BUG
            if (!owner.Topmost) {
                owner.Topmost = true;
            }
            Owner_Activated(sender, e);
        }

        private static readonly HashSet<Window> _owners = new HashSet<Window>();
        public static void SwitchOwner(Window owner) {
            if (_instance == null) {
                return;
            }
            if (_instance.Owner != owner) {
                bool isOwnerIsTopMost = owner.Topmost;
                if (isOwnerIsTopMost) {
                    owner.Topmost = false;
                }
                if (owner.Owner == _instance) {
                    owner.Owner = _instance.Owner;
                }
                _instance.Owner = owner;
                if (isOwnerIsTopMost) {
                    owner.Topmost = true;
                    _instance.Topmost = true;
                }
            }
            if (_owners.Contains(owner)) {
                if (owner.IsEnabled) {
                    _instance.Owner?.Activate();
                }
                else {
                    _instance.Activate();
                }
            }
            else {
                owner.Closed += Owner_Closed;
                owner.IsVisibleChanged += Owner_IsVisibleChanged;
                owner.StateChanged += Owner_StateChanged;
                _owners.Add(owner);
                _instance.Activate();
            }
        }

        private static void Owner_Closed(object sender, EventArgs e) {
            _owners.Remove((Window)sender);
        }

        private static void Owner_StateChanged(object sender, EventArgs e) {
            if (_instance == null) {
                return;
            }
            Window owner = (Window)sender;
            if (_instance.Owner == owner && owner.WindowState == WindowState.Minimized) {
                _instance.Owner = null;
                if (!_instance.IsVisible) {
                    _instance.Show();
                }
            }
        }

        private static void Owner_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (_instance == null) {
                return;
            }
            Window owner = (Window)sender;
            if (_instance.Owner == owner && !owner.IsVisible) {
                _instance.Owner = null;
            }
        }
        #endregion

        public NotiCenterWindowViewModel Vm {
            get { return NotiCenterWindowViewModel.Instance; }
        }

        private NotiCenterWindow() {
            this.DataContext = Vm;
            InitializeComponent();
            if (WpfUtil.IsInDesignMode) {
                return;
            }
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
