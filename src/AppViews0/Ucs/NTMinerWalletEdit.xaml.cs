using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class NTMinerWalletEdit : UserControl {
        public static void ShowWindow(FormType formType, NTMinerWalletViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "NTMiner钱包",
                FormType = formType,
                IsDialogWindow = true,
                Width = 520,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Wallet"
            }, ucFactory: (window) => {
                NTMinerWalletViewModel vm = new NTMinerWalletViewModel(source) {
                    CloseWindow = window.Close
                };
                return new NTMinerWalletEdit(vm);
            }, fixedSize: true);
        }

        private NTMinerWalletViewModel Vm {
            get {
                return (NTMinerWalletViewModel)this.DataContext;
            }
        }
        public NTMinerWalletEdit(NTMinerWalletViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
