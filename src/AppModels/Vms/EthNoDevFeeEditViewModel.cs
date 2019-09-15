using NTMiner.Core;
using NTMiner.MinerServer;
using System;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class EthNoDevFeeEditViewModel : ViewModelBase {
        private string _ethNoDevFeeWallet;

        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public EthNoDevFeeEditViewModel() {
            _ethNoDevFeeWallet = GetEthNoDevFeeWallet();
            this.Save = new DelegateCommand(() => {                
                AppSettingData appSettingData = new AppSettingData() {
                    Key = nameof(EthNoDevFeeWallet),
                    Value = EthNoDevFeeWallet
                };
                VirtualRoot.Execute(new ChangeLocalAppSettingCommand(appSettingData));
                CloseWindow?.Invoke();
            });
        }

        public static string GetEthNoDevFeeWallet() {
            if (NTMinerRoot.Instance.LocalAppSettingSet.TryGetAppSetting(nameof(EthNoDevFeeWallet), out IAppSetting appSetting)) {
                return (string)appSetting.Value;
            }
            return string.Empty;
        }

        public string EthNoDevFeeWallet {
            get {
                return _ethNoDevFeeWallet;
            }
            set {
                if (_ethNoDevFeeWallet != value) {
                    _ethNoDevFeeWallet = value;
                    OnPropertyChanged(nameof(EthNoDevFeeWallet));
                    if (NTMinerRoot.Instance.CoinSet.TryGetCoin("ETH", out ICoin coin)) {
                        if (!string.IsNullOrEmpty(coin.WalletRegexPattern)) {
                            Regex regex = new Regex(coin.WalletRegexPattern);
                            if (!regex.IsMatch(value)) {
                                throw new ValidationException("钱包地址格式不正确。");
                            }
                        }
                    }
                }
            }
        }
    }
}
