using NTMiner.Core;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NTMiner.Views {
    public partial class MainWindow : Window, IMaskWindow {
        private static class SafeNativeMethods {
            #region enum struct class
            internal enum MonitorOptions : uint {
                MONITOR_DEFAULTTONULL = 0x00000000,
                MONITOR_DEFAULTTOPRIMARY = 0x00000001,
                MONITOR_DEFAULTTONEAREST = 0x00000002
            }

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
            #endregion

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GetCursorPos(out POINT lpPoint);

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

            [DllImport("user32.dll")]
            internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
        }
        public enum ResizeDirection {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

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

        private const int WM_SYSCOMMAND = 0x112;
        private HwndSource hwndSource;
        private readonly Brush _borderBrush;
        public MainWindow() {
            this.MinHeight = 430;
            this.MinWidth = 640;
            this.Width = AppStatic.MainWindowWidth;
            this.Height = AppStatic.MainWindowHeight;
#if DEBUG
            Write.Stopwatch.Restart();
#endif
            UIThread.StartTimer();
            ConsoleWindow.Instance.OnSplashHided = MoveConsoleWindow;
            this.Owner = ConsoleWindow.Instance;
            ConsoleWindow.Instance.Activate();
            InitializeComponent();
            _borderBrush = this.BorderBrush;
            NTMinerRoot.RefreshArgsAssembly.Invoke();
            if (Design.IsInDesignMode) {
                return;
            }
            ToogleLeft();
            this.IsVisibleChanged += (object sender, DependencyPropertyChangedEventArgs e) => {
                if (this.IsVisible) {
                    NTMinerRoot.IsUiVisible = true;
                    NTMinerRoot.MainWindowRendedOn = DateTime.Now;
                }
                else {
                    NTMinerRoot.IsUiVisible = false;
                }
            };
            this.Activated += (sender, e)=> {
                NotiCenterWindow.Instance.SwitchOwner(this);
            };
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
                if (WindowState == WindowState.Maximized) {
                    ResizeCursors.Visibility = Visibility.Collapsed;
                    this.BorderBrush = WpfUtil.BlackBrush;
                }
                else {
                    ResizeCursors.Visibility = Visibility.Visible;
                    this.BorderBrush = _borderBrush;
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
            VirtualRoot.BuildCmdPath<CloseMainWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    if (NTMinerRoot.Instance.MinerProfile.IsCloseMeanExit) {
                        VirtualRoot.Execute(new CloseNTMinerCommand());
                        return;
                    }
                    this.Close();
                    VirtualRoot.Out.ShowSuccess(message.Message, "开源矿工");
                });
            });
            if (DevMode.IsDevMode) {
                this.EventPath<ServerJsonVersionChangedEvent>("开发者模式展示ServerJsonVersion", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            Vm.ServerJsonVersion = NTMinerRoot.Instance.GetServerJsonVersion();
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
            RefreshCpu();
#if DEBUG
            Write.DevTimeSpan($"耗时{Write.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
        }

        private int _cpuPerformance = 0;
        private int _cpuTemperature = 0;
        private void UpdateCpuView(int performance, int temperature) {
            if (temperature < 0) {
                temperature = 0;
            }
            if (_cpuPerformance != performance) {
                _cpuPerformance = performance;
                Vm.StateBarVm.CpuPerformanceText = performance.ToString() + " %";
            }
            if (_cpuTemperature != temperature) {
                _cpuTemperature = temperature;
                Vm.StateBarVm.CpuTemperatureText = temperature.ToString() + " ℃";
            }
        }
        private bool _isFirstRefreshCpu = true;
        private void RefreshCpu() {
            if (_isFirstRefreshCpu) {
                _isFirstRefreshCpu = false;
                Task.Factory.StartNew(() => {
#if DEBUG
                    Write.Stopwatch.Restart();
#endif
                    int performance = (int)Windows.Cpu.Instance.GetPerformance();
                    // 因为初始化费时间
                    int temperature = (int)Windows.Cpu.Instance.GetTemperature();
#if DEBUG
                    Write.DevTimeSpan($"耗时{Write.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.RefreshCpu");
#endif
                    UIThread.Execute(() => {
                        UpdateCpuView(performance, temperature);
                    });
                    this.EventPath<Per1SecondEvent>("每秒钟更新CPU使用率和温度", LogEnum.None,
                        action: message => {
                            RefreshCpu();
                        });
                });
            }
            else {
                int performance = (int)Windows.Cpu.Instance.GetPerformance();
                int temperature = (int)Windows.Cpu.Instance.GetTemperature();
                UpdateCpuView(performance, temperature);
                if (Vm.MinerProfile.IsAutoStopByCpu) {
                    if (NTMinerRoot.Instance.IsMining) {
                        Vm.MinerProfile.LowTemperatureCount = 0;
                        if (temperature >= Vm.MinerProfile.CpuStopTemperature) {
                            Vm.MinerProfile.HighTemperatureCount++;
                        }
                        else {
                            Vm.MinerProfile.HighTemperatureCount = 0;
                        }
                        if (Vm.MinerProfile.HighTemperatureCount >= Vm.MinerProfile.CpuGETemperatureSeconds) {
                            Vm.MinerProfile.HighTemperatureCount = 0;
                            NTMinerRoot.Instance.StopMineAsync(StopMineReason.HighCpuTemperature);
                            VirtualRoot.ThisWorkerMessage(nameof(MainWindow), WorkerMessageType.Info, $"自动停止挖矿，因为 CPU 温度连续{Vm.MinerProfile.CpuGETemperatureSeconds}秒不低于{Vm.MinerProfile.CpuStopTemperature}℃");
                        }
                    }
                    else {
                        Vm.MinerProfile.HighTemperatureCount = 0;
                        if (Vm.MinerProfile.IsAutoStartByCpu && NTMinerRoot.Instance.StopReason == StopMineReason.HighCpuTemperature) {
                            if (temperature <= Vm.MinerProfile.CpuStartTemperature) {
                                Vm.MinerProfile.LowTemperatureCount++;
                            }
                            else {
                                Vm.MinerProfile.LowTemperatureCount = 0;
                            }
                            if (Vm.MinerProfile.LowTemperatureCount >= Vm.MinerProfile.CpuLETemperatureSeconds) {
                                Vm.MinerProfile.LowTemperatureCount = 0;
                                VirtualRoot.ThisWorkerMessage(nameof(MainWindow), WorkerMessageType.Info, $"自动开始挖矿，因为 CPU 温度连续{Vm.MinerProfile.CpuLETemperatureSeconds}秒不高于{Vm.MinerProfile.CpuStartTemperature}℃");
                                NTMinerRoot.Instance.StartMine();
                            }
                        }
                    }
                }
            }
        }

        private void Resize(object sender, MouseButtonEventArgs e) {
            this.ResizeWindow(sender);
        }

        private void DisplayResizeCursor(object sender, MouseEventArgs e) {
            this.DisplayResizeCursor(sender);
        }

        private void ResetCursor(object sender, MouseEventArgs e) {
            if (Mouse.LeftButton != MouseButtonState.Pressed) {
                this.Cursor = Cursors.Arrow;
            }
        }

        public void ShowMask() {
            if (this.WindowState != WindowState.Maximized) {
                this.BorderBrush = WpfUtil.TransparentBrush;
            }
            MaskLayer.Visibility = Visibility.Visible;
        }

        public void HideMask() {
            this.BorderBrush = _borderBrush;
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
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
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
            ConsoleWindow consoleWindow = ConsoleWindow.Instance;
            if (!consoleWindow.IsVisible) {
                consoleWindow.Show();
            }
            if (consoleWindow.WindowState != this.WindowState) {
                consoleWindow.WindowState = this.WindowState;
            }
            if (consoleWindow.Width != this.ActualWidth) {
                consoleWindow.Width = this.ActualWidth;
            }
            if (consoleWindow.Height != this.ActualHeight) {
                consoleWindow.Height = this.ActualHeight;
            }
            if (this.WindowState == WindowState.Normal) {
                if (consoleWindow.Left != this.Left) {
                    consoleWindow.Left = this.Left;
                }
                if (consoleWindow.Top != this.Top) {
                    consoleWindow.Top = this.Top;
                }
            }
            Point point = ConsoleRectangle.TransformToAncestor(this).Transform(new Point(0, 0));
            consoleWindow.ReSizeConsoleWindow(marginLeft: (int)point.X, marginTop: (int)point.Y, (int)ConsoleRectangle.ActualHeight);
        }

        private void Window_SourceInitialized(object sender, EventArgs e) {
            hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
            hwndSource.AddHook(new HwndSourceHook(WindowProc));
        }

        private void ResizeWindow(ResizeDirection direction) {
            SafeNativeMethods.SendMessage(hwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }


        private void ResizeWindow(object sender) {
            Rectangle clickedRectangle = sender as Rectangle;

            switch (clickedRectangle.Name) {
                case "top":
                    this.Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "bottom":
                    this.Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "left":
                    this.Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "right":
                    this.Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "topLeft":
                    this.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "topRight":
                    this.Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "bottomLeft":
                    this.Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                case "bottomRight":
                    this.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
                default:
                    break;
            }
        }

        private void DisplayResizeCursor(object sender) {
            Rectangle clickedRectangle = sender as Rectangle;

            switch (clickedRectangle.Name) {
                case "top":
                    this.Cursor = Cursors.SizeNS;
                    break;
                case "bottom":
                    this.Cursor = Cursors.SizeNS;
                    break;
                case "left":
                    this.Cursor = Cursors.SizeWE;
                    break;
                case "right":
                    this.Cursor = Cursors.SizeWE;
                    break;
                case "topLeft":
                    this.Cursor = Cursors.SizeNWSE;
                    break;
                case "topRight":
                    this.Cursor = Cursors.SizeNESW;
                    break;
                case "bottomLeft":
                    this.Cursor = Cursors.SizeNESW;
                    break;
                case "bottomRight":
                    this.Cursor = Cursors.SizeNWSE;
                    break;
                default:
                    break;
            }
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
            SafeNativeMethods.GetCursorPos(out SafeNativeMethods.POINT lMousePosition);

            IntPtr lPrimaryScreen = SafeNativeMethods.MonitorFromPoint(new SafeNativeMethods.POINT(0, 0), SafeNativeMethods.MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
            SafeNativeMethods.MONITORINFO lPrimaryScreenInfo = new SafeNativeMethods.MONITORINFO();
            if (SafeNativeMethods.GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false) {
                return;
            }

            IntPtr lCurrentScreen = SafeNativeMethods.MonitorFromPoint(lMousePosition, SafeNativeMethods.MonitorOptions.MONITOR_DEFAULTTONEAREST);

            SafeNativeMethods.MINMAXINFO lMmi = (SafeNativeMethods.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(SafeNativeMethods.MINMAXINFO));

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
                        Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(this);
                        break;
                    }
                case WindowState.Maximized: {
                        Microsoft.Windows.Shell.SystemCommands.RestoreWindow(this);
                        break;
                    }
            }
        }

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
    }
}
