using NTMiner.Vms;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class MinerClientSetting : BlankWindow {
        public static void ShowWindow(MinerClientSettingViewModel vm) {
            Window window = new MinerClientSetting(vm);
            vm.CloseWindow = () => window.Close();
            window.MousePosition();
            window.ShowSoftDialog();
        }

        public MinerClientSetting(MinerClientSettingViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            var owner = WpfUtil.GetTopWindow();
            if (this != owner) {
                this.Owner = owner;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ButtonState == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}
