using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerServerHostConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Server",
                CloseVisible = System.Windows.Visibility.Visible,
                SaveVisible = System.Windows.Visibility.Visible,
                OnOk = uc => {
                    try {
                        var vm = (MinerServerHostConfigViewModel)uc.DataContext;
                        if (string.IsNullOrEmpty(vm.MinerServerHost)) {
                            vm.MinerServerHost = Server.MINER_SERVER_HOST;
                        }
                        string serverPubKey = Server.TimeService.GetServerPubKey(vm.MinerServerHost);
                        Server.MinerServerPubKey = serverPubKey;
                        Server.MinerServerHost = vm.MinerServerHost;
                        return true;
                    }
                    catch (System.Exception e) {
                        Global.Logger.Error(e.Message, e);
                        return false;
                    }
                }
            }, ucFactory: (window) => new MinerServerHostConfig(), fixedSize: true);
        }

        public MinerServerHostConfigViewModel Vm {
            get {
                return (MinerServerHostConfigViewModel)this.DataContext;
            }
        }

        public MinerServerHostConfig() {
            InitializeComponent();
        }
    }
}
