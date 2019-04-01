using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class ControlCenterHostConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "群控服务器",
                IsDialogWindow = true,
                IconName = "Icon_Server",
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new ControlCenterHostConfig();
                var vm = (ControlCenterHostConfigViewModel)uc.DataContext;
                vm.CloseWindow = () => window.Close();
                return uc;
            }, fixedSize: true);
        }

        public ControlCenterHostConfigViewModel Vm {
            get {
                return (ControlCenterHostConfigViewModel)this.DataContext;
            }
        }

        public ControlCenterHostConfig() {
            InitializeComponent();
        }
    }
}
