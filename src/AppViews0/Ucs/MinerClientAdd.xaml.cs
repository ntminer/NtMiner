using NTMiner.Vms;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class MinerClientAdd : BlankWindow {
        public static void ShowWindow() {
            MinerClientAddViewModel vm = new MinerClientAddViewModel();
            Window window = new MinerClientAdd(vm);
            vm.CloseWindow = () => window.Close();
            window.MousePosition();
            window.ShowSoftDialog();
        }

        public MinerClientAdd(MinerClientAddViewModel vm) {
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
