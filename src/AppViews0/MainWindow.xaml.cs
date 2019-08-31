using NTMiner.Core;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class MainWindow : BlankWindow {
        private MainWindowViewModel Vm {
            get {
                return (MainWindowViewModel)this.DataContext;
            }
        }

        public MainWindow() {
            this.MinHeight = 430;
            this.MinWidth = 640;
            this.Width = AppStatic.MainWindowWidth;
            this.Height = AppStatic.MainWindowHeight;
#if DEBUG
            Write.Stopwatch.Restart();
#endif
            UIThread.StartTimer();
            InitializeComponent();
            if (Design.IsInDesignMode) {
                return;
            }
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
            this.SizeChanged += (object sender, SizeChangedEventArgs e)=> {
                if (e.WidthChanged) {
                    const double width = 800;
                    if (e.NewSize.Width < width) {
                        Collapse();
                    }
                    else if (e.NewSize.Width >= width) {
                        Expand();
                    }
                }
            };
            EventHandler changeNotiCenterWindowLocation = NotiCenterWindow.CreateNotiCenterWindowLocationManager(this);
            this.Activated += changeNotiCenterWindowLocation;
            this.LocationChanged += changeNotiCenterWindowLocation;
            if (DevMode.IsDevMode) {
                this.On<ServerJsonVersionChangedEvent>("开发者模式展示ServerJsonVersion", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            Vm.ServerJsonVersion = Vm.GetServerJsonVersion();
                        });
                    });
            }
            this.On<PoolDelayPickedEvent>("从内核输出中提取了矿池延时时展示到界面", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        if (message.IsDual) {
                            Vm.StateBarVm.DualPoolDelayText = message.PoolDelayText;
                        }
                        else {
                            Vm.StateBarVm.PoolDelayText = message.PoolDelayText;
                        }
                    });
                });
            this.On<MineStartedEvent>("开始挖矿后将清空矿池延时", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Vm.StateBarVm.PoolDelayText = string.Empty;
                        Vm.StateBarVm.DualPoolDelayText = string.Empty;
                    });
                });
            this.On<MineStopedEvent>("停止挖矿后将清空矿池延时", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Vm.StateBarVm.PoolDelayText = string.Empty;
                        Vm.StateBarVm.DualPoolDelayText = string.Empty;
                    });
                });
            this.On<Per1MinuteEvent>("挖矿中时自动切换为无界面模式 和 守护进程状态显示", LogEnum.DevConsole,
                action: message => {
                    if (NTMinerRoot.IsUiVisible && NTMinerRoot.GetIsAutoNoUi() && NTMinerRoot.Instance.IsMining) {
                        if (NTMinerRoot.MainWindowRendedOn.AddMinutes(NTMinerRoot.GetAutoNoUiMinutes()) < message.Timestamp) {
                            VirtualRoot.Execute(new CloseMainWindowCommand($"界面展示{NTMinerRoot.GetAutoNoUiMinutes()}分钟后自动切换为无界面模式，可在选项页调整配置"));
                        }
                    }
                    Vm.RefreshDaemonStateBrush();
                });
#if DEBUG
            Write.DevTimeSpan($"耗时{Write.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
        }

        protected override void OnClosing(CancelEventArgs e) {
            e.Cancel = true;
            AppContext.Disable();
            Write.SetConsoleUserLineMethod();
            this.Hide();
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void BtnLeftTriangle_Click(object sender, RoutedEventArgs e) {
            Collapse();
        }

        private void BtnRightTriangle_Click(object sender, RoutedEventArgs e) {
            Expand();
        }

        private void Collapse() {
            MinerProfileContainerLeft.Visibility = Visibility.Collapsed;
            MinerProfileContainerLeft.Child = null;
            MinerProfileContainerRight.Child = GridMineStart;
            TabItemMinerProfile.Visibility = Visibility.Visible;
        }

        private void Expand() {
            MinerProfileContainerLeft.Visibility = Visibility.Visible;
            MinerProfileContainerRight.Child = null;
            MinerProfileContainerLeft.Child = GridMineStart;
            TabItemMinerProfile.Visibility = Visibility.Collapsed;
            if (TabItemMinerProfile.IsSelected) {
                TabItemLog.IsSelected = true;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var selectedItem = ((TabControl)sender).SelectedItem;
            if (selectedItem == TabItemToolbox) {
                if (ToolboxContainer.Child == null) {
                    ToolboxContainer.Child = new Toolbox();
                }
            }
            else if (selectedItem == TabItemMinerProfileOption) {
                if (MinerProfileOptionContainer.Child == null) {
                    MinerProfileOptionContainer.Child = new MinerProfileOption();
                }
            }
            NTMinerConsole.ShowWindow(NTMinerConsole.Show(), 0);
            if (selectedItem == TabItemLog) {
                TimeSpan.FromMilliseconds(200).Delay().ContinueWith(t => {
                    NTMinerConsole.ShowWindow(NTMinerConsole.Show(), 1);
                });
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void NTMinerLogo_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                if (NTMinerRoot.IsBrandSpecified) {
                    return;
                }
                BrandTag.ShowWindow();
                e.Handled = true;
            }
        }

        private void BtnOverClockVisible_Click(object sender, RoutedEventArgs e) {
            ContentControl btn = (ContentControl)sender;
            var speedTableUc = this.SpeedTable;
            if (RightTab.SelectedItem == TabItemSpeedTable) {
                speedTableUc.ShowOrHideOverClock(isShow: !Vm.IsOverClockVisible);
                Vm.IsOverClockVisible = !Vm.IsOverClockVisible;
            }
            else {
                speedTableUc.ShowOrHideOverClock(isShow: true);
                Vm.IsOverClockVisible = true;
            }
            RightTab.SelectedItem = TabItemSpeedTable;
        }
    }
}
