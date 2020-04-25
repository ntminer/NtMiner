using NTMiner.MinerStudio.Vms;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class MinerClients : UserControl {
        public MinerClientsWindowViewModel Vm {
            get {
                return MinerStudioRoot.MinerClientsWindowVm;
            }
        }

        public MinerClients() {
            this.DataContext = Vm;
            InitializeComponent();
        }

        private void MinerClientsGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            Vm.SelectedMinerClients = ((DataGrid)sender).SelectedItems.Cast<MinerClientViewModel>().ToArray();
        }

        private void DataGrid_OnSorting(object sender, DataGridSortingEventArgs e) {
            e.Handled = true;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            WpfUtil.DataGrid_MouseDoubleClick<MinerClientViewModel>(sender, e, rowVm => {
                rowVm.RemoteDesktop.Execute(Vm.SelectedMinerClients[0].GetRemoteDesktopIp());
            });
        }
    }
}
