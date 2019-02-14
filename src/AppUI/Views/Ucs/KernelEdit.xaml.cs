using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class KernelEdit : UserControl {
        public static void ShowEditWindow(KernelViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Kernel",
                IsDialogWindow = true,
                Width = 580,
                Height = 400,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                KernelViewModel vm = new KernelViewModel(source);
                vm.CloseWindow = () => window.Close();
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
            this.CbCoins.SelectedItem = CoinViewModel.PleaseSelect;
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void CoinKernelDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            Point p = e.GetPosition(dg);
            if (p.Y < 30) {
                return;
            }
            if (dg.SelectedItem != null) {
                CoinKernelViewModel kernelVm = (CoinKernelViewModel)dg.SelectedItem;
                kernelVm.Edit.Execute(null);
            }
        }
    }
}
