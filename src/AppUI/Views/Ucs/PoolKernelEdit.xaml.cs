using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class PoolKernelEdit : UserControl {
        public static void ShowEditWindow(PoolKernelViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                IconName = "Icon_Kernel",
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                PoolKernelViewModel vm = new PoolKernelViewModel(source);
                vm.CloseWindow = () => window.Close();
                return new PoolKernelEdit(vm);
            }, fixedSize: true);
        }

        private PoolKernelViewModel Vm {
            get {
                return (PoolKernelViewModel)this.DataContext;
            }
        }

        public PoolKernelEdit(PoolKernelViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
