using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelSelect : UserControl {
        public KernelSelectViewModel Vm {
            get {
                return (KernelSelectViewModel)this.DataContext;
            }
        }

        public KernelSelect(KernelSelectViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
