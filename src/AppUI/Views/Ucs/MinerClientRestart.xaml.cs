using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerClientRestart : UserControl {
        public static void ShowWindow(MinerClientViewModel minerClientVm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Restart",
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                SaveVisible = System.Windows.Visibility.Visible,
                OnOk = uc => {
                    var vm = (MinerClientRestartViewModel)uc.DataContext;
                    NTMinerClientDaemon.Instance.RestartNTMiner(minerClientVm.ClientDataVm.MinerIp, Global.ClientPort, vm.SelectedMineWork.Id, null);
                    return true;
                }
            }, ucFactory: (window) => new MinerClientRestart(new MinerClientRestartViewModel(minerClientVm)), fixedSize: true);
        }

        public MinerClientRestartViewModel Vm {
            get {
                return (MinerClientRestartViewModel)this.DataContext;
            }
        }

        public MinerClientRestart(MinerClientRestartViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
