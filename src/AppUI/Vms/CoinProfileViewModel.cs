using NTMiner.MinerServer;
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
            this.CopyWallet = new DelegateCommand(() =>
            {
                string wallet = this.Wallet ?? "无";
                Clipboard.SetDataObject(wallet);
                MainWindowViewModel.Current.Manager.CreateMessage()
                    .Accent("#1751C3")
                    .Background("#333")
                    .HasBadge("Info")
                    .HasMessage("复制成功：" + wallet)
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(4))
                    .Queue();
            });
            this.CopyDualCoinWallet = new DelegateCommand(() =>
            {
                string wallet = this.DualCoinWallet ?? "无";
                Clipboard.SetDataObject(wallet);
                MainWindowViewModel.Current.Manager.CreateMessage()
                    .Accent("#1751C3")
                    .Background("#333")
                    .HasBadge("Info")
                    .HasMessage("复制成功：" + wallet)
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(4))
                    .Queue();
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
            this.AddWallet = new DelegateCommand(() =>
            {
                CoinViewModel coinVm;
                if (CoinViewModels.Current.TryGetCoinVm(this.CoinId, out coinVm)) {
                    Guid id = Guid.NewGuid();
                    var wallets = coinVm.Wallets.Where(a => a.IsTestWallet).ToArray();
                    int sortNumber = wallets.Length == 0 ? 1 : wallets.Max(a => a.SortNumber) + 1;
                    new WalletViewModel(id) {
                        CoinId = CoinId,
                        SortNumber = sortNumber
                    }.Edit.Execute(FormType.Add);
                    IWallet wallet;
                    if (NTMinerRoot.Current.MinerProfile.TryGetWallet(id, out wallet)) {
                        this.SelectedWallet = WalletViewModels.Current.WalletList.FirstOrDefault(a => a.Id == id);
                    }
                }
            });
            this.AddDualCoinWallet = new DelegateCommand(() =>
            {
                CoinViewModel coinVm;
                if (CoinViewModels.Current.TryGetCoinVm(this.CoinId, out coinVm)) {
                    Guid id = Guid.NewGuid();
                    var wallets = coinVm.Wallets.Where(a => a.IsTestWallet).ToArray();
                    int sortNumber = wallets.Length == 0 ? 1 : wallets.Max(a => a.SortNumber) + 1;
                    new WalletViewModel(id) {
                        CoinId = CoinId,
                        SortNumber = sortNumber
                    }.Edit.Execute(FormType.Add);
                    IWallet wallet;
                    if (NTMinerRoot.Current.MinerProfile.TryGetWallet(id, out wallet)) {
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
                    NTMinerRoot.Current.SetCoinProfileProperty(this.CoinId, nameof(PoolId), value);
                    OnPropertyChanged(nameof(PoolId));
                    OnPropertyChanged(nameof(SelectedWallet));
                }
            }
        }

        public string Wallet {
            get => _inner.Wallet;
            set {
                if (_inner.Wallet != value) {
                    NTMinerRoot.Current.SetCoinProfileProperty(this.CoinId, nameof(Wallet), value ?? string.Empty);
                    OnPropertyChanged(nameof(Wallet));
                    VirtualRoot.Execute(new RefreshArgsAssemblyCommand());
                }
            }
        }

        public WalletViewModel SelectedWallet {
            get {
                CoinViewModel coinVm = CoinViewModels.Current[this.CoinId];
                WalletViewModel walletVm = coinVm.Wallets.FirstOrDefault(a => a.CoinId == this.CoinId && a.Address == this.Wallet);
                if (walletVm == null) {
                    walletVm = coinVm.Wallets.FirstOrDefault(a => a.CoinId == this.CoinId);
                    if (walletVm != null) {
                        this.Wallet = walletVm.Address;
                    }
                }
                return walletVm;
            }
            set {
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
                    NTMinerRoot.Current.SetCoinProfileProperty(this.CoinId, nameof(IsHideWallet), value);
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
                    NTMinerRoot.Current.SetCoinProfileProperty(this.CoinId, nameof(CoinKernelId), value);
                    OnPropertyChanged(nameof(CoinKernelId));
                }
            }
        }

        public PoolViewModel MainCoinPool {
            get {
                if (this.CoinId == Guid.Empty) {
                    return null;
                }
                var pools = PoolViewModels.Current.AllPools.Where(a => a.CoinId == this.CoinId);
                PoolViewModel pool = pools.FirstOrDefault(a => a.Id == PoolId);
                if (pool == null) {
                    pool = pools.FirstOrDefault();
                    if (pool != null) {
                        PoolId = pool.Id;
                    }
                }
                if (pool != null && !pool.IsCurrentPool) {
                    foreach (var poolVm in pools) {
                        poolVm.IsCurrentPool = false;
                    }
                    pool.IsCurrentPool = true;
                }
                return pool;
            }
            set {
                if (value != null && value.Id != Guid.Empty) {
                    PoolId = value.Id;
                    OnPropertyChanged(nameof(MainCoinPool));
                    VirtualRoot.Execute(new RefreshArgsAssemblyCommand());
                }
            }
        }

        public Guid DualCoinPoolId {
            get => _inner.DualCoinPoolId;
            set {
                if (_inner.DualCoinPoolId != value) {
                    NTMinerRoot.Current.SetCoinProfileProperty(this.CoinId, nameof(DualCoinPoolId), value);
                    OnPropertyChanged(nameof(DualCoinPoolId));
                }
            }
        }

        public string DualCoinWallet {
            get => _inner.DualCoinWallet;
            set {
                if (_inner.DualCoinWallet != value) {
                    NTMinerRoot.Current.SetCoinProfileProperty(this.CoinId, nameof(DualCoinWallet), value ?? string.Empty);
                    OnPropertyChanged(nameof(DualCoinWallet));
                    VirtualRoot.Execute(new RefreshArgsAssemblyCommand());
                }
            }
        }

        public WalletViewModel SelectedDualCoinWallet {
            get {
                CoinViewModel coinVm = CoinViewModels.Current[this.CoinId];
                WalletViewModel walletVm = coinVm.Wallets.FirstOrDefault(a => a.CoinId == this.CoinId && a.Address == this.DualCoinWallet);
                if (walletVm == null) {
                    walletVm = coinVm.Wallets.FirstOrDefault(a => a.CoinId == this.CoinId);
                    if (walletVm != null) {
                        this.DualCoinWallet = walletVm.Address;
                    }
                }
                return walletVm;
            }
            set {
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
                    NTMinerRoot.Current.SetCoinProfileProperty(this.CoinId, nameof(IsDualCoinHideWallet), value);
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

        public bool IsOverClockEnabled {
            get { return _inner.IsOverClockEnabled; }
            set {
                NTMinerRoot.Current.SetCoinProfileProperty(this.CoinId, nameof(IsOverClockEnabled), value);
                OnPropertyChanged(nameof(IsOverClockEnabled));
            }
        }

        public bool IsOverClockGpuAll {
            get { return _inner.IsOverClockGpuAll; }
            set {
                NTMinerRoot.Current.SetCoinProfileProperty(this.CoinId, nameof(IsOverClockGpuAll), value);
                OnPropertyChanged(nameof(IsOverClockGpuAll));
            }
        }

        public PoolViewModel DualCoinPool {
            get {
                var dualCoinPools = CoinViewModels.Current[this.CoinId].Pools;
                PoolViewModel pool = dualCoinPools.FirstOrDefault(a => a.Id == DualCoinPoolId);
                if (pool == null) {
                    pool = dualCoinPools.FirstOrDefault();
                    if (pool != null) {
                        DualCoinPoolId = pool.Id;
                    }
                }
                return pool;
            }
            set {
                if (value != null && value.Id != Guid.Empty) {
                    DualCoinPoolId = value.Id;
                    OnPropertyChanged(nameof(DualCoinPool));
                    VirtualRoot.Execute(new RefreshArgsAssemblyCommand());
                }
            }
        }
    }
}
