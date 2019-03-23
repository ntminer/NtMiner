using System.Windows;
using System.Windows.Input;
using NTMiner.Vms;

namespace NTMiner.Views {
    public partial class NotiCenterWindow : Window {
        public static readonly NotiCenterWindow Instance = new NotiCenterWindow();

        public NotiCenterWindowViewModel Vm {
            get { return NotiCenterWindowViewModel.Current; }
        }

        private NotiCenterWindow() {
            this.DataContext = Vm;
            InitializeComponent();
            this.Topmost = true;
            this.Left = (SystemParameters.FullPrimaryScreenWidth - this.Width) / 2;
            this.Top = 10;
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void Window_StateChanged(object sender, System.EventArgs e) {
            if (WindowState != WindowState.Normal) {
                WindowState = WindowState.Normal;
            }
        }
    }
}
