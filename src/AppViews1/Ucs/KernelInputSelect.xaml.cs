using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelInputSelect : UserControl {
        public KernelInputSelectViewModel Vm {
            get {
                return (KernelInputSelectViewModel)this.DataContext;
            }
        }

        public KernelInputSelect(KernelInputSelectViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }

        private void KbButtonManagePools_Click(object sender, System.Windows.RoutedEventArgs e) {
            Vm.HideView?.Execute(null);
        }

        private void DataGrid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Vm.OnOk?.Invoke(Vm.SelectedResult);
        }

        private void DataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == System.Windows.Input.Key.Enter) {
                Vm.OnOk?.Invoke(Vm.SelectedResult);
                e.Handled = true;
            }
        }
    }
}
