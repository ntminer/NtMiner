using MahApps.Metro.Controls;
using NTMiner.Vms;
using System;
using System.Windows;

namespace NTMiner.Views {
    public partial class MainWindow : MetroWindow {
        public MainWindowViewModel Vm {
            get {
                return (MainWindowViewModel)this.DataContext;
            }
        }

        public MainWindow() {
            InitializeComponent();
            EventHandler ChangeNotiCenterWindowLocation = Wpf.Util.ChangeNotiCenterWindowLocation(this);
            this.Activated += ChangeNotiCenterWindowLocation;
            this.LocationChanged += ChangeNotiCenterWindowLocation;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void MetroWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<NTMinerFileViewModel>(sender, e);
        }
    }
}
