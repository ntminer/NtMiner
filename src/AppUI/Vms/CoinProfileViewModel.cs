using NTMiner.Core;
using NTMiner.Notifications;
using NTMiner.Profile;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class CoinProfileViewModel : ViewModelBase, ICoinProfile {
        private readonly ICoinProfile _inner;
        public ICommand CopyWallet { get; private set; }
        public ICommand CopyDualCoinWallet { get; private set; }
        public ICommand HideWallet { get; private set; }
        public ICommand ShowWallet { get; private set; }
        public ICommand HideDualCoinWallet { get; private set; }
        public ICommand ShowDualCoinWallet { get; private set; }
        public ICommand AddWallet { get; private set; }
        public ICommand AddDualCoinWallet { get; private set; }

        public CoinProfileViewModel(ICoinProfile innerProfile) {
            _inner = innerProfile;
            this.CopyWallet = new DelegateCommand(() => {
                string wallet = this.Wallet ?? "无";
                Clipboard.SetDataObject(wallet);
                NotiCenterWindowViewModel.Current.Manager.ShowSuccessMessage("复制成功：" + wallet);
            });
            this.CopyDualCoinWallet = new DelegateCommand(() => {
                string wallet = this.DualCoinWallet ?? "无";
                Clipboard.SetDataObject(wallet);
                NotiCenterWindowViewModel.Current.Manager.ShowSuccessMessage("复制成功：" + wallet);
            });
            this.HideWallet = new DelegateCommand(() => {
                this.IsHideWallet = true;
            });
            this.ShowWallet = new DelegateCommand(() => {
                this.IsHideWallet = false;
            });
            this.HideDualCoinWallet = new DelegateCommand(() => {
                this.IsDualCoinHideWallet = true;
            });
            this.ShowDualCoinWallet = new DelegateCommand(() => {
                this.IsDualCoinHideWallet = false;
            });
            this.AddWallet = new DelegateCommand(() => {
                if (CoinViewModels.Current.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                    Guid id = Guid.NewGuid();
                    var wallets = coinVm.Wallets.Where(a => a.IsTestWallet).ToArray();
                    int sortNumber = wallets.Length == 0 ? 1 : wallets.Max(a => a.SortNumber) + 1;
                    new WalletViewModel(id) {
                        CoinId = CoinId,
                        SortNumber = sortNumber
                    }.Edit.Execute(FormType.Add);
                    if (NTMinerRoot.Current.MinerProfile.TryGetWallet(id, out IWallet wallet)) {
                        this.SelectedWallet = WalletViewModels.Current.WalletList.FirstOrDefault(a => a.Id == id);
                    }
                }
            });
            this.AddDualCoinWallet = new DelegateCommand(() => {
                if (CoinViewModels.Current.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                    Guid id = Guid.NewGuid();
                    var wallets = coinVm.Wallets.Where(a => a.IsTestWallet).ToArray();
                    int sortNumber = wallets.Length == 0 ? 1 : wallets.Max(a => a.SortNumber) + 1;
                    new WalletViewModel(id) {
                        CoinId = CoinId,
                        SortNumber = sortNumber
                    }.Edit.Execute(FormType.Add);
                    if (NTMinerRoot.Current.MinerProfile.TryGetWallet(id, out IWallet wallet)) {
                        this.SelectedDualCoinWallet = WalletViewModels.Current.WalletList.FirstOrDefault(a => a.Id == id);
                    }
                }
            });
        }

        public Guid CoinId {
            get { return _inner.CoinId; }
        }

        public Guid PoolId {
            get => _inner.PoolId;
            set {
                if (_inner.PoolId != value) {
                    NTMinerRoot.Current.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(PoolId), value);
                    OnPropertyChanged(nameof(PoolId));
                    OnPropertyChanged(nameof(SelectedWallet));
                }
            }
        }

        public string Wallet {
            get => _inner.Wallet;
            set {
                if (_inner.Wallet != value) {
                    NTMinerRoot.Current.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(Wallet), value ?? string.Empty);
                    OnPropertyChanged(nameof(Wallet));
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                }
            }
        }

        public WalletViewModel SelectedWallet {
            get {
                if (!CoinViewModels.Current.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                    return null;
                }
                WalletViewModel walletVm = coinVm.Wallets.FirstOrDefault(a => a.Address == this.Wallet);
                if (walletVm == null) {
                    walletVm = coinVm.Wallets.FirstOrDefault();
                    if (walletVm != null) {
                        this.Wallet = walletVm.Address;
                    }
                }
                return walletVm;
            }
            set {
                if (value == null) {
                    if (CoinViewModels.Current.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                        value = coinVm.Wallets.FirstOrDefault();
                    }
                }
                if (value != null) {
                    this.Wallet = value.Address;
                    OnPropertyChanged(nameof(SelectedWallet));
                }
            }
        }

        public bool IsHideWallet {
            get => _inner.IsHideWallet;
            set {
                if (_inner.IsHideWallet != value) {
                    NTMinerRoot.Current.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(IsHideWallet), value);
                    OnPropertyChanged(nameof(IsHideWallet));
                    OnPropertyChanged(nameof(IsShowWallet));
                }
            }
        }

        public bool IsShowWallet {
            get {
                return !IsHideWallet;
            }
        }

        public Guid CoinKernelId {
            get => _inner.CoinKernelId;
            set {
                if (_inner.CoinKernelId != value) {
                    NTMinerRoot.Current.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(CoinKernelId), value);
                    OnPropertyChanged(nameof(CoinKernelId));
                }
            }
        }

        public PoolViewModel MainCoinPool {
            get {
                if (this.CoinId == Guid.Empty) {
                    return null;
                }

                if (!CoinViewModels.Current.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                    return null;
                }
                if (!PoolViewModels.Current.TryGetPoolVm(this.PoolId, out PoolViewModel pool)) {
                    pool = coinVm.Pools.OrderBy(a => a.SortNumber).FirstOrDefault();
                    if (pool != null) {
                        PoolId = pool.Id;
                    }
                }
                return pool;
            }
            set {
                if (value == null) {
                    if (CoinViewModels.Current.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                        value = coinVm.Pools.OrderBy(a => a.SortNumber).FirstOrDefault();
                    }
                }
                if (value != null) {
                    PoolId = value.Id;
                    OnPropertyChanged(nameof(MainCoinPool));
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                }
            }
        }

        public Guid DualCoinPoolId {
            get => _inner.DualCoinPoolId;
            set {
                if (_inner.DualCoinPoolId != value) {
                    NTMinerRoot.Current.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(DualCoinPoolId), value);
                    OnPropertyChanged(nameof(DualCoinPoolId));
                }
            }
        }

        public string DualCoinWallet {
            get => _inner.DualCoinWallet;
            set {
                if (_inner.DualCoinWallet != value) {
                    NTMinerRoot.Current.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(DualCoinWallet), value ?? string.Empty);
                    OnPropertyChanged(nameof(DualCoinWallet));
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                }
            }
        }

        public WalletViewModel SelectedDualCoinWallet {
            get {
                if (!CoinViewModels.Current.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                    return null;
                }
                WalletViewModel walletVm = coinVm.Wallets.FirstOrDefault(a => a.Address == this.DualCoinWallet);
                if (walletVm == null) {
                    walletVm = coinVm.Wallets.FirstOrDefault();
                    if (walletVm != null) {
                        this.DualCoinWallet = walletVm.Address;
                    }
                }
                return walletVm;
            }
            set {
                if (value == null) {
                    if (CoinViewModels.Current.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                        value = coinVm.Wallets.FirstOrDefault();
                    }
                }
                if (value != null) {
                    this.DualCoinWallet = value.Address;
                    OnPropertyChanged(nameof(SelectedDualCoinWallet));
                }
            }
        }

        public bool IsDualCoinHideWallet {
            get => _inner.IsDualCoinHideWallet;
            set {
                if (_inner.IsDualCoinHideWallet != value) {
                    NTMinerRoot.Current.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(IsDualCoinHideWallet), value);
                    OnPropertyChanged(nameof(IsDualCoinHideWallet));
                    OnPropertyChanged(nameof(IsDualCoinShowWallet));
                }
            }
        }

        public bool IsDualCoinShowWallet {
            get {
                return !IsDualCoinHideWallet;
            }
        }

        public PoolViewModel DualCoinPool {
            get {
                if (!CoinViewModels.Current.TryGetCoinVm(CoinId, out CoinViewModel coinVm)) {
                    return null;
                }
                if (!PoolViewModels.Current.TryGetPoolVm(this.DualCoinPoolId, out PoolViewModel pool)) {
                    pool = coinVm.Pools.OrderBy(a => a.SortNumber).FirstOrDefault();
                    if (pool != null) {
                        DualCoinPoolId = pool.Id;
                    }
                }
                return pool;
            }
            set {
                if (value == null) {
                    if (CoinViewModels.Current.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                        value = coinVm.Pools.OrderBy(a => a.SortNumber).FirstOrDefault();
                    }
                }
                if (value != null) {
                    DualCoinPoolId = value.Id;
                    OnPropertyChanged(nameof(DualCoinPool));
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                }
            }
        }
    }
}
