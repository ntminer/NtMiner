using NTMiner.Vms;
using System;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelDownloading : UserControl {
        public static void ShowWindow(Guid kernelId, Action<bool, string> downloadComplete) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                Title = "下载挖矿内核",
                IconName = "Icon_Download",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed
            },
            ucFactory: (window) => {
                var uc = new KernelDownloading {
                    CloseWindow = window.Close
                };
                return uc;
            },
            beforeShow: uc=> {
                if (kernelId != Guid.Empty) {
                    var vm = (KernelDownloadingViewModel)uc.DataContext;
                    vm.Download(kernelId, (isSuccess, message) => {
                        if (isSuccess) {
                            ((KernelDownloading)uc).CloseWindow();
                        }
                        downloadComplete(isSuccess, message);
                    });
                    vm.OnPropertyChanged(nameof(vm.DownloadingVms));
                }
            }, fixedSize: true);
        }

        private Action CloseWindow;

        public KernelDownloadingViewModel Vm {
            get {
                return (KernelDownloadingViewModel)this.DataContext;
            }
        }

        public KernelDownloading() {
            InitializeComponent();
        }
    }
}
