using NTMiner.Core;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class MainWindow : Window {
        private readonly ColumnDefinition _column1CloneForLayer0 = new ColumnDefinition {
            SharedSizeGroup = "column1",
            Width = new GridLength(332)
        };

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
            NTMinerRoot.RefreshArgsAssembly.Invoke();
            if (Design.IsInDesignMode) {
                return;
            }
            ConsoleWindow.Instance.Show();
            this.Owner = ConsoleWindow.Instance;
            ToogleLeft();
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
                if (WindowState == WindowState.Minimized) {
                    ConsoleWindow.Instance.Hide();
                }
                else {
                    ConsoleWindow.Instance.Show();
                    MoveConsoleWindow();
                }
            };
            this.ConsoleRectangle.SizeChanged += ConsoleRectangle_SizeChanged;
            this.ConsoleRectangle.IsVisibleChanged += ConsoleRectangle_IsVisibleChanged;
            EventHandler changeNotiCenterWindowLocation = NotiCenterWindow.CreateNotiCenterWindowLocationManager(this);
            this.Activated += changeNotiCenterWindowLocation;
            this.LocationChanged += (sender, e)=> {
                changeNotiCenterWindowLocation(sender, e);
                MoveConsoleWindow();
            };
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

        public void BtnMinerProfilePin_Click(object sender, RoutedEventArgs e) {
            ToogleLeft();
        }

        private void ToogleLeft() {
            if (BtnMinerProfileVisible.Visibility == Visibility.Collapsed) {
                layer1.Visibility = Visibility.Collapsed;
                BtnMinerProfileVisible.Visibility = Visibility.Visible;
                PinRotateTransform.Angle = 90;

                layer0.ColumnDefinitions.Remove(_column1CloneForLayer0);
                MainArea.SetValue(Grid.ColumnProperty, layer0.ColumnDefinitions.Count - 1);
            }
            else {
                BtnMinerProfileVisible.Visibility = Visibility.Collapsed;
                PinRotateTransform.Angle = 0;

                layer0.ColumnDefinitions.Insert(0, _column1CloneForLayer0);
                MainArea.SetValue(Grid.ColumnProperty, layer0.ColumnDefinitions.Count - 1);
            }
        }

        private void BtnMinerProfileVisible_Click(object sender, RoutedEventArgs e) {
            if (layer1.Visibility == Visibility.Collapsed) {
                layer1.Visibility = Visibility.Visible;
            }
            else {
                layer1.Visibility = Visibility.Collapsed;
            }
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
            var speedTableUc = this.SpeedTable;
            if (MainArea.SelectedItem == TabItemSpeedTable) {
                speedTableUc.ShowOrHideOverClock(isShow: speedTableUc.IsOverClockVisible == Visibility.Collapsed);
            }
            else {
                speedTableUc.ShowOrHideOverClock(isShow: true);
            }
            MainArea.SelectedItem = TabItemSpeedTable;
        }

        private void ConsoleRectangle_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (ConsoleRectangle.IsVisible) {
                MoveConsoleWindow();
            }
        }

        private void ConsoleRectangle_SizeChanged(object sender, SizeChangedEventArgs e) {
            MoveConsoleWindow();
        }

        private void MoveConsoleWindow() {
            if (ConsoleRectangle == null || !ConsoleRectangle.IsVisible || ConsoleRectangle.ActualWidth == 0) {
                ConsoleWindow.Instance.Hide();
                return;
            }
            Point point = ConsoleRectangle.TransformToAncestor(this).Transform(new Point(0, 0));
            var width = ConsoleRectangle.ActualWidth - 1;
            var height = ConsoleRectangle.ActualHeight;
            ConsoleWindow.Instance.Width = width;
            ConsoleWindow.Instance.Height = height;
            if (this.WindowState == WindowState.Maximized) {
                ConsoleWindow.Instance.Left = point.X;
                ConsoleWindow.Instance.Top = point.Y;
            }
            else {
                ConsoleWindow.Instance.Left = point.X + this.Left;
                ConsoleWindow.Instance.Top = point.Y + this.Top;
            }
            ConsoleWindow.Instance.Show();
        }
    }
}
