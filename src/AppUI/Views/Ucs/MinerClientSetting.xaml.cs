using MahApps.Metro.Controls;
using NTMiner.Vms;
using NTMiner.Wpf;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class MinerClientSetting : MetroWindow {
        public static void ShowWindow(MinerClientSettingViewModel vm) {
            Window window = new MinerClientSetting(vm);
            vm.CloseWindow = () => window.Close();
            if (window.Owner != null) {
                window.MouseBottom();
                double ownerOpacity = window.Owner.Opacity;
                window.Owner.Opacity = 0.6;
                window.ShowDialog();
                window.Owner.Opacity = ownerOpacity;
            }
            else {
                window.ShowDialog();
            }
        }

        private MinerClientSettingViewModel Vm {
            get {
                return (MinerClientSettingViewModel)this.DataContext;
            }
        }
        public MinerClientSetting(MinerClientSettingViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            var owner = TopWindow.GetTopWindow();
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
