using Microsoft.Win32;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace NTMiner.Views {
    public partial class MainWindow : Window, IMaskWindow {
        private bool mRestoreIfMove = false;

        public MainWindowViewModel Vm { get; private set; }

        private HwndSource hwndSource;
        private readonly GridLength _leftDrawerGripWidth;
        private readonly Brush _btnOverClockBackground;
        public MainWindow() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            if (!NTMinerConsole.IsEnabled) {
                NTMinerConsole.Enable();
            }
            this.Vm = new MainWindowViewModel();
            this.DataContext = Vm;
            this.MinHeight = 430;
            this.MinWidth = 640;
            this.Width = AppRoot.MainWindowWidth;
            this.Height = AppRoot.MainWindowHeight;
#if DEBUG
            NTStopwatch.Start();
#endif
            ConsoleWindow.Instance.MouseDown += (sender, e) => {
                MoveConsoleWindow();
            };
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            this.Loaded += (sender, e) => {
                ConsoleTabItemTopBorder.Margin = new Thickness(0, ConsoleTabItem.ActualHeight - 1, 0, 0);
                MoveConsoleWindow();
                hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
                hwndSource.AddHook(new HwndSourceHook(Win32Proc.WindowProc));
            };
            InitializeComponent();
            _leftDrawerGripWidth = LeftDrawerGrip.Width;
            _btnOverClockBackground = BtnOverClock.Background;
            // 下面几行是为了看见设计视图
            this.ResizeCursors.Visibility = Visibility.Visible;
            this.HideLeftDrawerGrid();
            // 上面几行是为了看见设计视图

            DateTime lastGetServerMessageOn = DateTime.MinValue;
            // 切换了主界面上的Tab时
            this.MainTabControl.SelectionChanged += (sender, e) => {
                // 延迟创建，以加快主界面的启动
                #region
                var selectedItem = MainTabControl.SelectedItem;
                if (selectedItem == TabItemSpeedTable) {
                    if (SpeedTableContainer.Child == null) {
                        SpeedTableContainer.Child = GetSpeedTable();
                    }
                }
                else if (selectedItem == TabItemMessage) {
                    if (MessagesContainer.Child == null) {
                        MessagesContainer.Child = new Messages();
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
                RpcRoot.SetIsServerMessagesVisible(selectedItem == TabItemMessage);
                if (selectedItem == TabItemMessage) {
                    if (lastGetServerMessageOn.AddSeconds(10) < DateTime.Now) {
                        lastGetServerMessageOn = DateTime.Now;
                        VirtualRoot.Execute(new LoadNewServerMessageCommand());
                    }
                }
                if (selectedItem == ConsoleTabItem) {
                    ConsoleTabItemTopBorder.Visibility = Visibility.Visible;
                }
                else {
                    ConsoleTabItemTopBorder.Visibility = Visibility.Collapsed;
                }
                #endregion
            };
            this.IsVisibleChanged += (sender, e) => {
                #region
                if (this.IsVisible) {
                    NTMinerContext.IsUiVisible = true;
                }
                else {
                    NTMinerContext.IsUiVisible = false;
                }
                MoveConsoleWindow();
                #endregion
            };
            this.StateChanged += (s, e) => {
                #region
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
                if (WindowState == WindowState.Maximized) {
                    ResizeCursors.Visibility = Visibility.Collapsed;
                }
                else {
                    ResizeCursors.Visibility = Visibility.Visible;
                }
                MoveConsoleWindow();
                #endregion
            };
            this.ConsoleRectangle.IsVisibleChanged += (sender, e) => {
                MoveConsoleWindow();
            };
            this.ConsoleRectangle.SizeChanged += (s, e) => {
                MoveConsoleWindow();
            };
            if (this.Width < 860) {
                NTMinerConsole.UserWarn("左侧面板已折叠，可点击侧边的'开始挖矿'按钮展开。");
            }
            this.SizeChanged += (s, e) => {
                #region
                if (this.Width < 860) {
                    this.CloseLeftDrawer();
                    this.BtnAboutNTMiner.Visibility = Visibility.Collapsed;
                }
                else {
                    this.OpenLeftDrawer();
                    this.BtnAboutNTMiner.Visibility = Visibility.Visible;
                }
                if (!this.ConsoleRectangle.IsVisible) {
                    if (e.WidthChanged) {
                        ConsoleWindow.Instance.Width = e.NewSize.Width;
                    }
                    if (e.HeightChanged) {
                        ConsoleWindow.Instance.Height = e.NewSize.Height;
                    }
                }
                #endregion
            };
            NotiCenterWindow.Bind(this, ownerIsTopmost: true);
            this.LocationChanged += (sender, e) => {
                MoveConsoleWindow();
            };
            VirtualRoot.BuildCmdPath<TopmostCommand>(path: message => {
                UIThread.Execute(() => {
                    if (!this.Topmost) {
                        this.Topmost = true;
                    }
                });
            }, this.GetType());
            VirtualRoot.BuildCmdPath<UnTopmostCommand>(path: message => {
                UIThread.Execute(() => {
                    if (this.Topmost) {
                        this.Topmost = false;
                    }
                });
            }, this.GetType());
            VirtualRoot.BuildCmdPath<CloseMainWindowCommand>(path: message => {
                UIThread.Execute(() => {
                    if (message.IsAutoNoUi) {
                        SwitchToNoUi();
                    }
                    else {
                        this.Close();
                    }
                });
            }, location: this.GetType());
            this.BuildEventPath<Per1MinuteEvent>("挖矿中时自动切换为无界面模式", LogEnum.DevConsole,
                path: message => {
                    if (NTMinerContext.IsUiVisible && NTMinerContext.Instance.MinerProfile.IsAutoNoUi && NTMinerContext.Instance.IsMining) {
                        if (NTMinerContext.MainWindowRendedOn.AddMinutes(NTMinerContext.Instance.MinerProfile.AutoNoUiMinutes) < message.BornOn) {
                            VirtualRoot.ThisLocalInfo(nameof(MainWindow), $"挖矿中界面展示{NTMinerContext.Instance.MinerProfile.AutoNoUiMinutes}分钟后自动切换为无界面模式，可在选项页调整配置");
                            VirtualRoot.Execute(new CloseMainWindowCommand(isAutoNoUi: true));
                        }
                    }
                }, location: this.GetType());
#if DEBUG
            var elapsedMilliseconds = NTStopwatch.Stop();
            if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                NTMinerConsole.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
            }
#endif
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e) {
            MoveConsoleWindow();
        }

        #region 改变下面的控制台窗口的尺寸和位置
        private void MoveConsoleWindow() {
            if (!this.IsLoaded) {
                return;
            }
            ConsoleWindow consoleWindow = ConsoleWindow.Instance;
            if (!this.IsVisible || this.WindowState == WindowState.Minimized) {
                consoleWindow.Hide();
                return;
            }
            if (!consoleWindow.IsVisible) {
                consoleWindow.Show();
            }
            if (consoleWindow.WindowState != this.WindowState) {
                consoleWindow.WindowState = this.WindowState;
            }
            // -2 -1是因为主窗口有圆角，但下层的控制台窗口不能透明所以不能圆角，把下层的控制台窗口的宽高缩小一点点从而避免看见下层控制台窗口的棱角
            if (consoleWindow.Width != this.Width - 2) {
                consoleWindow.Width = this.Width - 2;
            }
            if (consoleWindow.Height != this.Height - 2) {
                consoleWindow.Height = this.Height - 2;
            }
            if (this.WindowState == WindowState.Normal) {
                if (consoleWindow.Left != this.Left + 1) {
                    consoleWindow.Left = this.Left + 1;
                }
                if (consoleWindow.Top != this.Top + 1) {
                    consoleWindow.Top = this.Top + 1;
                }
            }
            if (ConsoleRectangle != null && ConsoleRectangle.IsVisible) {
                Point point = ConsoleRectangle.TransformToAncestor(this).Transform(new Point(0, 0));
                const int paddingLeft = 4;
                const int paddingRight = 5;
                int marginLeft = paddingLeft + (int)point.X;
                int width = (int)this.ActualWidth - marginLeft - paddingRight;
                consoleWindow.MoveWindow(marginLeft: marginLeft, marginTop: (int)point.Y, width, height: (int)ConsoleRectangle.ActualHeight);
            }
        }
        #endregion

        #region 显示或隐藏半透明遮罩层
        // 因为挖矿端主界面是透明的，遮罩方法和普通窗口不同，如果按照通用的方法遮罩的话会导致能透过窗口看见windows桌面或者下面的窗口。
        public void ShowMask() {
            MaskLayer.Visibility = Visibility.Visible;
        }

        public void HideMask() {
            MaskLayer.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region 主界面左侧的抽屉
        // 点击pin按钮
        public void BtnLeftDrawerPin_Click(object sender, RoutedEventArgs e) {
            if (LeftDrawerGrip.Width != _leftDrawerGripWidth) {
                CloseLeftDrawer();
            }
            else {
                OpenLeftDrawer();
            }
        }

        // 点击x按钮
        private void BtnLeftDrawerClose_Click(object sender, RoutedEventArgs e) {
            CloseLeftDrawer();
        }

        private void BtnLeftDrawerGrip_Click(object sender, RoutedEventArgs e) {
            if (leftDrawer.Visibility == Visibility.Collapsed) {
                leftDrawer.Visibility = Visibility.Visible;
            }
            else {
                leftDrawer.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnLeftDrawerGrip_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            OpenLeftDrawer();
        }

        // 打开左侧抽屉
        private void CloseLeftDrawer() {
            if (leftDrawer.Visibility == Visibility.Collapsed) {
                return;
            }
            leftDrawer.Visibility = Visibility.Collapsed;
            this.ShowLeftDrawerGrid();
            PinRotateTransform.Angle = 90;

            mainLayer.ColumnDefinitions.Remove(MinerProfileColumn);
            MainTabControl.SetValue(Grid.ColumnProperty, mainLayer.ColumnDefinitions.Count - 1);
        }

        private void ShowLeftDrawerGrid() {
            LeftDrawerGrip.Width = _leftDrawerGripWidth;
        }

        // 关闭左侧抽屉
        private void OpenLeftDrawer() {
            if (LeftDrawerGrip.Width != _leftDrawerGripWidth) {
                return;
            }
            leftDrawer.Visibility = Visibility.Visible;
            this.HideLeftDrawerGrid();
            PinRotateTransform.Angle = 0;

            if (!mainLayer.ColumnDefinitions.Contains(MinerProfileColumn)) {
                mainLayer.ColumnDefinitions.Insert(0, MinerProfileColumn);
            }
            MainTabControl.SetValue(Grid.ColumnProperty, mainLayer.ColumnDefinitions.Count - 1);
        }

        private void HideLeftDrawerGrid() {
            LeftDrawerGrip.Width = new GridLength(0);
        }
        #endregion

        protected override void OnClosing(CancelEventArgs e) {
            if (NTMinerContext.Instance.MinerProfile.IsCloseMeanExit) {
                VirtualRoot.Execute(new CloseNTMinerCommand("手动操作，关闭主界面意为退出"));
            }
            else {
                e.Cancel = true;
                SwitchToNoUi();
            }
        }

        private void SwitchToNoUi() {
            AppRoot.Disable();
            this.Hide();
            VirtualRoot.Out.ShowSuccess("已切换为无界面模式运行");
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void BtnOverClockVisible_Click(object sender, RoutedEventArgs e) {
            var speedTableUc = this.GetSpeedTable();
            if (MainTabControl.SelectedItem == TabItemSpeedTable) {
                speedTableUc.ShowOrHideOverClock(isShow: speedTableUc.IsOverClockVisible == Visibility.Collapsed);
            }
            else {
                speedTableUc.ShowOrHideOverClock(isShow: true);
            }
            MainTabControl.SelectedItem = TabItemSpeedTable;
            IconOverClockEyeClosed.Visibility = speedTableUc.IsOverClockVisible == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            if (IconOverClockEyeClosed.Visibility == Visibility.Visible) {
                BtnOverClock.Background = _btnOverClockBackground;
            }
            else {
                BtnOverClock.Background = WpfUtil.WhiteBrush;
            }
        }

        private void BtnOuterUserShowOption_Click(object sender, RoutedEventArgs e) {
            MainTabControl.SelectedItem = TabItemMinerProfileOption;
            ((MinerProfileOption)MinerProfileOptionContainer.Child).HighlightOuterUser();
        }

        private void BtnAutomationShowOption_Click(object sender, RoutedEventArgs e) {
            MainTabControl.SelectedItem = TabItemMinerProfileOption;
            ((MinerProfileOption)MinerProfileOptionContainer.Child).HighlightAutomation();
        }

        private SpeedTable _speedTable;
        private SpeedTable GetSpeedTable() {
            if (_speedTable == null) {
                _speedTable = new SpeedTable();
            }
            return _speedTable;
        }

        private void SwitchWindowState() {
            switch (WindowState) {
                case WindowState.Normal:
                    Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(this);
                    break;
                case WindowState.Maximized:
                    Microsoft.Windows.Shell.SystemCommands.RestoreWindow(this);
                    break;
            }
        }

        #region 拖动窗口头部
        private void RctHeader_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                SwitchWindowState();
                return;
            }

            else if (WindowState == WindowState.Maximized) {
                mRestoreIfMove = true;
                return;
            }

            DragMove();
        }

        private void RctHeader_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            mRestoreIfMove = false;
        }

        private void RctHeader_PreviewMouseMove(object sender, MouseEventArgs e) {
            if (mRestoreIfMove && e.LeftButton == MouseButtonState.Pressed) {
                mRestoreIfMove = false;

                double percentHorizontal = e.GetPosition(this).X / ActualWidth;
                double targetHorizontal = RestoreBounds.Width * percentHorizontal;

                double percentVertical = e.GetPosition(this).Y / ActualHeight;
                double targetVertical = RestoreBounds.Height * percentVertical;

                WindowState = WindowState.Normal;

                SafeNativeMethods.GetCursorPos(out SafeNativeMethods.POINT lMousePosition);

                Left = lMousePosition.X - targetHorizontal;
                Top = lMousePosition.Y - targetVertical;

                DragMove();
            }
        }
        #endregion

        private void Menu_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            VirtualRoot.Execute(new TopmostCommand());
        }

        private void Menu_PreviewMouseUp(object sender, MouseButtonEventArgs e) {
            VirtualRoot.Execute(new UnTopmostCommand());
        }
    }
}
