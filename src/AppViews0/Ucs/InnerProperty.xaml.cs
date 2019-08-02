using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class InnerProperty : UserControl {
        private InnerPropertyViewModel Vm {
            get {
                return (InnerPropertyViewModel)this.DataContext;
            }
        }

        public InnerProperty() {
            InitializeComponent();
        }
    }
}
