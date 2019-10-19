using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class Icons : UserControl {
        public IconsViewModel Vm {
            get {
                return (IconsViewModel)this.DataContext;
            }
        }

        public Icons() {
            InitializeComponent();
        }
    }
}
