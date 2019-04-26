using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class StartStopMineButton : UserControl {
        private StartStopMineButtonViewModel Vm {
            get {
                return MainWindowViewModel.Current.StartStopMineButtonVm;
            }
        }

        public StartStopMineButton() {
            this.DataContext = MainWindowViewModel.Current.StartStopMineButtonVm;
            InitializeComponent();
        }
    }
}
