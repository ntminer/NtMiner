using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class LatestUc : UserControl {
        public MainWindowViewModel Vm {
            get {
                return MainWindowViewModel.Instance;
            }
        }

        public LatestUc() {
            InitializeComponent();
        }
    }
}
