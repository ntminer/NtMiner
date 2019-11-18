using NTMiner.Vms;
using System.Windows;

namespace NTMiner {
    public partial class App : Application {
        public App() {
            VirtualRoot.SetOut(NotiCenterWindowViewModel.Instance);
            AppUtil.Init(this);
            InitializeComponent();
        }
    }
}
