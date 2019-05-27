using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class SpeedTable : UserControl {
        private AppContext.GpuSpeedViewModels Vm {
            get {
                return AppContext.Instance.GpuSpeedVms;
            }
        }

        public SpeedTable() {
            if (!Design.IsInDesignMode) {
                this.DataContext = AppContext.Instance.GpuSpeedVms;
            }
            InitializeComponent();
        }
    }
}
