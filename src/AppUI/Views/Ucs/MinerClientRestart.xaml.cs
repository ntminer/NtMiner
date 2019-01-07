using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerClientRestart : UserControl {
        public static void ShowWindow(MinerClientViewModel minerClientVm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Restart",
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                var vm = new MinerClientRestartViewModel(minerClientVm);
                vm.CloseWindow = () => window.Close();
                return new MinerClientRestart(vm);
            }, fixedSize: true);
        }

        public MinerClientRestartViewModel Vm {
            get {
                return (MinerClientRestartViewModel)this.DataContext;
            }
        }

        public MinerClientRestart(MinerClientRestartViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
