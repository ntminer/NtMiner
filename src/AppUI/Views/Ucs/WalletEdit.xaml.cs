using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class WalletEdit : UserControl {
        public static void ShowEditWindow(WalletViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Wallet",
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                WalletViewModel vm = new WalletViewModel(source);
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
