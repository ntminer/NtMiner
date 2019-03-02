using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class WalletEdit : UserControl {
        public static void ShowWindow(FormType formType, WalletViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                FormType = formType,
                IconName = "Icon_Wallet",
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                WalletViewModel vm = new WalletViewModel(source);
                vm.CloseWindow = () => window.Close();
                return new WalletEdit(vm);
            }, fixedSize: true);
        }

        private WalletViewModel Vm {
            get {
                return (WalletViewModel)this.DataContext;
            }
        }

        public WalletEdit(WalletViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
