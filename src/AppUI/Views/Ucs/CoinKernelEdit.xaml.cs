using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class CoinKernelEdit : UserControl {
        public static void ShowWindow(FormType formType, CoinKernelViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                FormType = formType,
                IsDialogWindow = true,
                IconName = "Icon_Kernel",
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                CoinKernelViewModel vm = new CoinKernelViewModel(source);
                vm.CloseWindow = () => window.Close();
                return new CoinKernelEdit(vm);
            }, fixedSize: true);
        }

        private CoinKernelViewModel Vm {
            get {
                return (CoinKernelViewModel)this.DataContext;
            }
        }

        public CoinKernelEdit(CoinKernelViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
