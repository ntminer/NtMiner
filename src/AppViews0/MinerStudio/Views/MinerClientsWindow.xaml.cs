using NTMiner.MinerStudio.Views.Ucs;
using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using NTMiner.Ws;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace NTMiner.MinerStudio.Views {
    public partial class MinerClientsWindow : Window, IMaskWindow {
        private static MinerClientsWindow _instance = null;
        public static MinerClientsWindow ShowWindow(bool isToggle) {
            if (_instance == null) {
                _instance = new MinerClientsWindow();
                _instance.Show();
            }
            else {
                if (_instance.WindowState == WindowState.Minimized) {
                    _instance.WindowState = WindowState.Normal;
                }
                else if (!_instance.IsVisible) {
                    _instance.Show();
                    _instance.Activate();
                }
                else if (isToggle) {
                    _instance.Hide();
                }
                else {
                    _instance.Show();
                    _instance.Activate();
                }
            }
            return _instance;
        }

        public MinerClientsWindowViewModel Vm {
            get {
                return MinerStudioRoot.MinerClientsWindowVm;
            }
        }

        private bool mRestoreIfMove = false;
        private HwndSource hwndSource;
        private MinerClientsWindow() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            Width = SystemParameters.FullPrimaryScreenWidth * 0.95;
            Height = SystemParameters.FullPrimaryScreenHeight * 0.95;
            this.DataContext = Vm;
            ConsoleWindow.Instance.Show();
            ConsoleWindow.Instance.MouseDown += (sender, e) => {
                MoveConsoleWindow();
            };
            this.Owner = ConsoleWindow.Instance;
            this.Loaded += (sender, e) => {
                MoveConsoleWindow();
                hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
                hwndSource.AddHook(new HwndSourceHook(Win32Proc.WindowProc));
                this.WindowState = WindowState.Maximized;
            };
            InitializeComponent();
            // 这一行是为了看见设计视图
            this.ResizeCursors.Visibility = Visibility.Visible;
            this.TbUcName.Text = nameof(MinerClientsWindow);
            DateTime lastGetServerMessageOn = DateTime.MinValue;
            this.MinerStudioTabControl.SelectionChanged += (sender, e) => {
                // 延迟创建，以加快主界面的启动
                #region
                var selectedItem = MinerStudioTabControl.SelectedItem;
                if (selectedItem == MinerStudioTabItemMessage) {
                    if (MinerStudioMessagesContainer.Child == null) {
                        MinerStudioMessagesContainer.Child = new Messages();
                    }
                    if (lastGetServerMessageOn.AddSeconds(10) < DateTime.Now) {
                        lastGetServerMessageOn = DateTime.Now;
                        VirtualRoot.Execute(new LoadNewServerMessageCommand());
                    }
                }
                JsonRpcRoot.SetIsServerMessagesVisible(selectedItem == MinerStudioTabItemMessage);
                #endregion
            };
            this.MinerClienTabControl.SelectionChanged += (sender, e) => {
                // 延迟创建，以加快主界面的启动
                #region
                var selectedItem = MinerClienTabControl.SelectedItem;
                if(selectedItem == MinerClientTabItemMessage) {
                    if (MinerClientMessagesContainer.Child == null) {
                        MinerClientMessagesContainer.Child = new MinerClientMessages();
                    }
                    MinerStudioRoot.SetIsMinerClientMessagesVisible(true);
                }
                else {
                    MinerStudioRoot.SetIsMinerClientMessagesVisible(false);
                }
                #endregion
            };
            this.ConsoleRectangle.IsVisibleChanged += (sender, e) => {
                MoveConsoleWindow();
            };
            this.ConsoleRectangle.SizeChanged += (s, e) => {
                MoveConsoleWindow();
            };
            this.StateChanged += (s, e) => {
                #region
                if (WindowState == WindowState.Maximized) {
                    ResizeCursors.Visibility = Visibility.Collapsed;
                }
                else {
                    ResizeCursors.Visibility = Visibility.Visible;
                }
                MoveConsoleWindow();
                #endregion
            };
            this.SizeChanged += (s, e) => {
                #region
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
            this.LocationChanged += (sender, e) => {
                MoveConsoleWindow();
            };
            this.AddEventPath<Per1SecondEvent>("刷新倒计时秒表", LogEnum.None,
                action: message => {
                    #region
                    var minerClients = Vm.MinerClients.ToArray();
                    if (Vm.CountDown > 0) {
                        Vm.CountDown = Vm.CountDown - 1;
                        foreach (var item in minerClients) {
                            item.OnPropertyChanged(nameof(item.LastActivedOnText));
                        }
                        if (JsonRpcRoot.IsOuterNet && Vm.CountDown == 5) {
                            // 外网群控时在矿机列表页数据刷新前5秒通过Ws刷新矿机的算力数据
                            MinerStudioRoot.WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetSpeed) {
                                Data = Vm.MinerClients.Select(a => a.ClientId).ToList()
                            });
                        }
                    }
                    else if (Vm.CountDown == 0) {
                        MinerStudioRoot.MinerClientsWindowVm.QueryMinerClients(isAuto: true);
                    }
                    #endregion
                }, location: this.GetType());
            NotiCenterWindow.Bind(this, ownerIsTopMost: true);
            MinerStudioRoot.MinerClientsWindowVm.QueryMinerClients();
        }

        protected override void OnClosed(EventArgs e) {
            _instance = null;
            base.OnClosed(e);
            ConsoleWindow.Instance.Hide();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void MinerClientScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e, isLeftBar: true);
        }

        #region 显示或隐藏半透明遮罩层
        // 因为挖矿端主界面是透明的，遮罩方法和普通窗口不同，如果按照通用的方法遮罩的话会导致能透过窗口看见windows桌面或者下面的窗口。
        public void ShowMask() {
            MaskLayer.Visibility = Visibility.Visible;
        }

        public void HideMask() {
            MaskLayer.Visibility = Visibility.Collapsed;
        }
        #endregion

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
                int marginLeft = paddingLeft + (int)point.X;
                int width = (int)(ConsoleRectangle.ActualWidth - paddingLeft);
                consoleWindow.MoveWindow(marginLeft: marginLeft, marginTop: (int)point.Y, width, height: (int)ConsoleRectangle.ActualHeight + 1);
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
