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
                Kill(CommandLineArgs.ParentProcessId);
            }
            catch {
            }
            this.Close();
        }


        /// <summary>
        /// 不会抛出异常，因为吞掉了异常
        /// </summary>
        private static void Kill(int pid) {
            try {
                if (pid <= 0) {
                    return;
                }
                string args = $"/F /T /PID {pid}";
                using (Process proc = new Process()) {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = $"/C taskkill {args}";
                    proc.Start();
                }
            }
            catch {
            }
        }
    }
}
