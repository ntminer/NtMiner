using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class ServerMessages : UserControl {
        private ServerMessagesViewModel Vm {
            get {
                return (ServerMessagesViewModel)this.DataContext;
            }
        }

        public ServerMessages() {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (!DevMode.IsDevMode) {
                return;
            }
            WpfUtil.DataGrid_MouseDoubleClick<ServerMessageViewModel>(sender, e);
        }
    }
}
