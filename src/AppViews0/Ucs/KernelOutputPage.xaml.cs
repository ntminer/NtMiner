using NTMiner.Vms;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputPage : UserControl {
        public static void ShowWindow(KernelOutputViewModel selectedKernelOutputVm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "内核输出",
                IconName = "Icon_KernelOutput",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = 1360,
                Height = 720
            }, 
            ucFactory: (window) => new KernelOutputPage(selectedKernelOutputVm));
        }

        private KernelOutputPageViewModel Vm {
            get {
                return (KernelOutputPageViewModel)this.DataContext;
            }
        }

        public KernelOutputPage(KernelOutputViewModel selectedKernelOutputVm) {
            InitializeComponent();
            if (selectedKernelOutputVm != null) {
                Vm.CurrentKernelOutputVm = selectedKernelOutputVm;
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<KernelOutputViewModel>(sender, e);
        }

        private void KernelOutputTranslaterDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            WpfUtil.DataGrid_EditRow<KernelOutputTranslaterViewModel>(sender, e);
        }
    }
}
