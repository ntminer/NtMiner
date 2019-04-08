using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class WalletSelect : UserControl {
        public WalletSelectViewModel Vm {
            get {
                return (WalletSelectViewModel)this.DataContext;
            }
        }

        public WalletSelect(WalletSelectViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }

        private void KbButtonManageWallets_Click(object sender, System.Windows.RoutedEventArgs e) {
            Vm.HideView?.Execute(null);
        }
    }
}
