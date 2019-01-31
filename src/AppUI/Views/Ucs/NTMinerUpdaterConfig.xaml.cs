using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class NTMinerUpdaterConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Update",
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new NTMinerUpdaterConfig();
                var vm = (NTMinerUpdaterConfigViewModel)uc.DataContext;
                vm.CloseWindow = () => window.Close();
                return uc;
            }, fixedSize: true);
        }

        public NTMinerUpdaterConfigViewModel Vm {
            get {
                return (NTMinerUpdaterConfigViewModel)this.DataContext;
            }
        }

        public NTMinerUpdaterConfig() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
