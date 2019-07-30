using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class HistoryUc : UserControl {
        public MainWindowViewModel Vm {
            get {
                return (MainWindowViewModel)this.DataContext;
            }
        }

        public HistoryUc() {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<NTMinerFileViewModel>(sender, e);
        }
    }
}
