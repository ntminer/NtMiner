using MahApps.Metro.Controls;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class ControlCenterWindow : MetroWindow, IMainWindow {
        public ControlCenterWindowViewModel Vm {
            get {
                return (ControlCenterWindowViewModel)this.DataContext;
            }
        }

        public ControlCenterWindow() {
            InitializeComponent();
            this.Container.Children.Add(new MinersSpeedCharts(this));
        }

        public void ShowThisWindow() {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}
