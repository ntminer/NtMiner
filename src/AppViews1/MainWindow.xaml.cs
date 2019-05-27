using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Views {
    public partial class MainWindow : BlankWindow {
        private MainWindowViewModel Vm {
            get {
                return (MainWindowViewModel)this.DataContext;
            }
        }

        private bool _isFirstLoaded = true;
        public MainWindow() {
#if DEBUG
            VirtualRoot.Stopwatch.Restart();
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
            this.SizeChanged += (object sender, SizeChangedEventArgs e) => {
                if (e.WidthChanged) {
                    const double width = 720;
                    if (e.NewSize.Width < width) {
                        foreach (var tabItem in this.MainTab.Items.OfType<MainTabItem>()) {
                            tabItem.Margin = new Thickness(0);
                        }
                    }
                    else if (e.NewSize.Width >= width) {
                        foreach (var tabItem in this.MainTab.Items.OfType<MainTabItem>()) {
                            tabItem.Margin = new Thickness(8, 0, 8,  0);
                        }
                    }
                }
            };
            EventHandler changeNotiCenterWindowLocation = NotiCenterWindow.CreateNotiCenterWindowLocationManager(this);
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
        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }

    public class MainTabItem : TabItem {
        [Description("图标"), Category("KbSkin")]
        public Geometry Icon {
            get { return (Geometry)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Geometry), typeof(MainTabItem), new PropertyMetadata(null));
    }
}
