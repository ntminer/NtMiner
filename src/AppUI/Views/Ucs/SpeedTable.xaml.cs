using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class SpeedTable : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Gpu",
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => new SpeedTable(), fixedSize: true);
        }

        private GpuSpeedViewModels Vm {
            get {
                return (GpuSpeedViewModels)this.DataContext;
            }
        }

        public SpeedTable() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(nameof(SpeedTable), this.Resources);
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
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
