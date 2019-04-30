using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Linq;

namespace NTMiner.Vms {
    public class MinerProfileViewModel : ViewModelBase, IMinerProfile {
        public MinerProfileViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            NTMinerRoot.RefreshArgsAssembly = () => {
                if (CoinVm != null && CoinVm.CoinKernel != null && CoinVm.CoinKernel.Kernel != null) {
                    var coinKernelProfile = CoinVm.CoinKernel.CoinKernelProfile;
                    var kernelInput = CoinVm.CoinKernel.Kernel.KernelInputVm;
                    if (coinKernelProfile != null && kernelInput != null) {
                        if (coinKernelProfile.IsDualCoinEnabled && !kernelInput.IsAutoDualWeight) {
                            if (coinKernelProfile.DualCoinWeight > kernelInput.DualWeightMax) {
                                coinKernelProfile.DualCoinWeight = kernelInput.DualWeightMax;
                            }
                            else if (coinKernelProfile.DualCoinWeight < kernelInput.DualWeightMin) {
                                coinKernelProfile.DualCoinWeight = kernelInput.DualWeightMin;
                            }
                            NTMinerRoot.Instance.MinerProfile.SetCoinKernelProfileProperty(coinKernelProfile.CoinKernelId, nameof(coinKernelProfile.DualCoinWeight), coinKernelProfile.DualCoinWeight);
                        }
                    }
                }
                this.ArgsAssembly = NTMinerRoot.Instance.BuildAssembleArgs();
            };
            NTMinerRoot.Instance.OnReRendContext += () => {
                OnPropertyChanged(nameof(CoinVm));
            };
            VirtualRoot.Window<RefreshAutoBootStartCommand>("刷新开机启动和自动挖矿的展示", LogEnum.UserConsole,
                action: message => {
                    OnPropertyChanged(nameof(IsAutoBoot));
                    OnPropertyChanged(nameof(IsAutoStart));
                }).AddToCollection(AppContext.ContextHandlers);
            VirtualRoot.On<MinerProfilePropertyChangedEvent>("MinerProfile设置变更后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    OnPropertyChanged(message.PropertyName);
                }).AddToCollection(AppContext.ContextHandlers);
            VirtualRoot.On<MineWorkPropertyChangedEvent>("MineWork设置变更后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    OnPropertyChanged(message.PropertyName);
                }).AddToCollection(AppContext.ContextHandlers);

            NTMinerRoot.Instance.OnReRendMinerProfile += () => {
                this.CoinVm.CoinKernel?.OnPropertyChanged(nameof(CoinKernelViewModel.CoinKernelProfile));
                AllPropertyChanged();
            };
            NTMinerRoot.RefreshArgsAssembly.Invoke();
        }

        public IMineWork MineWork {
            get {
                return NTMinerRoot.Instance.MinerProfile.MineWork;
            }
        }

        public bool IsFreeClient {
            get {
                return MineWork == null || VirtualRoot.IsMinerStudio;
            }
        }

        public Guid Id {
            get { return NTMinerRoot.Instance.MinerProfile.GetId(); }
        }

        public Guid GetId() {
            return this.Id;
        }

        public string MinerName {
            get => NTMinerRoot.Instance.MinerProfile.MinerName;
            set {
                if (NTMinerRoot.Instance.MinerProfile.MinerName != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(MinerName), value);
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                    OnPropertyChanged(nameof(MinerName));
                }
            }
        }

        public bool IsShowInTaskbar {
            get => NTMinerRegistry.GetIsShowInTaskbar();
            set {
                if (NTMinerRegistry.GetIsShowInTaskbar() != value) {
                    NTMinerRegistry.SetIsShowInTaskbar(value);
                    OnPropertyChanged(nameof(IsShowInTaskbar));
                }
            }
        }

        public bool IsNoUi {
            get { return NTMinerRegistry.GetIsNoUi(); }
            set {
                if (NTMinerRegistry.GetIsNoUi() != value) {
                    NTMinerRegistry.SetIsNoUi(value);
                    OnPropertyChanged(nameof(IsNoUi));
                }
            }
        }

        public bool IsShowNotifyIcon {
            get => NTMinerRegistry.GetIsShowNotifyIcon();
            set {
                if (NTMinerRegistry.GetIsShowNotifyIcon() != value) {
                    NTMinerRegistry.SetIsShowNotifyIcon(value);
                    OnPropertyChanged(nameof(IsShowNotifyIcon));
                    AppHelper.NotifyIcon.RefreshIcon();
                }
            }
        }

        public string HotKey {
            get { return HotKeyUtil.GetHotKey(); }
            set {
                if (HotKeyUtil.GetHotKey() != value) {
                    if (HotKeyUtil.SetHotKey(value)) {
                        OnPropertyChanged(nameof(HotKey));
                    }
                }
            }
        }

        private string _argsAssembly;
        private bool _isMining;

        public string ArgsAssembly {
            get {
                return _argsAssembly;
            }
            set {
                _argsAssembly = value;
                OnPropertyChanged(nameof(ArgsAssembly));
                NTMinerRoot.UserKernelCommandLine = value;
            }
        }

        public bool IsAutoBoot {
            get => NTMinerRegistry.GetIsAutoBoot();
            set {
                NTMinerRegistry.SetIsAutoBoot(value);
                OnPropertyChanged(nameof(IsAutoBoot));
            }
        }

        public bool IsAutoStart {
            get => NTMinerRegistry.GetIsAutoStart();
            set {
                NTMinerRegistry.SetIsAutoStart(value);
                OnPropertyChanged(nameof(IsAutoStart));
            }
        }

        public bool IsNoShareRestartKernel {
            get => NTMinerRoot.Instance.MinerProfile.IsNoShareRestartKernel;
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsNoShareRestartKernel != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsNoShareRestartKernel), value);
                    OnPropertyChanged(nameof(IsNoShareRestartKernel));
                }
            }
        }

        public int NoShareRestartKernelMinutes {
            get => NTMinerRoot.Instance.MinerProfile.NoShareRestartKernelMinutes;
            set {
                if (NTMinerRoot.Instance.MinerProfile.NoShareRestartKernelMinutes != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(NoShareRestartKernelMinutes), value);
                    OnPropertyChanged(nameof(NoShareRestartKernelMinutes));
                }
            }
        }

        public bool IsPeriodicRestartKernel {
            get => NTMinerRoot.Instance.MinerProfile.IsPeriodicRestartKernel;
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsPeriodicRestartKernel != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsPeriodicRestartKernel), value);
                    OnPropertyChanged(nameof(IsPeriodicRestartKernel));
                }
            }
        }

        public int PeriodicRestartKernelHours {
            get => NTMinerRoot.Instance.MinerProfile.PeriodicRestartKernelHours;
            set {
                if (NTMinerRoot.Instance.MinerProfile.PeriodicRestartKernelHours != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(PeriodicRestartKernelHours), value);
                    OnPropertyChanged(nameof(PeriodicRestartKernelHours));
                }
            }
        }

        public bool IsPeriodicRestartComputer {
            get => NTMinerRoot.Instance.MinerProfile.IsPeriodicRestartComputer;
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsPeriodicRestartComputer != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsPeriodicRestartComputer), value);
                    OnPropertyChanged(nameof(IsPeriodicRestartComputer));
                }
            }
        }

        public int PeriodicRestartComputerHours {
            get => NTMinerRoot.Instance.MinerProfile.PeriodicRestartComputerHours;
            set {
                if (NTMinerRoot.Instance.MinerProfile.PeriodicRestartComputerHours != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(PeriodicRestartComputerHours), value);
                    OnPropertyChanged(nameof(PeriodicRestartComputerHours));
                }
            }
        }

        public bool IsAutoRestartKernel {
            get => NTMinerRoot.Instance.MinerProfile.IsAutoRestartKernel;
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsAutoRestartKernel != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoRestartKernel), value);
                    OnPropertyChanged(nameof(IsAutoRestartKernel));
                }
            }
        }

        public bool IsShowCommandLine {
            get { return NTMinerRoot.GetIsShowCommandLine(); }
            set {
                if (NTMinerRoot.GetIsShowCommandLine() != value) {
                    NTMinerRoot.SetIsShowCommandLine(value);
                    OnPropertyChanged(nameof(IsShowCommandLine));
                }
            }
        }

        public Guid CoinId {
            get => NTMinerRoot.Instance.MinerProfile.CoinId;
            set {
                if (NTMinerRoot.Instance.MinerProfile.CoinId != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(CoinId), value);
                    OnPropertyChanged(nameof(CoinId));
                }
            }
        }

        public CoinViewModel CoinVm {
            get {
                if (!AppContext.Current.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm) || !coinVm.IsSupported) {
                    coinVm = AppContext.Current.CoinVms.MainCoins.Where(a => a.IsSupported).FirstOrDefault();
                    if (coinVm != null) {
                        CoinId = coinVm.Id;
                    }
                }
                return coinVm;
            }
            set {
                if (value == null) {
                    value = AppContext.Current.CoinVms.MainCoins.Where(a => a.IsSupported).OrderBy(a => a.SortNumber).FirstOrDefault();
                }
                if (value != null) {
                    this.CoinId = value.Id;
                    OnPropertyChanged(nameof(CoinVm));
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                }
            }
        }

        public bool IsWorker {
            get {
                return MineWork != null && !VirtualRoot.IsMinerStudio;
            }
        }

        public bool IsMining {
            get => _isMining;
            set {
                _isMining = value;
                OnPropertyChanged(nameof(IsMining));
            }
        }
    }
}
