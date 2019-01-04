using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class KernelEdit : UserControl {
        public static void ShowEditWindow(KernelViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Kernel",
                IsDialogWindow = true,
                Width = 660,
                Height = 502,
                SaveVisible = DevMode.IsDevMode ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed,
                CloseVisible = System.Windows.Visibility.Visible,
                OnOk = (uc) => {
                    var vm = ((KernelEdit)uc).Vm;
                    if (NTMinerRoot.Current.KernelSet.Contains(source.Id)) {
                        Global.Execute(new UpdateKernelCommand(vm));
                    }
                    else {
                        Global.Execute(new AddKernelCommand(vm));
                    }
                    return true;
                }
            }, ucFactory: (window) => {
                KernelViewModel vm = new KernelViewModel(source);
                return new KernelEdit(vm);
            }, fixedSize: false);
        }

        private KernelViewModel Vm {
            get {
                return (KernelViewModel)this.DataContext;
            }
        }

        public KernelEdit(KernelViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
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
