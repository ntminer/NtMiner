using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class CoinPage : UserControl {
        public static void ShowWindow(CoinViewModel currentCoin, string tabType) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "币种",
                IconName = "Icon_Coin",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = DevMode.IsDebugMode ? AppStatic.MainWindowWidth : 960,
                Height = 520
            },
            ucFactory: (window) => new CoinPage(),
            beforeShow: (window, uc) => {
                if (currentCoin != null) {
                    switch (tabType) {
                        case NTKeyword.PoolParameterName:
                            uc.Vm.IsPoolTabSelected = true;
                            break;
                        case NTKeyword.WalletParameterName:
                            uc.Vm.IsWalletTabSelected = true;
                            break;
                        default:
                            break;
                    }
                    uc.Vm.CurrentCoin = currentCoin;
                }
            });
        }

        public CoinPageViewModel Vm {
            get {
                return (CoinPageViewModel)this.DataContext;
            }
        }

        public CoinPage() {
            InitializeComponent();
            AppContext.Instance.CoinVms.PropertyChanged += Current_PropertyChanged;
            this.Unloaded += CoinPage_Unloaded;
        }

        private void CoinPage_Unloaded(object sender, System.Windows.RoutedEventArgs e) {
            AppContext.Instance.CoinVms.PropertyChanged -= Current_PropertyChanged;
        }

        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(AppContext.Instance.CoinVms.AllCoins)) {
                Vm.OnPropertyChanged(nameof(Vm.QueryResults));
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_MouseDoubleClick<CoinViewModel>(sender, e);
        }

        private void WalletDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_MouseDoubleClick<WalletViewModel>(sender, e);
        }

        private void PoolDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_MouseDoubleClick<PoolViewModel>(sender, e);
        }

        private void KernelDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_MouseDoubleClick<CoinKernelViewModel>(sender, e);
        }

        private void ButtonAddCoinKernel_Click(object sender, System.Windows.RoutedEventArgs e) {
            var coinVm = Vm.CurrentCoin;
            if (coinVm == null) {
                return;
            }
            PopupKernel.Child = new KernelSelect(
                new KernelSelectViewModel(coinVm, null, onOk: selectedResult => {
                    if (selectedResult != null) {
                        VirtualRoot.Execute(new AddCoinKernelCommand(new CoinKernelViewModel(Guid.NewGuid()) {
                            Args = string.Empty,
                            CoinId = coinVm.Id,
                            KernelId = selectedResult.Id
                        }));
                        PopupKernel.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        PopupKernel.IsOpen = false;
                    })
                });
            PopupKernel.IsOpen = true;
        }
    }
}
