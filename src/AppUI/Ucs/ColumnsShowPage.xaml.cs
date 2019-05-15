using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class ColumnsShowPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "列显",
                IconName = "Icon_ColumnsShow",
                Width = 300,
                Height = 400,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => new ColumnsShowPage(), fixedSize: true);
        }

        public ColumnsShowPageViewModel Vm {
            get {
                return (ColumnsShowPageViewModel)this.DataContext;
            }
        }

        public ColumnsShowPage() {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<ColumnsShowViewModel>(sender, e);
        }
    }
}
