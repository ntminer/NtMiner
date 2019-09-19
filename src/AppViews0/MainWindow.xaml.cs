using NTMiner.Core;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace NTMiner.Views {
    public partial class MainWindow : Window, IMaskWindow {
        private bool mRestoreIfMove = false;
        private readonly ColumnDefinition _mainLayerColumn0 = new ColumnDefinition {
            SharedSizeGroup = "column0",
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
                MoveConsoleWindow();
            };
            this.SizeChanged += (s, e) => {
                if (!ConsoleRectangle.IsVisible) {
                    ConsoleWindow.Instance.Hide();
                }
            };
            this.ConsoleRectangle.SizeChanged += (s, e) => {
                MoveConsoleWindow();
            };
            bool isFirst = true;
            this.ConsoleRectangle.IsVisibleChanged += (s, e) => {
                if (ConsoleRectangle.IsVisible) {
                    if (isFirst) {
                        isFirst = false;
                    }
                    else {
                        MoveConsoleWindow();
                    }
                }
            };
            this.IsVisibleChanged += (s, e) => {
                if (!this.IsVisible) {
                    ConsoleWindow.Instance.Hide();
                }
            };
            EventHandler changeNotiCenterWindowLocation = NotiCenterWindow.CreateNotiCenterWindowLocationManager(this);
            this.Activated += changeNotiCenterWindowLocation;
            this.LocationChanged += (sender, e) => {
                changeNotiCenterWindowLocation(sender, e);
                MoveConsoleWindow();
            };
            if (DevMode.IsDevMode) {
                this.EventPath<ServerJsonVersionChangedEvent>("开发者模式展示ServerJsonVersion", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            Vm.ServerJsonVersion = Vm.GetServerJsonVersion();
                        });
                    });
            }
            this.EventPath<PoolDelayPickedEvent>("从内核输出中提取了矿池延时时展示到界面", LogEnum.DevConsole,
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
            this.EventPath<MineStartedEvent>("开始挖矿后将清空矿池延时", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Vm.StateBarVm.PoolDelayText = string.Empty;
                        Vm.StateBarVm.DualPoolDelayText = string.Empty;
                    });
                });
            this.EventPath<MineStopedEvent>("停止挖矿后将清空矿池延时", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Vm.StateBarVm.PoolDelayText = string.Empty;
                        Vm.StateBarVm.DualPoolDelayText = string.Empty;
                    });
                });
            this.EventPath<Per1MinuteEvent>("挖矿中时自动切换为无界面模式 和 守护进程状态显示", LogEnum.DevConsole,
                action: message => {
                    if (NTMinerRoot.IsUiVisible && NTMinerRoot.Instance.MinerProfile.IsAutoNoUi && NTMinerRoot.Instance.IsMining) {
                        if (NTMinerRoot.MainWindowRendedOn.AddMinutes(NTMinerRoot.Instance.MinerProfile.AutoNoUiMinutes) < message.Timestamp) {
                            VirtualRoot.Execute(new CloseMainWindowCommand($"界面展示{NTMinerRoot.Instance.MinerProfile.AutoNoUiMinutes}分钟后自动切换为无界面模式，可在选项页调整配置"));
                        }
                    }
                    Vm.RefreshDaemonStateBrush();
                });
