using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class GpuSpeedDataTable : UserControl {
        private GpuSpeedDataViewModels Vm {
            get {
                return (GpuSpeedDataViewModels)this.DataContext;
            }
        }

        public GpuSpeedDataTable() {
            InitializeComponent();
        }
    }
}
