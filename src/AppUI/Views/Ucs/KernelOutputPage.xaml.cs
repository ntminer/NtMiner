using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputPage : UserControl {
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

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            if (Vm.CurrentKernelOutputVm != null) {
                Vm.CurrentKernelOutputVm.Edit.Execute(null);
            }
        }

        private void KernelOutputFilterDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            Point p = e.GetPosition(dg);
            if (p.Y < 30) {
                return;
            }
            if (dg.SelectedItem != null) {
                KernelOutputFilterViewModel kernelOutputFilterVm = (KernelOutputFilterViewModel)dg.SelectedItem;
                kernelOutputFilterVm.Edit.Execute(null);
            }
        }

        private void KernelOutputTranslaterDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            Point p = e.GetPosition(dg);
            if (p.Y < 30) {
                return;
            }
            if (dg.SelectedItem != null) {
                KernelOutputTranslaterViewModel kernelOutputTranslaterVm = (KernelOutputTranslaterViewModel)dg.SelectedItem;
                kernelOutputTranslaterVm.Edit.Execute(null);
            }
        }
    }
}
