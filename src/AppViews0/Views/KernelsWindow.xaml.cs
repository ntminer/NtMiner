using NTMiner.Vms;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class KernelsWindow : BlankWindow {
        private static readonly object _locker = new object();
        private static KernelsWindow _instance = null;
        public static void ShowWindow(string keyword = null) {
            if (_instance == null) {
                lock (_locker) {
                    if (_instance == null) {
                        _instance = new KernelsWindow();
                        _instance.Show();
                    }
                }
            }
            else {
                _instance.ShowWindow(false);
            }
            if (keyword != null) {
                _instance.Vm.Keyword = keyword;
            }
        }

        public KernelsWindowViewModel Vm { get; private set; }

        public KernelsWindow() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new KernelsWindowViewModel();
            this.DataContext = this.Vm;
            // 为了使设计视图的宽高生效以下几个属性的赋值挪到这里
            Width = AppRoot.MainWindowWidth;
            Height = AppRoot.MainWindowHeight;
            MinHeight = 430;
            MinWidth = 640;
            InitializeComponent();
            if (DevMode.IsDevMode) {
                this.Width += 600;
            }
            this.TbUcName.Text = nameof(KernelsWindow);
            this.AddEventPath<MineStopedEvent>("当内核宝库窗口开着时如果是本地手动停止的挖矿则引发UserActionEvent事件", LogEnum.DevConsole,
                action: message => {
                    if (message.StopReason == StopMineReason.LocalUserAction) {
                        VirtualRoot.RaiseEvent(new UserActionEvent());
                    }
                }, location: this.GetType());
            this.AddEventPath<LocalContextReInitedEventHandledEvent>("ServerContext的Vm集刷新后刷新内核宝库", LogEnum.DevConsole,
                action: message => {
                    Vm.OnPropertyChanged(nameof(Vm.QueryResults));
                }, location: this.GetType());
            AppRoot.KernelVms.PropertyChanged += Current_PropertyChanged;
            NotiCenterWindow.Bind(this);
            if (!Vm.MinerProfile.IsMining) {
                VirtualRoot.RaiseEvent(new UserActionEvent());
            }
        }

        protected override void OnClosed(EventArgs e) {
            AppRoot.KernelVms.PropertyChanged -= Current_PropertyChanged;
            base.OnClosed(e);
            _instance = null;
        }

        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(AppRoot.KernelViewModels.AllKernels)) {
                Vm.OnPropertyChanged(nameof(Vm.QueryResults));
            }
        }

        private void TbKeyword_LostFocus(object sender, RoutedEventArgs e) {
            Vm.Search.Execute(null);
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<KernelViewModel>(sender, e);
        }

        private void ButtonLeftCoin_Click(object sender, RoutedEventArgs e) {
            double offset = CoinsScrollView.ContentHorizontalOffset - CoinsScrollView.ViewportWidth;
            CoinsScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeftCoin.IsEnabled = offset > 0;
            ButtonRightCoin.IsEnabled = offset < CoinsScrollView.ScrollableWidth;
        }

        private void ButtonRightCoin_Click(object sender, RoutedEventArgs e) {
            double offset = CoinsScrollView.ContentHorizontalOffset + CoinsScrollView.ViewportWidth;
            CoinsScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeftCoin.IsEnabled = offset > 0;
            ButtonRightCoin.IsEnabled = offset < CoinsScrollView.ScrollableWidth;
        }

        private void CoinsScrollView_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void ListBox_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window window = Window.GetWindow(this);
                window.DragMove();
            }
        }

        private void ButtonLeftBrand_Click(object sender, RoutedEventArgs e) {
            double offset = BrandsScrollView.ContentHorizontalOffset - BrandsScrollView.ViewportWidth;
            BrandsScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeftBrand.IsEnabled = offset > 0;
            ButtonRightBrand.IsEnabled = offset < BrandsScrollView.ScrollableWidth;
        }

        private void ButtonRightBrand_Click(object sender, RoutedEventArgs e) {
            double offset = BrandsScrollView.ContentHorizontalOffset + BrandsScrollView.ViewportWidth;
            BrandsScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeftBrand.IsEnabled = offset > 0;
            ButtonRightBrand.IsEnabled = offset < BrandsScrollView.ScrollableWidth;
        }

        private void BrandsScrollView_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void TbKeyword_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                Vm.Keyword = this.TbKeyword.Text;
            }
        }
    }
}
