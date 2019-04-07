using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class CoinSelect : UserControl {
        public CoinSelectViewModel Vm {
            get {
                return (CoinSelectViewModel)this.DataContext;
            }
        }

        public CoinSelect(CoinSelectViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }

        private void KbButtonManageCoins_Click(object sender, System.Windows.RoutedEventArgs e) {
            Vm.HideView?.Execute(null);
        }
    }
}
