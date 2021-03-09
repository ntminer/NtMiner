using NTMiner.Core;
using NTMiner.Core.Profile;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class CoinProfileViewModel : ViewModelBase, ICoinProfile {
        private readonly ICoinProfile _inner;
        public ICommand Up { get; private set; }
        public ICommand Down { get; private set; }
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
            this.Up = new DelegateCommand<string>(propertyName => {
                WpfUtil.Up(this, propertyName);
            });
            this.Down = new DelegateCommand<string>(propertyName => {
                WpfUtil.Down(this, propertyName);
            });
            this.CopyWallet = new DelegateCommand(() => {
                string wallet = this.Wallet ?? "无";
                Clipboard.SetDataObject(wallet, true);
                VirtualRoot.Out.ShowSuccess(wallet, header: "复制成功");
            }, ()=> {
                return this.SelectedWallet != null && !string.IsNullOrEmpty(this.SelectedWallet.Address);
            });
            this.CopyDualCoinWallet = new DelegateCommand(() => {
                string wallet = this.DualCoinWallet ?? "无";
                Clipboard.SetDataObject(wallet, true);
                VirtualRoot.Out.ShowSuccess(wallet, header: "复制成功");
            }, ()=> {
                return this.SelectedDualCoinWallet != null && !string.IsNullOrEmpty(this.SelectedDualCoinWallet.Address);
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
                if (AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                    Guid id = Guid.NewGuid();
                    var wallets = coinVm.Wallets.Where(a => a.IsTestWallet).ToArray();
                    int sortNumber = wallets.Length == 0 ? 1 : wallets.Max(a => a.SortNumber) + 1;
                    new WalletViewModel(id) {
                        CoinId = CoinId,
                        SortNumber = sortNumber,
                        AfterClose = () => {
                            if (NTMinerContext.Instance.MinerProfile.TryGetWallet(id, out IWallet wallet)) {
                                this.SelectedWallet = AppRoot.WalletVms.WalletList.FirstOrDefault(a => a.Id == id);
                            }
                        }
                    }.Edit.Execute(FormType.Add);
                }
            });
            this.AddDualCoinWallet = new DelegateCommand(() => {
                if (AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                    Guid id = Guid.NewGuid();
                    var wallets = coinVm.Wallets.Where(a => a.IsTestWallet).ToArray();
                    int sortNumber = wallets.Length == 0 ? 1 : wallets.Max(a => a.SortNumber) + 1;
                    new WalletViewModel(id) {
                        CoinId = CoinId,
                        SortNumber = sortNumber,
                        AfterClose = () => {
                            if (NTMinerContext.Instance.MinerProfile.TryGetWallet(id, out IWallet wallet)) {
                                this.SelectedDualCoinWallet = AppRoot.WalletVms.WalletList.FirstOrDefault(a => a.Id == id);
                            }
                        }
                    }.Edit.Execute(FormType.Add);                    
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
                    NTMinerContext.Instance.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(PoolId), value);
                    OnPropertyChanged(nameof(PoolId));
                    OnPropertyChanged(nameof(SelectedWallet));
                }
            }
        }

        public Guid PoolId1 {
            get => _inner.PoolId1;
            set {
                if (_inner.PoolId1 != value) {
                    NTMinerContext.Instance.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(PoolId1), value);
                    OnPropertyChanged(nameof(PoolId1));
                }
            }
        }

        public string Wallet {
            get => _inner.Wallet;
            set {
                if (_inner.Wallet != value) {
                    NTMinerContext.Instance.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(Wallet), value ?? string.Empty);
                    OnPropertyChanged(nameof(Wallet));
                    // 不必判断该对象是否是主界面上当前展示的对象，因为若不是主界面上当前显式的对象的话没有机会变更
                    NTMinerContext.RefreshArgsAssembly.Invoke("币种Profile上放置的挖矿钱包发生了变更");
                }
            }
        }

        public WalletViewModel SelectedWallet {
            get {
                if (!AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
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
                    if (AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
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
                    NTMinerContext.Instance.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(IsHideWallet), value);
                    OnPropertyChanged(nameof(IsHideWallet));
                }
            }
        }

        public Guid CoinKernelId {
            get => _inner.CoinKernelId;
            set {
                if (_inner.CoinKernelId != value) {
                    NTMinerContext.Instance.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(CoinKernelId), value);
                    OnPropertyChanged(nameof(CoinKernelId));
                }
            }
        }

        public PoolViewModel MainCoinPool {
            get {
                if (this.CoinId == Guid.Empty) {
                    return null;
                }

                if (!AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                    return null;
                }
                if (!AppRoot.PoolVms.TryGetPoolVm(this.PoolId, out PoolViewModel pool)) {
                    pool = coinVm.Pools.OrderBy(a => a.SortNumber).FirstOrDefault();
                    if (pool != null) {
                        PoolId = pool.Id;
                    }
                }
                return pool;
            }
            set {
                if (value == null) {
                    if (AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                        value = coinVm.Pools.OrderBy(a => a.SortNumber).FirstOrDefault();
                    }
                }
                if (value != null) {
                    PoolId = value.Id;
                    OnPropertyChanged(nameof(MainCoinPool));
                    // 不必判断该对象是否是主界面上当前展示的对象，因为若不是主界面上当前显式的对象的话没有机会变更
                    NTMinerContext.RefreshArgsAssembly.Invoke("币种Profile上引用的主挖币种的主挖矿池发生了切换");
                }
                AppRoot.MinerProfileVm.OnPropertyChanged(nameof(AppRoot.MinerProfileVm.IsAllMainCoinPoolIsUserMode));
            }
        }

        public PoolViewModel MainCoinPool1 {
            get {
                if (this.CoinId == Guid.Empty) {
                    return null;
                }

                if (!AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                    return null;
                }
                if (!AppRoot.PoolVms.TryGetPoolVm(this.PoolId1, out PoolViewModel pool)) {
                    pool = coinVm.Pools.OrderBy(a => a.SortNumber).FirstOrDefault();
                    if (pool != null) {
                        PoolId1 = pool.Id;
                    }
                }
                return pool;
            }
            set {
                if (value == null) {
                    if (AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                        value = coinVm.Pools.OrderBy(a => a.SortNumber).FirstOrDefault();
                    }
                }
                if (value != null) {
                    PoolId1 = value.Id;
                    OnPropertyChanged(nameof(MainCoinPool1));
                    // 不必判断该对象是否是主界面上当前展示的对象，因为若不是主界面上当前显式的对象的话没有机会变更
                    NTMinerContext.RefreshArgsAssembly.Invoke("币种Profile上引用的主挖币种的备用矿池发生了切换");
                }
                AppRoot.MinerProfileVm.OnPropertyChanged(nameof(AppRoot.MinerProfileVm.IsAllMainCoinPoolIsUserMode));
            }
        }

        public Guid DualCoinPoolId {
            get => _inner.DualCoinPoolId;
            set {
                if (_inner.DualCoinPoolId != value) {
                    NTMinerContext.Instance.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(DualCoinPoolId), value);
                    OnPropertyChanged(nameof(DualCoinPoolId));
                }
            }
        }

        public string DualCoinWallet {
            get => _inner.DualCoinWallet;
            set {
                if (_inner.DualCoinWallet != value) {
                    NTMinerContext.Instance.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(DualCoinWallet), value ?? string.Empty);
                    OnPropertyChanged(nameof(DualCoinWallet));
                    // 不必判断该对象是否是主界面上当前展示的对象，因为若不是主界面上当前显式的对象的话没有机会变更
                    NTMinerContext.RefreshArgsAssembly.Invoke("币种Profile上放置的双挖钱包发生了变更");
                }
            }
        }

        public WalletViewModel SelectedDualCoinWallet {
            get {
                if (!AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
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
                    if (AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
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
                    NTMinerContext.Instance.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(IsDualCoinHideWallet), value);
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

        public double CalcInput {
            get { return _inner.CalcInput; }
            set {
                if (_inner.CalcInput != value) {
                    NTMinerContext.Instance.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(CalcInput), value);
                    OnPropertyChanged(nameof(CalcInput));
                }
            }
        }

        public bool IsLowSpeedRestartComputer {
            get => _inner.IsLowSpeedRestartComputer;
            set {
                if (_inner.IsLowSpeedRestartComputer != value) {
                    NTMinerContext.Instance.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(IsLowSpeedRestartComputer), value);
                    OnPropertyChanged(nameof(IsLowSpeedRestartComputer));
                }
            }
        }

        public int LowSpeedRestartComputerMinutes {
            get => _inner.LowSpeedRestartComputerMinutes;
            set {
                if (_inner.LowSpeedRestartComputerMinutes != value) {
                    NTMinerContext.Instance.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(LowSpeedRestartComputerMinutes), value);
                    OnPropertyChanged(nameof(LowSpeedRestartComputerMinutes));
                }
            }
        }

        public double LowSpeed {
            get => _inner.LowSpeed;
            set {
                if (_inner.LowSpeed != value) {
                    NTMinerContext.Instance.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(LowSpeed), value);
                    OnPropertyChanged(nameof(LowSpeed));
                }
            }
        }

        public bool IsLowSpeedReOverClock {
            get => _inner.IsLowSpeedReOverClock;
            set {
                if (_inner.IsLowSpeedReOverClock != value) {
                    NTMinerContext.Instance.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(IsLowSpeedReOverClock), value);
                    OnPropertyChanged(nameof(IsLowSpeedReOverClock));
                }
            }
        }

        public int LowSpeedReOverClockMinutes {
            get => _inner.LowSpeedReOverClockMinutes;
            set {
                if (_inner.LowSpeedReOverClockMinutes != value) {
                    NTMinerContext.Instance.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(LowSpeedReOverClockMinutes), value);
                    OnPropertyChanged(nameof(LowSpeedReOverClockMinutes));
                }
            }
        }

        public double OverClockLowSpeed {
            get => _inner.OverClockLowSpeed;
            set {
                if (_inner.OverClockLowSpeed != value) {
                    NTMinerContext.Instance.MinerProfile.SetCoinProfileProperty(this.CoinId, nameof(OverClockLowSpeed), value);
                    OnPropertyChanged(nameof(OverClockLowSpeed));
                }
            }
        }

        public PoolViewModel DualCoinPool {
            get {
                if (!AppRoot.CoinVms.TryGetCoinVm(CoinId, out CoinViewModel coinVm)) {
                    return null;
                }
                if (!AppRoot.PoolVms.TryGetPoolVm(this.DualCoinPoolId, out PoolViewModel pool)) {
                    pool = coinVm.Pools.OrderBy(a => a.SortNumber).FirstOrDefault();
                    if (pool != null) {
                        DualCoinPoolId = pool.Id;
                    }
                }
                return pool;
            }
            set {
                if (value == null) {
                    if (AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                        value = coinVm.Pools.OrderBy(a => a.SortNumber).FirstOrDefault();
                    }
                }
                if (value != null) {
                    DualCoinPoolId = value.Id;
                    OnPropertyChanged(nameof(DualCoinPool));
                    // 不必判断该对象是否是主界面上当前展示的对象，因为若不是主界面上当前显式的对象的话没有机会变更
                    NTMinerContext.RefreshArgsAssembly.Invoke("币种Profile上引用的双挖币种的矿池发生了切换");
                }
            }
        }
    }
}
