using NTMiner.Vms;
using System;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelPage : UserControl {
        public static void ShowWindow(Guid kernelId, Action<bool, string> downloadComplete = null) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Kernels",
                CloseVisible = System.Windows.Visibility.Visible,
                HeaderVisible = System.Windows.Visibility.Collapsed,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = (DevMode.IsDevMode ? 1160 : 860),
                Height = 520
            },
            ucFactory: (window) => {
                var uc = new KernelPage();
                uc.CloseWindow = () => window.Close();
                return uc;
            },
            beforeShow: uc => {
                if (kernelId != Guid.Empty) {
                    KernelPageViewModel vm = (KernelPageViewModel)uc.DataContext;
                    vm.Download(kernelId, (isSuccess, message) => {
                        if (isSuccess) {
                            ((KernelPage)uc).CloseWindow();
                        }
                        downloadComplete(isSuccess, message);
                    });
                }
            }, fixedSize: true);
        }

        public Action CloseWindow { get; set; }

        public KernelPageViewModel Vm {
            get {
                return (KernelPageViewModel)this.DataContext;
            }
        }

        public KernelPage() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void BtnDownloadMenu_Click(object sender, System.Windows.RoutedEventArgs e) {
            if (Vm.KernelDownloadingVisible == System.Windows.Visibility.Visible) {
                Vm.KernelDownloadingVisible = System.Windows.Visibility.Collapsed;
            }
            else {
                Vm.KernelDownloadingVisible = System.Windows.Visibility.Visible;
            }
        }
    }
}
