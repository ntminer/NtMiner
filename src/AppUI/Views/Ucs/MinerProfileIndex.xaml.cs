using NTMiner.Notifications;
using NTMiner.Vms;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerProfileIndex : UserControl {
        private MinerProfileIndexViewModel Vm {
            get {
                return (MinerProfileIndexViewModel)this.DataContext;
            }
        }

        public MinerProfileIndex() {
            InitializeComponent();
        }

        private INotificationMessage _walletNm;
        private void TbMainWallet_TextChanged(object sender, TextChangedEventArgs e) {
            CoinViewModel coin = Vm.MinerProfile.CoinVm;
            if (coin == null) {
                return;
            }
            if (!string.IsNullOrEmpty(coin.WalletRegexPattern)) {
                Regex regex = new Regex(coin.WalletRegexPattern);
                Match match = regex.Match(CbMainWallet.Text ?? string.Empty);
                if (!match.Success) {
                    if (_walletNm == null) {
                        _walletNm = MainWindowViewModel.Current.Manager
                           .CreateMessage()
                           .Warning("主币钱包地址格式不正确。")
                           .Dismiss().WithButton("忽略", null)
                           .Queue();
                    }
                    else {
                        MainWindowViewModel.Current.Manager.Queue(_walletNm);
                    }
                }
                else {
                    if (_walletNm != null) {
                        MainWindowViewModel.Current.Manager.Dismiss(_walletNm);
                        _walletNm = null;
                    }
                }
            }
        }

        private INotificationMessage _dualCoinWalletNm;
        private void TbDualCoinWallet_TextChanged(object sender, TextChangedEventArgs e) {
            CoinViewModel coin = Vm.MinerProfile.CoinVm?.CoinKernel?.CoinKernelProfile?.SelectedDualCoin;
            if (coin == null) {
                return;
            }
            if (!string.IsNullOrEmpty(coin.WalletRegexPattern)) {
                Regex regex = new Regex(coin.WalletRegexPattern);
                Match match = regex.Match(CbDualCoinWallet.Text ?? string.Empty);
                if (!match.Success) {
                    if (_dualCoinWalletNm == null) {
                        _dualCoinWalletNm = MainWindowViewModel.Current.Manager
                           .CreateMessage()
                           .Warning("双挖币钱包地址格式不正确。")
                           .Dismiss().WithButton("忽略", null)
                           .Queue();
                    }
                    else {
                        MainWindowViewModel.Current.Manager.Queue(_dualCoinWalletNm);
                    }
                }
                else {
                    if (_dualCoinWalletNm != null) {
                        MainWindowViewModel.Current.Manager.Dismiss(_dualCoinWalletNm);
                    }
                }
            }
        }
    }
}
