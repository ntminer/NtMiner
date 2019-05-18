using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerProfileOption : UserControl {
        public AppContext.MinerProfileViewModel Vm {
            get {
                return (AppContext.MinerProfileViewModel)this.DataContext;
            }
        }

        public MinerProfileOption() {
            this.DataContext = AppContext.Instance.MinerProfileVm;
            InitializeComponent();
            if (VirtualRoot.IsMinerStudio) {
                this.GroupSystemSetting.Visibility = Visibility.Collapsed;
            }
            else {
                this.GroupSystemSetting.Visibility = Visibility.Visible;
            }
        }

        private void ButtonHotKey_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key >= System.Windows.Input.Key.A && e.Key <= System.Windows.Input.Key.Z) {
                Vm.HotKey = e.Key.ToString();
            }
        }
    }
}
