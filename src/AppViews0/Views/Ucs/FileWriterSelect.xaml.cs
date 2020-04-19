using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class FileWriterSelect : UserControl {
        public FileWriterSelectViewModel Vm {
            get {
                return (FileWriterSelectViewModel)this.DataContext;
            }
        }

        public FileWriterSelect(FileWriterSelectViewModel vm) {
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
