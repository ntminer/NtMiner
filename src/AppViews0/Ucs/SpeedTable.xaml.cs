using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class SpeedTable : UserControl {
        private SpeedTableViewModel Vm {
            get {
                return (SpeedTableViewModel)this.DataContext;
            }
        }

        public SpeedTable() {
            InitializeComponent();
            this.RunOneceOnLoaded(window => {
                window.Window<ShowOrHideOverClockCommand>("处理显示或隐藏超频界面命令", LogEnum.DevConsole,
                    action: message => {
                        if (message.IsHide) {
                            Vm.IsOverClockVisible = Visibility.Collapsed;
                            Vm.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                        }
                        else {
                            Vm.IsOverClockVisible = Visibility.Visible;
                            Vm.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible;
                        }
                    });
            });
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

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window.GetWindow(this).DragMove();
            }
        }
    }
}
