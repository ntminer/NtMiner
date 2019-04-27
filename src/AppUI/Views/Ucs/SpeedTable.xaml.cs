using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class SpeedTable : UserControl {
        private GpuSpeedViewModels Vm {
            get {
                return MinerClientAppContext.Current.GpuSpeedVms;
            }
        }

        public SpeedTable() {
            this.DataContext = MinerClientAppContext.Current.GpuSpeedVms;
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            Point p = e.GetPosition(dg);
            if (p.Y < 30) {
                return;
            }
            if (dg.SelectedItem != null) {
                GpuSpeedViewModel gpuSpeedVm = (GpuSpeedViewModel)dg.SelectedItem;
                gpuSpeedVm.OpenChart.Execute(null);
            }
            else {
                SpeedCharts.ShowWindow(null);
            }
            e.Handled = true;
        }
    }
}
