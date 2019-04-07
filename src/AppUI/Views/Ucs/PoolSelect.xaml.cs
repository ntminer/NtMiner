using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class PoolSelect : UserControl {
        public PoolSelectViewModel Vm {
            get {
                return (PoolSelectViewModel)this.DataContext;
            }
        }

        public PoolSelect(PoolSelectViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
