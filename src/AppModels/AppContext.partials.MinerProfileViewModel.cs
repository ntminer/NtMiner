using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Profile;
using NTMiner.Vms;
using System;
using System.Linq;

namespace NTMiner {
    public partial class AppContext {
        public class MinerProfileViewModel : ViewModelBase, IMinerProfile {
            public static readonly MinerProfileViewModel Instance = new MinerProfileViewModel();

            private MinerProfileViewModel() {
#if DEBUG
                VirtualRoot.Stopwatch.Restart();
#endif
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
                    this.ArgsAssembly = NTMinerRoot.Instance.BuildAssembleArgs(out _, out _, out _);
                };
                VirtualRoot.On<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChanged(nameof(CoinVm));
                    });
                Window<RefreshAutoBootStartCommand>("刷新开机启动和自动挖矿的展示", LogEnum.UserConsole,
                    action: message => {
                        OnPropertyChanged(nameof(IsAutoBoot));
                        OnPropertyChanged(nameof(IsAutoStart));
                    });
                On<MinerProfilePropertyChangedEvent>("MinerProfile设置变更后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChanged(message.PropertyName);
                    });
                On<MineWorkPropertyChangedEvent>("MineWork设置变更后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        OnPropertyChanged(message.PropertyName);
                    });

                VirtualRoot.On<LocalContextVmsReInitedEvent>("本地上下文视图模型集刷新后刷新界面", LogEnum.DevConsole,
                    action: message => {
                        AllPropertyChanged();
                    });
