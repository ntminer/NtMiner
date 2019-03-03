using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class PoolKernelEdit : UserControl {
        public static string ViewId = nameof(PoolKernelEdit);

        public static void ShowWindow(FormType formType, PoolKernelViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                FormType = formType,
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
