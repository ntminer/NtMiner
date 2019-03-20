using MahApps.Metro.Controls;
using NTMiner.Notifications;
using NTMiner.Vms;
using NTMiner.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace NTMiner.Views {
    public partial class MainWindow : MetroWindow, IMainWindow {
        public static string ViewId = nameof(MainWindow);

        private MainWindowViewModel Vm {
            get {
                return (MainWindowViewModel)this.DataContext;
            }
        }

        public MainWindow() {
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
            InitializeComponent();
            EventHandler ChangeNotiCenterWindowLocation = Wpf.Util.ChangeNotiCenterWindowLocation(this);
            this.Activated += ChangeNotiCenterWindowLocation;
            this.LocationChanged += ChangeNotiCenterWindowLocation;
            Write.WriteDevLineMethod = DebugLine;
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
            if (!Windows.Role.IsAdministrator) {
                NotiCenterWindowViewModel.Current.Manager
                    .CreateMessage()
                    .Warning("请以管理员身份运行。")
                    .WithButton("点击以管理员身份运行", button => {
                        AppStatic.RunAsAdministrator.Execute(null);
                    })
                    .Dismiss().WithButton("忽略", button => {
                        Vm.IsBtnRunAsAdministratorVisible = Visibility.Visible;
                    }).Queue();
            }
            if (NTMinerRoot.Current.GpuSet.Count == 0) {
                NotiCenterWindowViewModel.Current.Manager.ShowErrorMessage("没有检测到矿卡。");
            }
            NTMinerRoot.RegHotKey = (key) => {
                string message;
                if (!RegHotKey(key, out message)) {
                    NotiCenterWindowViewModel.Current.Manager.ShowErrorMessage(message, 4);
                    return false;
                }
                else {
                    NotiCenterWindowViewModel.Current.Manager.ShowSuccessMessage($"热键Ctrl + Alt + {key.ToString()} 设置成功");
                    return true;
                }
            };
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
                NotiCenterWindowViewModel.Current.Manager
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
                            this.ShowThisWindow();
                        }
                    }
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        public void ShowThisWindow() {
            ShowInTaskbar = true;
            if (WindowState == WindowState.Minimized) {
                WindowState = WindowState.Normal;
            }
            else {
                var oldState = WindowState;
                this.WindowState = WindowState.Minimized;
                this.WindowState = oldState;
            }
            this.Activate();
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private ScrollViewer _scrollView;
        private ScrollViewer ScrollViewer {
            get {
                if (_scrollView == null) {
                    _scrollView = this.FlowDocumentScrollViewer.GetScrollViewer();
                }
                return _scrollView;
            }
        }

        private void InnerWrite(string text, ConsoleColor foreground) {
            Run run = new Run(text) {
                Foreground = new SolidColorBrush(foreground.ToMediaColor())
            };
            this.ConsoleParagraphDebug.Inlines.Add(run);

            InlineCollection list = this.ConsoleParagraphDebug.Inlines;
            if (list.Count > 1000) {
                int delLines = list.Count - 1000;
                while (delLines-- > 0) {
                    list.Remove(list.FirstInline);
                }
            }
            if (ChkbIsConsoleAutoScrollToEnd.IsChecked.HasValue && ChkbIsConsoleAutoScrollToEnd.IsChecked.Value) {
                this.ScrollViewer?.ScrollToEnd();
            }
        }

        public void DebugLine(string text, ConsoleColor foreground) {
            Dispatcher.Invoke((Action)(() => {
                if (this.ConsoleParagraphDebug.Inlines.Count > 0) {
                    text = "\n" + text;
                }
                InnerWrite(text, foreground);
            }));
        }
    }
}
