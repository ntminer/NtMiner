using NTMiner.MinerStudio.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.MinerStudio.Ucs {
    public partial class MineWorkSelect : UserControl {
        public MineWorkSelectViewModel Vm {
            get {
                return (MineWorkSelectViewModel)this.DataContext;
            }
        }

        public MineWorkSelect(MineWorkSelectViewModel vm) {
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
