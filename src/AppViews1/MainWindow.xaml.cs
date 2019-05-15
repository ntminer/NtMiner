using MahApps.Metro.Controls;
using NTMiner.Core;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class MainWindow : MetroWindow {
        private MainWindowViewModel Vm {
            get {
                return (MainWindowViewModel)this.DataContext;
            }
        }

        public MainWindow() {
#if DEBUG
            VirtualRoot.Stopwatch.Restart();
#endif
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
            this.SizeChanged += (object sender, SizeChangedEventArgs e)=> {
                if (e.WidthChanged) {
                    if (e.NewSize.Width < 800) {
                        Collapse();
                    }
                    else if (e.NewSize.Width >= 800) {
                        Expand();
                    }
                }
            };
            EventHandler changeNotiCenterWindowLocation = Wpf.Util.ChangeNotiCenterWindowLocation(this);
            this.Activated += changeNotiCenterWindowLocation;
            this.LocationChanged += changeNotiCenterWindowLocation;
            if (DevMode.IsDevMode) {
                this.On<ServerJsonVersionChangedEvent>("开发者模式展示ServerJsonVersion", LogEnum.DevConsole,
                    action: message => {
                        Vm.ServerJsonVersion = Vm.GetServerJsonVersion();
                    });
            }
#if DEBUG
            Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
        }

        protected override void OnClosing(CancelEventArgs e) {
            AppContext.Disable();
            Write.SetConsoleUserLineMethod();
            base.OnClosing(e);
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
            BtnRightTriangle.Visibility = Visibility.Visible;
            BtnLayoutLeftRight.Visibility = Visibility.Visible;
            BtnLeftTriangle.Visibility = Visibility.Collapsed;
            BtnLayoutMain.Visibility = Visibility.Collapsed;
            MinerProfileContainerLeft.Visibility = Visibility.Collapsed;
            MinerProfileContainerLeft.Child = null;
            MinerProfileContainerRight.Child = GridMineStart;
            TabItemMinerProfile.Visibility = Visibility.Visible;
        }

        private void Expand() {
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

        private void NTMinerLogo_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                if (NTMinerRoot.IsBrandSpecified) {
                    return;
                }
                BrandWindow brandWindow = new BrandWindow();
                brandWindow.ShowDialog();
                e.Handled = true;
            }
        }
    }
}
