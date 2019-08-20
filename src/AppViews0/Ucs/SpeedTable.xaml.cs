using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class SpeedTable : UserControl {
        private AppContext.GpuSpeedViewModels Vm {
            get {
                return AppContext.Instance.GpuSpeedVms;
            }
        }

        public SpeedTable() {
            this.DataContext = AppContext.Instance.GpuSpeedVms;
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

        private void BtnOverClockVisible_Click(object sender, RoutedEventArgs e) {
            ContentControl btn = (ContentControl)sender;
            if (DataGrid.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Collapsed) {
                btn.Content = "隐藏超频";
                DataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible;
            }
            else if (DataGrid.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Visible) {
                DataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                btn.Content = "显示超频";
            }
        }
    }
}
