using NTMiner.Core;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class WalletViewModel : ViewModelBase, IWallet, IEditableViewModel {
        public static readonly WalletViewModel PleaseSelect = new WalletViewModel(Guid.Empty) {
            _name = "不指定",
            _address = string.Empty,
        };

        public static WalletViewModel CreateEmptyWallet(Guid coinId) {
            return new WalletViewModel(Guid.Empty) {
                _name = "无",
                _address = string.Empty,
                _coinId = coinId,
                _sortNumber = 0
            };
        }

        private Guid _id;
        private string _name;
        private Guid _coinId;
        private string _address;
        private int _sortNumber;

        public Guid GetId() {
            return this.Id;
        }

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand SortUp { get; private set; }
        public ICommand SortDown { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public WalletViewModel(IWallet data) : this(data.GetId()) {
            _name = data.Name;
            _coinId = data.CoinId;
            _address = data.Address;
            _sortNumber = data.SortNumber;
        }

        public WalletViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (!this.IsTestWallet) {
                    if (NTMinerRoot.Instance.MinerProfile.TryGetWallet(this.Id, out IWallet wallet)) {
                        VirtualRoot.Execute(new UpdateWalletCommand(this));
                    }
                    else {
                        VirtualRoot.Execute(new AddWalletCommand(this));
                    }
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                VirtualRoot.Execute(new WalletEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (this.IsTestWallet) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{this.Name}钱包吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveWalletCommand(this.Id));
                }));
            });
            this.SortUp = new DelegateCommand(() => {
                WalletViewModel upOne = AppContext.Instance.WalletVms.GetUpOne(this.CoinId, this.SortNumber);
                if (upOne != null) {
                    int sortNumber = upOne.SortNumber;
                    upOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateWalletCommand(upOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateWalletCommand(this));
                    if (AppContext.Instance.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                        coinVm.OnPropertyChanged(nameof(coinVm.Wallets));
                        coinVm.OnPropertyChanged(nameof(coinVm.WalletItems));
                    }
                }
            });
            this.SortDown = new DelegateCommand(() => {
                WalletViewModel nextOne = AppContext.Instance.WalletVms.GetNextOne(this.CoinId, this.SortNumber);
                if (nextOne != null) {
                    int sortNumber = nextOne.SortNumber;
                    nextOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdateWalletCommand(nextOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdateWalletCommand(this));
                    if (AppContext.Instance.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                        coinVm.OnPropertyChanged(nameof(coinVm.Wallets));
                        coinVm.OnPropertyChanged(nameof(coinVm.WalletItems));
                    }
                }
            });
        }

        public Guid Id {
            get => _id;
            private set {
                if (_id != value) {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Name {
            get => _name;
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("别名是必须的");
                    }
                }
            }
        }

        public int SortNumber {
            get => _sortNumber;
            set {
                if (_sortNumber != value) {
                    _sortNumber = value;
                    OnPropertyChanged(nameof(SortNumber));
                }
            }
        }

        public Guid CoinId {
            get => _coinId;
            set {
                if (_coinId != value) {
                    _coinId = value;
                    OnPropertyChanged(nameof(CoinId));
                    OnPropertyChanged(nameof(CoinCode));
                }
            }
        }

        public string CoinCode {
            get {
                if (NTMinerRoot.Instance.ServerContext.CoinSet.TryGetCoin(this.CoinId, out ICoin coin)) {
                    return coin.Code;
                }
                return string.Empty;
            }
        }

        public ICoin Coin {
            get {
                if (NTMinerRoot.Instance.ServerContext.CoinSet.TryGetCoin(this.CoinId, out ICoin coin)) {
                    return coin;
                }
                return CoinViewModel.Empty;
            }
        }

        public bool IsTestWallet {
            get {
                return Coin.GetId() == this.Id;
            }
        }

        public bool IsNotTestWallet {
            get {
                return !IsTestWallet;
            }
        }

        public string Address {
            get => _address;
            set {
                if (_address != value) {
                    _address = value ?? string.Empty;
                    OnPropertyChanged(nameof(Address));
                    if (!string.IsNullOrEmpty(Coin.WalletRegexPattern)) {
                        Regex regex = VirtualRoot.GetRegex(Coin.WalletRegexPattern);
                        if (!regex.IsMatch(value ?? string.Empty)) {
                            throw new ValidationException("钱包地址格式不正确。");
                        }
                    }
                }
            }
        }

        // 钱包地址保存后不能修改
        public bool IsAddressEditable {
            get {
                if (IsTestWallet) {
                    return false;
                }
                if (NTMinerRoot.Instance.MinerProfile.TryGetWallet(this.Id, out IWallet _)) {
                    return false;
                }
                return true;
            }
        }
    }
}
