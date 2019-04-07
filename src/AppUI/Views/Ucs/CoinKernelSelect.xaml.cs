using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class CoinKernelSelect : UserControl {
        public CoinKernelSelectViewModel Vm {
            get {
                return (CoinKernelSelectViewModel)this.DataContext;
            }
        }

        public CoinKernelSelect(CoinKernelSelectViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
