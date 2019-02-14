using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class CoinPage : UserControl {
        public static void ShowWindow(CoinViewModel currentCoin) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Coin",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = DevMode.IsDebugMode ? 960 : 860,
                Height = 520
            }, 
            ucFactory: (window) => new CoinPage(),
            beforeShow: uc => {
                if (currentCoin != null) {
                    CoinPageViewModel vm = (CoinPageViewModel)uc.DataContext;
                    vm.CurrentCoin = currentCoin;
                }
            });
        }

        private CoinPageViewModel Vm {
            get {
                return (CoinPageViewModel)this.DataContext;
            }
        }

        public CoinPage() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            Point p = e.GetPosition(dg);
            if (p.Y < 30) {
                return;
            }
            if (Vm.CurrentCoin != null) {
                Vm.CurrentCoin.Edit.Execute(null);
            }
        }

        private void WalletDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            Point p = e.GetPosition(dg);
            if (p.Y < 30) {
                return;
            }
            if (dg.SelectedItem != null) {
                WalletViewModel walletVm = (WalletViewModel)dg.SelectedItem;
                walletVm.Edit.Execute(null);
            }
        }

        private void PoolDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            Point p = e.GetPosition(dg);
            if (p.Y < 30) {
                return;
            }
            if (dg.SelectedItem != null) {
                PoolViewModel poolVm = (PoolViewModel)dg.SelectedItem;
                poolVm.Edit.Execute(null);
            }
        }

        private void KernelDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            Point p = e.GetPosition(dg);
            if (p.Y < 30) {
                return;
            }
            if (dg.SelectedItem != null) {
                CoinKernelViewModel kernelVm = (CoinKernelViewModel)dg.SelectedItem;
                kernelVm.Edit.Execute(null);
            }
        }
    }
}