#if DEBUG
            Write.DevTimeSpan($"耗时{Write.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
        }

        public void ShowMask() {
            MaskLayer.Visibility = Visibility.Visible;
        }

        public void HideMask() {
            MaskLayer.Visibility = Visibility.Collapsed;
        }

        public void BtnMinerProfilePin_Click(object sender, RoutedEventArgs e) {
            ToogleLeft();
        }

        private void BtnMinerProfileClose_Click(object sender, RoutedEventArgs e) {
            HideLeft();
        }

        private void HideLeft() {
            minerProfileLayer.Visibility = Visibility.Collapsed;
            BtnMinerProfileGrip.Visibility = Visibility.Visible;
            PinRotateTransform.Angle = 90;

            mainLayer.ColumnDefinitions.Remove(_mainLayerColumn0);
            MainArea.SetValue(Grid.ColumnProperty, mainLayer.ColumnDefinitions.Count - 1);
        }

        private void ToogleLeft() {
            if (BtnMinerProfileGrip.Visibility == Visibility.Collapsed) {
                HideLeft();
            }
            else {
                BtnMinerProfileGrip.Visibility = Visibility.Collapsed;
                PinRotateTransform.Angle = 0;

                mainLayer.ColumnDefinitions.Insert(0, _mainLayerColumn0);
                MainArea.SetValue(Grid.ColumnProperty, mainLayer.ColumnDefinitions.Count - 1);
            }
        }

        private void BtnMinerProfileGrip_Click(object sender, RoutedEventArgs e) {
            if (minerProfileLayer.Visibility == Visibility.Collapsed) {
                minerProfileLayer.Visibility = Visibility.Visible;
            }
            else {
                minerProfileLayer.Visibility = Visibility.Collapsed;
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

        private void MoveConsoleWindow() {
            if (this.WindowState == WindowState.Minimized || ConsoleRectangle == null || !ConsoleRectangle.IsVisible || ConsoleRectangle.ActualWidth == 0) {
                ConsoleWindow.Instance.Hide();
                return;
            }
            if (ConsoleWindow.Instance.WindowState != this.WindowState) {
                ConsoleWindow.Instance.WindowState = this.WindowState;
            }
            if (this.WindowState == WindowState.Normal) {
                if (ConsoleWindow.Instance.Left != this.Left) {
                    ConsoleWindow.Instance.Left = this.Left;
                }
                if (ConsoleWindow.Instance.Top != this.Top) {
                    ConsoleWindow.Instance.Top = this.Top;
                }
            }
            if (!ConsoleWindow.Instance.IsVisible) {
                ConsoleWindow.Instance.Show();
            }
            Point point = ConsoleRectangle.TransformToAncestor(this).Transform(new Point(0, 0));
            if (ConsoleWindow.Instance.Width != this.ActualWidth) {
                ConsoleWindow.Instance.Width = this.ActualWidth;
            }
            if (ConsoleWindow.Instance.Height != this.ActualHeight) {
                ConsoleWindow.Instance.Height = this.ActualHeight;
            }
            int marginBottom = (int)StateBar.ActualHeight;
            ConsoleWindow.Instance.ReSizeConsoleWindow(marginLeft: (int)point.X, marginTop: (int)point.Y, marginBottom);
        }

        private void Window_SourceInitialized(object sender, EventArgs e) {
            IntPtr mWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(mWindowHandle).AddHook(new HwndSourceHook(WindowProc));
        }

        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            switch (msg) {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam) {
            POINT lMousePosition;
            GetCursorPos(out lMousePosition);

            IntPtr lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
            MONITORINFO lPrimaryScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false) {
                return;
            }

            IntPtr lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);

            MINMAXINFO lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            if (lPrimaryScreen.Equals(lCurrentScreen) == true) {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcWork.Right - lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcWork.Bottom - lPrimaryScreenInfo.rcWork.Top;
            }
            else {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcMonitor.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcMonitor.Right - lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcMonitor.Bottom - lPrimaryScreenInfo.rcMonitor.Top;
            }

            Marshal.StructureToPtr(lMmi, lParam, true);
        }

        private void SwitchWindowState() {
            switch (WindowState) {
                case WindowState.Normal: {
                        WindowState = WindowState.Maximized;
                        break;
                    }
                case WindowState.Maximized: {
                        WindowState = WindowState.Normal;
                        break;
                    }
            }
        }

        private void rctHeader_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                if ((ResizeMode == ResizeMode.CanResize) || (ResizeMode == ResizeMode.CanResizeWithGrip)) {
                    SwitchWindowState();
                }

                return;
            }

            else if (WindowState == WindowState.Maximized) {
                mRestoreIfMove = true;
                return;
            }

            DragMove();
        }

        private void rctHeader_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            mRestoreIfMove = false;
        }

        private void rctHeader_PreviewMouseMove(object sender, MouseEventArgs e) {
            if (mRestoreIfMove) {
                mRestoreIfMove = false;

                double percentHorizontal = e.GetPosition(this).X / ActualWidth;
                double targetHorizontal = RestoreBounds.Width * percentHorizontal;

                double percentVertical = e.GetPosition(this).Y / ActualHeight;
                double targetVertical = RestoreBounds.Height * percentVertical;

                WindowState = WindowState.Normal;

                POINT lMousePosition;
                GetCursorPos(out lMousePosition);

                Left = lMousePosition.X - targetHorizontal;
                Top = lMousePosition.Y - targetVertical;

                DragMove();
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        enum MonitorOptions : uint {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }

        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT {
            public int X;
            public int Y;

            public POINT(int x, int y) {
                this.X = x;
                this.Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom) {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }
    }
}
