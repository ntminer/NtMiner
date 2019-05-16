using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class OverClockDataPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "超频菜谱",
                IconName = "Icon_OverClock",
                Width = 600,
                Height = 400,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => new OverClockDataPage(), fixedSize: true);
        }

        public OverClockDataPageViewModel Vm {
            get {
                return (OverClockDataPageViewModel)this.DataContext;
            }
        }

        public OverClockDataPage() {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<OverClockDataViewModel>(sender, e);
        }
    }
}
