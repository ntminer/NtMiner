using NTMiner.Vms;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class SplashWindow : Window {
        public SplashWindowViewModel Vm {
            get {
                return (SplashWindowViewModel)this.DataContext;
            }
        }

        public SplashWindow() {
            InitializeComponent();
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}
