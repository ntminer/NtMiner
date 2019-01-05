using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerServerHostConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Server",
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new MinerServerHostConfig();
                var vm = (MinerServerHostConfigViewModel)uc.DataContext;
                vm.CloseWindow = () => window.Close();
                return uc;
            }, fixedSize: true);
        }

        public MinerServerHostConfigViewModel Vm {
            get {
                return (MinerServerHostConfigViewModel)this.DataContext;
            }
        }

        public MinerServerHostConfig() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
