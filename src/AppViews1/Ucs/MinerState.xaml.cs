using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerState : UserControl {
        private MinerStateViewModel Vm {
            get {
                return (MinerStateViewModel)this.DataContext;
            }
        }

        public MinerState() {
            InitializeComponent();
        }
    }
}
