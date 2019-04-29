using MahApps.Metro.Controls;
using NTMiner.Notifications;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace NTMiner.Views {
    public partial class MainWindow : MetroWindow, IMainWindow {
        private MainWindowViewModel Vm {
            get {
                return (MainWindowViewModel)this.DataContext;
            }
        }

        public MainWindow() {
            UIThread.StartTimer();
            InitializeComponent();
            this.StateChanged += (s, e) => {
                if (Vm.MinerProfile.IsShowInTaskbar) {
                    ShowInTaskbar = true;
                }
                else {
                    if (WindowState == WindowState.Minimized) {
                        ShowInTaskbar = false;
                    }
                    else {
                        ShowInTaskbar = true;
                    }
                }
            };
            EventHandler changeNotiCenterWindowLocation = Wpf.Util.ChangeNotiCenterWindowLocation(this);
            this.Activated += changeNotiCenterWindowLocation;
            this.LocationChanged += changeNotiCenterWindowLocation;
            if (!Windows.Role.IsAdministrator) {
                NotiCenterWindowViewModel.Instance.Manager
                    .CreateMessage()
                    .Warning("请以管理员身份运行。")
                    .WithButton("点击以管理员身份运行", button => {
                        AppContext.Current.RunAsAdministrator.Execute(null);
                    })
                    .Dismiss().WithButton("忽略", button => {
                        Vm.IsBtnRunAsAdministratorVisible = Visibility.Visible;
                    }).Queue();
            }
            if (NTMinerRoot.Instance.GpuSet.Count == 0) {
                NotiCenterWindowViewModel.Instance.Manager.ShowErrorMessage("没有矿卡或矿卡未驱动。");
            }
            NTMinerRoot.RegHotKey = (key) => {
                string message;
                if (!RegHotKey(key, out message)) {
                    NotiCenterWindowViewModel.Instance.Manager.ShowErrorMessage(message, 4);
                    return false;
                }
                else {
                    NotiCenterWindowViewModel.Instance.Manager.ShowSuccessMessage($"热键Ctrl + Alt + {key.ToString()} 设置成功");
                    return true;
                }
            };
        }

        protected override void OnClosing(CancelEventArgs e) {
            Write.ResetWriteUserLineMethod();
            UIThread.StopTimer();
            base.OnClosing(e);
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
            _thisWindowHandle = new WindowInteropHelper(this).Handle;
            HwndSource hWndSource = HwndSource.FromHwnd(_thisWindowHandle);
            if (hWndSource != null) hWndSource.AddHook(WndProc);
        }

        protected override void OnContentRendered(EventArgs e) {
            base.OnContentRendered(e);
            System.Windows.Forms.Keys hotKey = System.Windows.Forms.Keys.X;
            Enum.TryParse(NTMinerRoot.GetHotKey(), out hotKey);
            string message;
            if (!RegHotKey(hotKey, out message)) {
                NotiCenterWindowViewModel.Instance.Manager
                    .CreateMessage()
                    .Error(message)
                    .Dismiss().WithButton("忽略", null)
                    .Queue();
            }
        }

        protected override void OnClosed(EventArgs e) {
            SystemHotKey.UnRegHotKey(_thisWindowHandle, c_hotKeyId);
            base.OnClosed(e);
        }

        private const int WM_HOTKEY = 0x312;

        private const int c_hotKeyId = 1; //热键ID（自定义）
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            switch (msg) {
                case WM_HOTKEY:
                    int tmpWParam = wParam.ToInt32();
                    if (tmpWParam == c_hotKeyId) {
                        if (this.WindowState != WindowState.Minimized) {
                            this.WindowState = WindowState.Minimized;
                        }
                        else {
                            this.ShowThisWindow(isToggle: false);
                        }
                    }
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        public void ShowThisWindow(bool isToggle) {
            if (!isToggle) {
                this.Show();
            }
            ShowInTaskbar = true;
            if (WindowState == WindowState.Minimized) {
                WindowState = WindowState.Normal;
            }
            else {
                if (isToggle) {
                    WindowState = WindowState.Minimized;
                }
                else {
                    var oldState = WindowState;
                    this.WindowState = WindowState.Minimized;
                    this.WindowState = oldState;
                }
            }
            if (!isToggle) {
                this.Activate();
            }
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void BtnLeftTriangle_Click(object sender, RoutedEventArgs e) {
            BtnRightTriangle.Visibility = Visibility.Visible;
            BtnLayoutLeftRight.Visibility = Visibility.Visible;
            BtnLeftTriangle.Visibility = Visibility.Collapsed;
            BtnLayoutMain.Visibility = Visibility.Collapsed;
            MinerProfileContainerLeft.Visibility = Visibility.Collapsed;
            MinerProfileContainerLeft.Child = null;
            MinerProfileContainerRight.Child = GridMineStart;
            TabItemMinerProfile.Visibility = Visibility.Visible;
        }

        private void BtnRightTriangle_Click(object sender, RoutedEventArgs e) {
            BtnRightTriangle.Visibility = Visibility.Collapsed;
            BtnLayoutMain.Visibility = Visibility.Visible;
            BtnLeftTriangle.Visibility = Visibility.Visible;
            BtnLayoutLeftRight.Visibility = Visibility.Collapsed;
            MinerProfileContainerLeft.Visibility = Visibility.Visible;
            MinerProfileContainerRight.Child = null;
            MinerProfileContainerLeft.Child = GridMineStart;
            TabItemMinerProfile.Visibility = Visibility.Collapsed;
            if (TabItemMinerProfile.IsSelected) {
                TabItemLog.IsSelected = true;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (ConsoleUc == null) {
                return;
            }
            var selectedItem = ((TabControl)sender).SelectedItem;
            ConsoleUc.IsBuffer = selectedItem != TabItemLog;
            if (selectedItem == TabItemOuterProperty) {
                if (OuterPropertyContainer.Child == null) {
                    OuterPropertyContainer.Child = new OuterProperty();
                }
            }
            else if (selectedItem == TabItemToolbox) {
                if (ToolboxContainer.Child == null) {
                    ToolboxContainer.Child = new Toolbox();
                }
            }
            else if (selectedItem == TabItemMinerProfileOption) {
                if (MinerProfileOptionContainer.Child == null) {
                    MinerProfileOptionContainer.Child = new MinerProfileOption();
                }
            }
            else if (selectedItem == TabItemGpuOverClock) {
                if (GpuOverClockContainer.Child == null) {
                    GpuOverClockContainer.Child = new GpuOverClock();
                }
            }
            else if (selectedItem == TabItemSpeedTable) {
                if (SpeedTableContainer.Child == null) {
                    SpeedTableContainer.Child = new SpeedTable();
                }
            }
        }
    }
}
