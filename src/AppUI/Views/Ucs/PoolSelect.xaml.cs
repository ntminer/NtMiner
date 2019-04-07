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

        private void KbButtonManagePools_Click(object sender, System.Windows.RoutedEventArgs e) {
            Vm.HideView?.Execute(null);
        }
    }
}
