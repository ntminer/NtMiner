using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class ControlCenterHostConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
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
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
