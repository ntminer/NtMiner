using NTMiner.Views;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner {
    public partial class MainWindow : BlankWindow {
        public MainWindowViewModel Vm {
            get {
                return (MainWindowViewModel)this.DataContext;
            }
        }

        public MainWindow() {
            InitializeComponent();
            NotiCenterWindow.Bind(this);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            Point p = e.GetPosition(dg);
            if (p.Y < dg.ColumnHeaderHeight) {
                return;
            }
            if (dg.SelectedItem != null) {
                string ip = (string)dg.SelectedItem;
                Clipboard.SetDataObject(ip);
                VirtualRoot.Out.ShowSuccess(ip, autoHideSeconds: 1, "复制成功");
            }
        }
    }
}
