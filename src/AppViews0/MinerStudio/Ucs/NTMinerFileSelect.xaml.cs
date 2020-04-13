using NTMiner.MinerStudio.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.MinerStudio.Ucs {
    public partial class NTMinerFileSelect : UserControl {
        public NTMinerFileSelectViewModel Vm {
            get {
                return (NTMinerFileSelectViewModel)this.DataContext;
            }
        }

        public NTMinerFileSelect(NTMinerFileSelectViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }

        private void Lb_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Vm.OnOk?.Invoke(Vm.SelectedResult);
        }

        private void HideView(object sender, System.Windows.RoutedEventArgs e) {
            Vm.HideView?.Execute(null);
        }
    }
}
