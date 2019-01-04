using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class WalletEdit : UserControl {
        public static void ShowEditWindow(WalletViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Wallet",
                IsDialogWindow = true,
                SaveVisible = source.IsTestWallet ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible,
                CloseVisible = System.Windows.Visibility.Visible,
                OnOk = (uc) =>
                {
                    var vm = ((WalletEdit)uc).Vm;
                    if (!source.IsTestWallet) {
                        if (NTMinerRoot.Current.WalletSet.Contains(source.Id)) {
                            Global.Execute(new UpdateWalletCommand(vm));
                        }
                        else {
                            Global.Execute(new AddWalletCommand(vm));
                        }
                    }
                    return true;
                }
            }, ucFactory: (window) =>
            {
                WalletViewModel vm = new WalletViewModel(source.Id).Update(source);
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
            ResourceDictionarySet.Instance.FillResourceDic(nameof(WalletEdit), this.Resources);
        }
    }
}
