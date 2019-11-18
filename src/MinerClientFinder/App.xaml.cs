using NTMiner.Views;
using NTMiner.Vms;
using System.Windows;

namespace NTMiner {
    public partial class App : Application {
        public App() {
            VirtualRoot.SetOut(NotiCenterWindowViewModel.Instance);
            AppUtil.Init(this);
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e) {
            NotiCenterWindow.ShowWindow();
            MainWindow = new MainWindow();
            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}
