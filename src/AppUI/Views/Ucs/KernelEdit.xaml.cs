using NTMiner.Vms;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class KernelEdit : UserControl {
        public static void ShowEditWindow(KernelViewModel source) {
            double width = 660;
            double height = 620;
            bool fixedSize = false;
            if (!DevMode.IsDevMode) {
                width = 0;
                height = 0;
                fixedSize = true;
            }
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Kernel",
                IsDialogWindow = true,
                Width = width,
                Height = height,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                KernelViewModel vm = new KernelViewModel(source);
                vm.CloseWindow = () => window.Close();
                return new KernelEdit(vm);
            }, fixedSize: fixedSize);
        }

        private KernelViewModel Vm {
            get {
                return (KernelViewModel)this.DataContext;
            }
        }

        public KernelEdit(KernelViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void KernelOutputFilterDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null) {
                KernelOutputFilterViewModel kernelOutputFilterVm = (KernelOutputFilterViewModel)dg.SelectedItem;
                kernelOutputFilterVm.Edit.Execute(null);
            }
        }

        private void KernelOutputTranslaterDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null) {
                KernelOutputTranslaterViewModel kernelOutputTranslaterVm = (KernelOutputTranslaterViewModel)dg.SelectedItem;
                kernelOutputTranslaterVm.Edit.Execute(null);
            }
        }

        private void CoinKernelDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null) {
                CoinKernelViewModel kernelVm = (CoinKernelViewModel)dg.SelectedItem;
                kernelVm.Edit.Execute(null);
            }
        }
    }
}
