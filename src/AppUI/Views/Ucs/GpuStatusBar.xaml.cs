using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class GpuStatusBar : UserControl {
        private GpuStatusBarViewModel Vm {
            get {
                return (GpuStatusBarViewModel)this.DataContext;
            }
        }

        public GpuStatusBar() {
            InitializeComponent();
        }
    }
}
