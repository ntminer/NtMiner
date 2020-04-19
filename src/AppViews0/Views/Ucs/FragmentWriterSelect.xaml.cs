using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class FragmentWriterSelect : UserControl {
        public FragmentWriterSelectViewModel Vm {
            get {
                return (FragmentWriterSelectViewModel)this.DataContext;
            }
        }

        public FragmentWriterSelect(FragmentWriterSelectViewModel vm) {
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
