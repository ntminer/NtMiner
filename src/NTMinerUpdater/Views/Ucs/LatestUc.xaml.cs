using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class LatestUc : UserControl {
        public MainWindowViewModel Vm {
            get {
                return (MainWindowViewModel)this.DataContext;
            }
        }

        public LatestUc() {
            InitializeComponent();
        }
    }
}
