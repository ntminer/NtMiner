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
    }
}
