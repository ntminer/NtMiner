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
            #endregion

            [DllImport(DllName.User32Dll, CharSet = CharSet.Auto)]
            internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport(DllName.User32Dll)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GetCursorPos(out POINT lpPoint);

            [DllImport(DllName.User32Dll)]
            internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

            [DllImport(DllName.User32Dll)]
            internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
        }

        #region 最大化窗口时避免最大化到Windows任务栏
        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam) {
            SafeNativeMethods.MINMAXINFO mmi = (SafeNativeMethods.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(SafeNativeMethods.MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = SafeNativeMethods.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero) {
                SafeNativeMethods.MONITORINFO monitorInfo = new SafeNativeMethods.MONITORINFO();
                SafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);
                SafeNativeMethods.RECT rcWorkArea = monitorInfo.rcWork;
                SafeNativeMethods.RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }
        #endregion

        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            switch (msg) {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        private bool mRestoreIfMove = false;

        private MainWindowViewModel Vm {
            get {
                return (MainWindowViewModel)this.DataContext;
            }
        }

        private const int WM_SYSCOMMAND = 0x112;
        private HwndSource hwndSource;
        private HwndSource hwndSourceBg;
        private readonly Brush _borderBrush;
        public MainWindow() {
            this.MinHeight = 430;
            this.MinWidth = 640;
            this.Width = AppStatic.MainWindowWidth;
            this.Height = AppStatic.MainWindowHeight;
#if DEBUG
            Write.Stopwatch.Start();
#endif
            this.Loaded += (sender, e) => {
                ConsoleWindow.Instance.Show();
                this.Owner = ConsoleWindow.Instance;
                hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
                hwndSourceBg = PresentationSource.FromVisual(ConsoleWindow.Instance) as HwndSource;
                hwndSource.AddHook(new HwndSourceHook(WindowProc));
                hwndSourceBg.AddHook(new HwndSourceHook(WindowProc));
            };
            InitializeComponent();

            BtnMinerProfileGrip.Visibility = Visibility.Collapsed;
            if (WpfUtil.IsInDesignMode) {
                return;
            }

            UIThread.StartTimer(); 
            bool isSplashed = false;
            ConsoleWindow.Instance.OnSplashHided = () => {
                if (!isSplashed) {
                    isSplashed = true;
                    ConsoleWindow.Instance.MouseDown += (sender, e) => {
                        MoveConsoleWindow();
                    };
                }
                MoveConsoleWindow();
            };
            _borderBrush = this.BorderBrush;
            DateTime lastGetServerMessageOn = DateTime.MinValue;
            NTMinerRoot.RefreshArgsAssembly.Invoke();
            // 切换了主界面上的Tab时
            this.MainArea.SelectionChanged += (sender, e) => {
                // 延迟创建，以加快主界面的启动
                var selectedItem = MainArea.SelectedItem;
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
                VirtualRoot.SetIsServerMessagesVisible(selectedItem == TabItemMessage);
                if (selectedItem == TabItemMessage) {
                    if (lastGetServerMessageOn.AddSeconds(10) < DateTime.Now) {
                        lastGetServerMessageOn = DateTime.Now;
                        VirtualRoot.Execute(new LoadNewServerMessageCommand());
                    }
                }
            };
            this.IsVisibleChanged += (sender, e) => {
                if (this.IsVisible) {
                    NTMinerRoot.IsUiVisible = true;
                    NTMinerRoot.MainWindowRendedOn = DateTime.Now;
                }
                else {
                    NTMinerRoot.IsUiVisible = false;
                }
                MoveConsoleWindow();
            };
            this.ConsoleRectangle.IsVisibleChanged += (sender, e) => {
                MoveConsoleWindow();
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
            NotiCenterWindow.Bind(this, ownerIsTopMost: true);
            this.LocationChanged += (sender, e) => {
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
            this.BuildEventPath<PoolDelayPickedEvent>("从内核输出中提取了矿池延时时展示到界面", LogEnum.DevConsole,
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
            this.BuildEventPath<MineStartedEvent>("开始挖矿后将清空矿池延时", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Vm.StateBarVm.PoolDelayText = string.Empty;
                        Vm.StateBarVm.DualPoolDelayText = string.Empty;
                    });
                });
            this.BuildEventPath<MineStopedEvent>("停止挖矿后将清空矿池延时", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Vm.StateBarVm.PoolDelayText = string.Empty;
                        Vm.StateBarVm.DualPoolDelayText = string.Empty;
                    });
                });
            this.BuildEventPath<Per1MinuteEvent>("挖矿中时自动切换为无界面模式 和 守护进程状态显示", LogEnum.DevConsole,
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
            var elapsedMilliseconds = Write.Stopwatch.Stop();
            Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
#endif
        }

        #region 改变下面的控制台窗口的尺寸和位置
        private void MoveConsoleWindow() {
            if (!this.IsLoaded) {
                return;
            }
            ConsoleWindow consoleWindow = ConsoleWindow.Instance;
            if (!this.IsVisible || this.WindowState == WindowState.Minimized || MainArea.SelectedItem != ConsoleTabItem) {
                consoleWindow.Hide();
                NTMinerConsole.Hide();
                return;
            }
            if (!consoleWindow.IsVisible) {
                consoleWindow.Show();
                NTMinerConsole.Show();
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
            if (ConsoleRectangle != null && ConsoleRectangle.IsVisible) {
                Point point = ConsoleRectangle.TransformToAncestor(this).Transform(new Point(0, 0));
                consoleWindow.MoveWindow(marginLeft: (int)point.X, marginTop: (int)point.Y, height: (int)ConsoleRectangle.ActualHeight);
            }
        }
        #endregion

        #region 更新状态栏展示的CPU使用率和温度
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
                    Write.Stopwatch.Start();
#endif
                    int performance = (int)Windows.Cpu.Instance.GetPerformance();
                    // 因为初始化费时间
                    int temperature = (int)Windows.Cpu.Instance.GetTemperature();
#if DEBUG
                    var elapsedMilliseconds = Write.Stopwatch.Stop();
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.RefreshCpu");
#endif
                    this.BuildEventPath<Per1SecondEvent>("每秒钟更新CPU使用率和温度", LogEnum.None,
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
                            VirtualRoot.ThisLocalInfo(nameof(MainWindow), $"自动停止挖矿，因为 CPU 温度连续{Vm.MinerProfile.CpuGETemperatureSeconds}秒不低于{Vm.MinerProfile.CpuStopTemperature}℃", toConsole: true);
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
                                VirtualRoot.ThisLocalInfo(nameof(MainWindow), $"自动开始挖矿，因为 CPU 温度连续{Vm.MinerProfile.CpuLETemperatureSeconds}秒不高于{Vm.MinerProfile.CpuStartTemperature}℃", toConsole: true);
                                NTMinerRoot.Instance.StartMine();
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region 显示或隐藏半透明遮罩层
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
        #endregion

        #region 主界面左侧的抽屉
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

            mainLayer.ColumnDefinitions.Remove(MinerProfileColumn);
            MainArea.SetValue(Grid.ColumnProperty, mainLayer.ColumnDefinitions.Count - 1);
        }

        private void ToogleLeft() {
            if (BtnMinerProfileGrip.Visibility == Visibility.Collapsed) {
                HideLeft();
            }
            else {
                BtnMinerProfileGrip.Visibility = Visibility.Collapsed;
                PinRotateTransform.Angle = 0;

                if (!mainLayer.ColumnDefinitions.Contains(MinerProfileColumn)) {
                    mainLayer.ColumnDefinitions.Insert(0, MinerProfileColumn);
                }
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
        #endregion

        protected override void OnClosing(CancelEventArgs e) {
            e.Cancel = true;
            AppContext.Disable();
            this.Hide();
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
            if (MainArea.SelectedItem == TabItemSpeedTable) {
                speedTableUc.ShowOrHideOverClock(isShow: speedTableUc.IsOverClockVisible == Visibility.Collapsed);
            }
            else {
                speedTableUc.ShowOrHideOverClock(isShow: true);
            }
            MainArea.SelectedItem = TabItemSpeedTable;
        }

        private SpeedTable _speedTable;
        private SpeedTable GetSpeedTable() {
            if (_speedTable == null) {
                _speedTable = new SpeedTable();
            }
            return _speedTable;
        }

        #region 拖动窗口边缘改变窗口尺寸
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

        private void ResizeWindow(SafeNativeMethods.ResizeDirection direction) {
            SafeNativeMethods.SendMessage(hwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }
        
        private void ResizeWindow(object sender) {
            Rectangle clickedRectangle = sender as Rectangle;

            switch (clickedRectangle.Name) {
                case nameof(this.top):
                    clickedRectangle.Cursor = Cursors.SizeNS;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.Top);
                    break;
                case nameof(this.bottom):
                    clickedRectangle.Cursor = Cursors.SizeNS;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.Bottom);
                    break;
                case nameof(this.left):
                    clickedRectangle.Cursor = Cursors.SizeWE;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.Left);
                    break;
                case nameof(this.right):
                    clickedRectangle.Cursor = Cursors.SizeWE;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.Right);
                    break;
                case nameof(this.topLeft):
                    clickedRectangle.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.TopLeft);
                    break;
                case nameof(this.topRight):
                    clickedRectangle.Cursor = Cursors.SizeNESW;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.TopRight);
                    break;
                case nameof(this.bottomLeft):
                    clickedRectangle.Cursor = Cursors.SizeNESW;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.BottomLeft);
                    break;
                case nameof(this.bottomRight):
                    clickedRectangle.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.BottomRight);
                    break;
                default:
                    break;
            }
        }

        private void DisplayResizeCursor(object sender) {
            Rectangle clickedRectangle = sender as Rectangle;

            switch (clickedRectangle.Name) {
                case nameof(this.top):
                    clickedRectangle.Cursor = Cursors.SizeNS;
                    break;
                case nameof(this.bottom):
                    clickedRectangle.Cursor = Cursors.SizeNS;
                    break;
                case nameof(this.left):
                    clickedRectangle.Cursor = Cursors.SizeWE;
                    break;
                case nameof(this.right):
                    clickedRectangle.Cursor = Cursors.SizeWE;
                    break;
                case nameof(this.topLeft):
                    clickedRectangle.Cursor = Cursors.SizeNWSE;
                    break;
                case nameof(this.topRight):
                    clickedRectangle.Cursor = Cursors.SizeNESW;
                    break;
                case nameof(this.bottomLeft):
                    clickedRectangle.Cursor = Cursors.SizeNESW;
                    break;
                case nameof(this.bottomRight):
                    clickedRectangle.Cursor = Cursors.SizeNWSE;
                    break;
                default:
                    break;
            }
        }
        #endregion

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
    }
}
