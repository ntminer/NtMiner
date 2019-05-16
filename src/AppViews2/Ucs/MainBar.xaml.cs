using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MainBar : UserControl {
        public MainBarViewModel Vm {
            get {
                return (MainBarViewModel)this.DataContext;
            }
        }

        public MainBar() {
            InitializeComponent();
        }
    }
}
