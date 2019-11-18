using System.Windows;
using System.Windows.Input;

namespace NTMiner {
    public partial class MainWindow : BlankWindow {
        public MainWindow() {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}
