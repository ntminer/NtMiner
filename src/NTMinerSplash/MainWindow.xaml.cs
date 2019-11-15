using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace NTMiner {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) {
            try {
                Process parentProcess = Process.GetProcessById(CommandLineArgs.ParentProcessId);
                parentProcess.Kill();
            }
            catch {
            }
            this.Close();
        }
    }
}
