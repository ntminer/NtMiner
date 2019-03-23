using NTMiner.Vms;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputPage : UserControl {
        public static string ViewId = nameof(KernelOutputPage);

        public static void ShowWindow(KernelOutputViewModel selectedKernelOutputVm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_KernelOutput",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = 960,
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
            if (selectedKernelOutputVm != null) {
                KernelOutputPageViewModel.Current.CurrentKernelOutputVm = selectedKernelOutputVm;
            }
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<KernelOutputViewModel>(sender, e);
        }

        private void KernelOutputFilterDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<KernelOutputFilterViewModel>(sender, e);
        }

        private void KernelOutputTranslaterDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<KernelOutputTranslaterViewModel>(sender, e);
        }
    }
}
