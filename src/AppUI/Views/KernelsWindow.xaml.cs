using MahApps.Metro.Controls;
using NTMiner.Vms;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class KernelsWindow : MetroWindow {
        private static readonly object _locker = new object();
        private static KernelsWindow _instance = null;
        public static void ShowWindow(Guid kernelId, Action<bool, string> downloadComplete = null) {
            UIThread.Execute(() => {
                if (_instance == null) {
                    lock (_locker) {
                        if (_instance == null) {
                            _instance = new KernelsWindow();
                            _instance.Show();
                        }
                    }
                }
                else {
                    AppHelper.ShowWindow(_instance, false);
                }
                AutoDownload(kernelId, downloadComplete);
            });
        }

        private static void AutoDownload(Guid kernelId, Action<bool, string> downloadComplete) {
            if (kernelId != Guid.Empty) {
                _instance.Vm.Download(kernelId, (isSuccess, message) => {
                    if (isSuccess) {
                        _instance.Close();
                    }
                    downloadComplete(isSuccess, message);
                });
            }
        }

        public KernelsWindowViewModel Vm {
            get {
                return (KernelsWindowViewModel)this.DataContext;
            }
        }

        public KernelsWindow() {
            InitializeComponent();
            if (DevMode.IsDevMode) {
                this.Width += 300;
            }
            AppContext.Instance.KernelVms.PropertyChanged += Current_PropertyChanged;
            AppContext.Instance.KernelVms.IsDownloadingChanged += Current_IsDownloadingChanged;
        }

        protected override void OnClosed(EventArgs e) {
            AppContext.Instance.KernelVms.PropertyChanged -= Current_PropertyChanged;
            AppContext.Instance.KernelVms.IsDownloadingChanged -= Current_IsDownloadingChanged;
            base.OnClosed(e);
            _instance = null;
        }

        private void Current_IsDownloadingChanged(KernelViewModel obj) {
            Vm.OnPropertyChanged(nameof(Vm.DownloadingVms));
        }

        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(AppContext.KernelViewModels.AllKernels)) {
                Vm.OnPropertyChanged(nameof(Vm.QueryResults));
            }
        }

        private void BtnDownloadMenu_Click(object sender, RoutedEventArgs e) {
            if (Vm.KernelDownloadingVisible == Visibility.Visible) {
                Vm.KernelDownloadingVisible = Visibility.Collapsed;
            }
            else {
                Vm.KernelDownloadingVisible = Visibility.Visible;
            }
        }

        private void ButtonLeftCoin_Click(object sender, RoutedEventArgs e) {
            double offset = CoinsScrollView.ContentHorizontalOffset - CoinsScrollView.ViewportWidth;
            CoinsScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeft.IsEnabled = offset > 0;
            ButtonRight.IsEnabled = offset < CoinsScrollView.ScrollableWidth;
        }

        private void ButtonRightCoin_Click(object sender, RoutedEventArgs e) {
            double offset = CoinsScrollView.ContentHorizontalOffset + CoinsScrollView.ViewportWidth;
            CoinsScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeft.IsEnabled = offset > 0;
            ButtonRight.IsEnabled = offset < CoinsScrollView.ScrollableWidth;
        }

        private void CoinsScrollView_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void ListBox_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                Window window = Window.GetWindow(this);
                window.DragMove();
            }
        }

        private void TbKeyword_LostFocus(object sender, RoutedEventArgs e) {
            Vm.Search.Execute(null);
        }

        private void MetroWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}
