using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelGrid : UserControl {
        public KernelPageViewModel Vm {
            get {
                return (KernelPageViewModel)this.DataContext;
            }
        }

        public KernelGrid() {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null) {
                ((KernelViewModel)dg.SelectedItem).Edit.Execute(FormType.Edit);
            }
        }

        private void DataGrid_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Vm.KernelDownloadingVisible = System.Windows.Visibility.Collapsed;
        }

        private void ButtonLeftCoin_Click(object sender, System.Windows.RoutedEventArgs e) {
            ButtonLeft.IsEnabled = Vm.PageNumber != 1;
            ButtonRight.IsEnabled = Vm.PageNumber != Vm.PageNumbers.Count;
        }

        private void ButtonRightCoin_Click(object sender, System.Windows.RoutedEventArgs e) {
            ButtonLeft.IsEnabled = Vm.PageNumber != 1;
            ButtonRight.IsEnabled = Vm.PageNumber != Vm.PageNumbers.Count;
        }
    }
}
