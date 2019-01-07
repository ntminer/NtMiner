using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class SplashWindow : Window {
        public SplashWindow() {
            InitializeComponent();
        }

        private void MetroWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}
