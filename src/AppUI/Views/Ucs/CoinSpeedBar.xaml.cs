using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class CoinSpeedBar : UserControl {
        private CoinSpeedBarViewModel Vm {
            get {
                return (CoinSpeedBarViewModel)this.DataContext;
            }
        }

        public CoinSpeedBar() {
            InitializeComponent();
        }
    }
}
