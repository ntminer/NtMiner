using MahApps.Metro.Controls;
using NTMiner.Notifications;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        }

        public void ShowThisWindow(bool isToggle) {
            AppHelper.ShowWindow(this, isToggle);
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
