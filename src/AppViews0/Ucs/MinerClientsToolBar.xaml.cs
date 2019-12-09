using NTMiner.MinerServer;
using NTMiner.Vms;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerClientsToolBar : UserControl {
        public MinerClientsWindowViewModel Vm {
            get {
                return (MinerClientsWindowViewModel)this.DataContext;
            }
        }

        public MinerClientsToolBar() {
            InitializeComponent();
        }

        private void MenuItemWork_Click(object sender, RoutedEventArgs e) {
            PopMineWork.IsOpen = !PopMineWork.IsOpen;
        }

        private void MenuItemGroup_Click(object sender, RoutedEventArgs e) {
            PopMinerGroup.IsOpen = !PopMinerGroup.IsOpen;
        }

        private void MenuItemUpgrade_Click(object sender, RoutedEventArgs e) {
            RpcRoot.OfficialServer.FileUrlService.GetNTMinerFilesAsync(NTMinerAppType.MinerClient, (ntMinerFiles, ex) => {
                UIThread.Execute(() => {
                    Vm.NTMinerFileList = (ntMinerFiles ?? new List<NTMinerFileData>()).OrderByDescending(a => a.GetVersion()).ToList();
                });
            });
            PopUpgrade.IsOpen = !PopUpgrade.IsOpen;
        }

        private void PopupButton_Click(object sender, RoutedEventArgs e) {
            PopMineWork.IsOpen = PopMinerGroup.IsOpen = PopUpgrade.IsOpen = false;
        }
    }
}
