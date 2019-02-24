using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class PoolEdit : UserControl {
        public static void ShowWindow(PoolViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Pool",
                IsDialogWindow = true,
                Width = 580,
                Height = 400,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                PoolViewModel vm = new PoolViewModel(source);
                vm.CloseWindow = () => window.Close();
                return new PoolEdit(vm);
            }, fixedSize: true);
        }

        private PoolViewModel Vm {
            get {
                return (PoolViewModel)this.DataContext;
            }
        }

        public PoolEdit(PoolViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void KernelDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            Point p = e.GetPosition(dg);
            if (p.Y < 30) {
                return;
            }
            if (dg.SelectedItem != null) {
                PoolKernelViewModel kernelVm = (PoolKernelViewModel)dg.SelectedItem;
                kernelVm.Edit.Execute(null);
            }
        }
    }
}