#if DEBUG
                Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
            }

            // 是否主矿池和备矿池都是用户名密码模式的矿池
            public bool IsAllMainCoinPoolIsUserMode {
                get {
                    if (CoinVm == null) {
                        return false;
                    }
                    var mainCoinPool = CoinVm.CoinProfile.MainCoinPool;
                    if (mainCoinPool == null) {
                        return false;
                    }
                    if (CoinVm.CoinKernel.IsSupportPool1) {
                        var mainCoinPool1 = CoinVm.CoinProfile.MainCoinPool1;
                        if (mainCoinPool1 == null) {
                            return mainCoinPool != null && mainCoinPool.IsUserMode;
                        }
                        return mainCoinPool != null && mainCoinPool.IsUserMode && mainCoinPool1 != null && mainCoinPool1.IsUserMode;
                    }
                    return mainCoinPool != null && mainCoinPool.IsUserMode;
                }
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
                get => Registry.GetIsShowInTaskbar();
                set {
                    if (Registry.GetIsShowInTaskbar() != value) {
                        Registry.SetIsShowInTaskbar(value);
                        OnPropertyChanged(nameof(IsShowInTaskbar));
                    }
                }
            }

            public bool IsNoUi {
                get { return Registry.GetIsNoUi(); }
                set {
                    if (Registry.GetIsNoUi() != value) {
                        Registry.SetIsNoUi(value);
                        OnPropertyChanged(nameof(IsNoUi));
                    }
                }
            }

            public bool IsAutoNoUi {
                get { return Registry.GetIsAutoNoUi(); }
                set {
                    if (Registry.GetIsAutoNoUi() != value) {
                        Registry.SetIsAutoNoUi(value);
                        OnPropertyChanged(nameof(IsAutoNoUi));
                    }
                }
            }

            public int AutoNoUiMinutes {
                get { return Registry.GetAutoNoUiMinutes(); }
                set {
                    if (Registry.GetAutoNoUiMinutes() != value) {
                        Registry.SetAutoNoUiMinutes(value);
                        OnPropertyChanged(nameof(AutoNoUiMinutes));
                    }
                }
            }

            public bool IsShowNotifyIcon {
                get => Registry.GetIsShowNotifyIcon();
                set {
                    if (Registry.GetIsShowNotifyIcon() != value) {
                        Registry.SetIsShowNotifyIcon(value);
                        OnPropertyChanged(nameof(IsShowNotifyIcon));
                        NotifyIcon?.RefreshIcon();
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
            private bool _isMining = NTMinerRoot.Instance.IsMining;

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
                get => Registry.GetIsAutoBoot();
                set {
                    Registry.SetIsAutoBoot(value);
                    OnPropertyChanged(nameof(IsAutoBoot));
                }
            }

            public bool IsAutoStart {
                get => Registry.GetIsAutoStart();
                set {
                    Registry.SetIsAutoStart(value);
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

            public int AutoRestartKernelTimes {
                get => NTMinerRoot.Instance.MinerProfile.AutoRestartKernelTimes;
                set {
                    if (value < 3) {
                        value = 3;
                    }
                    if (NTMinerRoot.Instance.MinerProfile.AutoRestartKernelTimes != value) {
                        NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(AutoRestartKernelTimes), value);
                        OnPropertyChanged(nameof(AutoRestartKernelTimes));
                    }
                }
            }

            public bool IsSpeedDownRestartComputer {
                get => NTMinerRoot.Instance.MinerProfile.IsSpeedDownRestartComputer;
                set {
                    if (NTMinerRoot.Instance.MinerProfile.IsSpeedDownRestartComputer != value) {
                        NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsSpeedDownRestartComputer), value);
                        OnPropertyChanged(nameof(IsSpeedDownRestartComputer));
                    }
                }
            }

            public int RestartComputerSpeedDownPercent {
                get => NTMinerRoot.Instance.MinerProfile.RestartComputerSpeedDownPercent;
                set {
                    if (NTMinerRoot.Instance.MinerProfile.RestartComputerSpeedDownPercent != value) {
                        NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(RestartComputerSpeedDownPercent), value);
                        OnPropertyChanged(nameof(RestartComputerSpeedDownPercent));
                    }
                }
            }

            public bool IsTempHighStopMine {
                get => NTMinerRoot.Instance.MinerProfile.IsTempHighStopMine;
                set {
                    if (NTMinerRoot.Instance.MinerProfile.IsTempHighStopMine != value) {
                        NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsTempHighStopMine), value);
                        OnPropertyChanged(nameof(IsTempHighStopMine));
                    }
                }
            }

            public int StopMineTempHigh {
                get => NTMinerRoot.Instance.MinerProfile.StopMineTempHigh;
                set {
                    if (NTMinerRoot.Instance.MinerProfile.StopMineTempHigh != value) {
                        NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(StopMineTempHigh), value);
                        OnPropertyChanged(nameof(StopMineTempHigh));
                    }
                }
            }

            public int StartMineTempLow {
                get => NTMinerRoot.Instance.MinerProfile.StartMineTempLow;
                set {
                    if (NTMinerRoot.Instance.MinerProfile.StartMineTempLow != value) {
                        NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(StartMineTempLow), value);
                        OnPropertyChanged(nameof(StartMineTempLow));
                    }
                }
            }

            public bool IsEChargeEnabled {
                get => NTMinerRoot.Instance.MinerProfile.IsEChargeEnabled;
                set {
                    if (NTMinerRoot.Instance.MinerProfile.IsEChargeEnabled != value) {
                        NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsEChargeEnabled), value);
                        OnPropertyChanged(nameof(IsEChargeEnabled));
                    }
                }
            }

            public double EPrice {
                get => NTMinerRoot.Instance.MinerProfile.EPrice;
                set {
                    if (NTMinerRoot.Instance.MinerProfile.EPrice != value) {
                        NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(EPrice), value);
                        OnPropertyChanged(nameof(EPrice));
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
                    if (!AppContext.Instance.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm) || !coinVm.IsSupported) {
                        coinVm = AppContext.Instance.CoinVms.MainCoins.Where(a => a.IsSupported).FirstOrDefault();
                        if (coinVm != null) {
                            CoinId = coinVm.Id;
                        }
                    }
                    return coinVm;
                }
                set {
                    if (value == null) {
                        value = AppContext.Instance.CoinVms.MainCoins.Where(a => a.IsSupported).OrderBy(a => a.SortNumber).FirstOrDefault();
                    }
                    if (value != null) {
                        this.CoinId = value.Id;
                        OnPropertyChanged(nameof(CoinVm));
                        NTMinerRoot.RefreshArgsAssembly.Invoke();
                        AppContext.Instance.MinerProfileVm.OnPropertyChanged(nameof(AppContext.Instance.MinerProfileVm.IsAllMainCoinPoolIsUserMode));
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
}
