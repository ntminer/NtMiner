using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class SpeedTable : UserControl {
        private SpeedTableViewModel Vm {
            get {
                return (SpeedTableViewModel)this.DataContext;
            }
        }

        public SpeedTable() {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            Point p = e.GetPosition(dg);
            if (p.Y < dg.ColumnHeaderHeight) {
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
