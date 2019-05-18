using NTMiner.Vms;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelsPage : UserControl {
        // TODO:单独一个弹窗下载
        public static void ShowWindow(Guid kernelId, Action<bool, string> downloadComplete = null) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "插件",
                IconName = "Icon_Kernel",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = DevMode.IsDebugMode ? 960 : 860,
                Height = 520
            },
            ucFactory: (window) => {
                var uc = new KernelsPage(); 
                uc.AutoDownload(kernelId, (isSuccess, message)=> {
                    downloadComplete(isSuccess, message);
                    window.Close();
                });
                return uc;
            });
        }

        private void AutoDownload(Guid kernelId, Action<bool, string> downloadComplete) {
            if (kernelId != Guid.Empty) {
                this.Vm.Download(kernelId, (isSuccess, message) => {
                    downloadComplete(isSuccess, message);
                });
            }
        }

        public KernelsWindowViewModel Vm {
            get {
                return (KernelsWindowViewModel)this.DataContext;
            }
        }

        public KernelsPage() {
            InitializeComponent();
            if (Design.IsInDesignMode) {
                return;
            }
            AppContext.Instance.KernelVms.PropertyChanged += Current_PropertyChanged;
            AppContext.Instance.KernelVms.IsDownloadingChanged += Current_IsDownloadingChanged;
            this.Unloaded += KernelsPage_Unloaded;
        }

        private void KernelsPage_Unloaded(object sender, RoutedEventArgs e) {
            AppContext.Instance.KernelVms.PropertyChanged -= Current_PropertyChanged;
            AppContext.Instance.KernelVms.IsDownloadingChanged -= Current_IsDownloadingChanged;
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
    }
}
