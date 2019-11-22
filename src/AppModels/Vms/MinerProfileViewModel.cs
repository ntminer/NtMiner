using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerProfileViewModel : ViewModelBase, IMinerProfile {
        public static readonly MinerProfileViewModel Instance = new MinerProfileViewModel();

        private readonly string _linkFileFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "开源矿工.lnk");
        private readonly string _linkFileDescription = "开源矿工 - 做最好的矿工 https://ntminer.com";

        public ICommand AutoStartDelaySecondsUp { get; private set; }
        public ICommand AutoStartDelaySecondsDown { get; private set; }

        public ICommand AutoRestartKernelTimesUp { get; private set; }
        public ICommand AutoRestartKernelTimesDown { get; private set; }

        public ICommand NoShareRestartKernelMinutesUp { get; private set; }
        public ICommand NoShareRestartKernelMinutesDown { get; private set; }
        public ICommand NoShareRestartComputerMinutesUp { get; private set; }
        public ICommand NoShareRestartComputerMinutesDown { get; private set; }

        public ICommand PeriodicRestartKernelHoursUp { get; private set; }
        public ICommand PeriodicRestartKernelHoursDown { get; private set; }

        public ICommand PeriodicRestartKernelMinutesUp { get; private set; }
        public ICommand PeriodicRestartKernelMinutesDown { get; private set; }

        public ICommand PeriodicRestartComputerHoursUp { get; private set; }
        public ICommand PeriodicRestartComputerHoursDown { get; private set; }

        public ICommand PeriodicRestartComputerMinutesUp { get; private set; }
        public ICommand PeriodicRestartComputerMinutesDown { get; private set; }

        public ICommand CpuGETemperatureSecondsUp { get; private set; }
        public ICommand CpuGETemperatureSecondsDown { get; private set; }

        public ICommand CpuStopTemperatureUp { get; private set; }
        public ICommand CpuStopTemperatureDown { get; private set; }

        public ICommand CpuLETemperatureSecondsUp { get; private set; }
        public ICommand CpuLETemperatureSecondsDown { get; private set; }

        public ICommand CpuStartTemperatureUp { get; private set; }
        public ICommand CpuStartTemperatureDown { get; private set; }

        public ICommand EPriceUp { get; private set; }
        public ICommand EPriceDown { get; private set; }

        public ICommand PowerAppendUp { get; private set; }
        public ICommand PowerAppendDown { get; private set; }

        public ICommand MaxTempUp { get; private set; }
        public ICommand MaxTempDown { get; private set; }

        public ICommand AutoNoUiMinutesUp { get; private set; }
        public ICommand AutoNoUiMinutesDown { get; private set; }

        public ICommand HighCpuBaselineUp { get; private set; }
        public ICommand HighCpuBaselineDown { get; private set; }

        public ICommand HighCpuSecondsUp { get; private set; }
        public ICommand HighCpuSecondsDown { get; private set; }

        public MinerProfileViewModel() {
#if DEBUG
            Write.Stopwatch.Start();
#endif
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            if (Instance != null) {
                throw new InvalidProgramException();
            }
            if (this.IsCreateShortcut) {
                CreateShortcut();
            }
            this.AutoStartDelaySecondsUp = new DelegateCommand(() => {
                this.AutoStartDelaySeconds++;
            });
            this.AutoStartDelaySecondsDown = new DelegateCommand(() => {
                if (this.AutoStartDelaySeconds > 0) {
                    this.AutoStartDelaySeconds--;
                }
            });
            this.AutoRestartKernelTimesUp = new DelegateCommand(() => {
                this.AutoRestartKernelTimes++;
            });
            this.AutoRestartKernelTimesDown = new DelegateCommand(() => {
                if (this.AutoRestartKernelTimes > 0) {
                    this.AutoRestartKernelTimes--;
                }
            });
            this.NoShareRestartKernelMinutesUp = new DelegateCommand(() => {
                this.NoShareRestartKernelMinutes++;
            });
            this.NoShareRestartKernelMinutesDown = new DelegateCommand(() => {
                if (this.NoShareRestartKernelMinutes > 0) {
                    this.NoShareRestartKernelMinutes--;
                }
            });
            this.NoShareRestartComputerMinutesUp = new DelegateCommand(() => {
                this.NoShareRestartComputerMinutes++;
            });
            this.NoShareRestartComputerMinutesDown = new DelegateCommand(() => {
                if (this.NoShareRestartComputerMinutes > 0) {
                    this.NoShareRestartComputerMinutes--;
                }
            });
            this.PeriodicRestartKernelHoursUp = new DelegateCommand(() => {
                this.PeriodicRestartKernelHours++;
            });
            this.PeriodicRestartKernelHoursDown = new DelegateCommand(() => {
                if (this.PeriodicRestartKernelHours > 0) {
                    this.PeriodicRestartKernelHours--;
                }
            });
            this.PeriodicRestartKernelMinutesUp = new DelegateCommand(() => {
                this.PeriodicRestartKernelMinutes++;
            });
            this.PeriodicRestartKernelMinutesDown = new DelegateCommand(() => {
                if (this.PeriodicRestartKernelMinutes > 0) {
                    this.PeriodicRestartKernelMinutes--;
                }
            });
            this.PeriodicRestartComputerHoursUp = new DelegateCommand(() => {
                this.PeriodicRestartComputerHours++;
            });
            this.PeriodicRestartComputerHoursDown = new DelegateCommand(() => {
                if (this.PeriodicRestartComputerHours > 0) {
                    this.PeriodicRestartComputerHours--;
                }
            });
            this.PeriodicRestartComputerMinutesUp = new DelegateCommand(() => {
                this.PeriodicRestartComputerMinutes++;
            });
            this.PeriodicRestartComputerMinutesDown = new DelegateCommand(() => {
                if (this.PeriodicRestartComputerMinutes > 0) {
                    this.PeriodicRestartComputerMinutes--;
                }
            });
            this.CpuGETemperatureSecondsUp = new DelegateCommand(() => {
                this.CpuGETemperatureSeconds++;
            });
            this.CpuGETemperatureSecondsDown = new DelegateCommand(() => {
                this.CpuGETemperatureSeconds--;
            });
            this.CpuStopTemperatureUp = new DelegateCommand(() => {
                this.CpuStopTemperature++;
            });
            this.CpuStopTemperatureDown = new DelegateCommand(() => {
                this.CpuStopTemperature--;
            });
            this.CpuLETemperatureSecondsUp = new DelegateCommand(() => {
                this.CpuLETemperatureSeconds++;
            });
            this.CpuLETemperatureSecondsDown = new DelegateCommand(() => {
                this.CpuLETemperatureSeconds--;
            });
            this.CpuStartTemperatureUp = new DelegateCommand(() => {
                this.CpuStartTemperature++;
            });
            this.CpuStartTemperatureDown = new DelegateCommand(() => {
                this.CpuStartTemperature--;
            });
            this.EPriceUp = new DelegateCommand(() => {
                this.EPrice = Math.Round(this.EPrice + 0.1, 2);
            });
            this.EPriceDown = new DelegateCommand(() => {
                if (this.EPrice > 0.1) {
                    this.EPrice = Math.Round(this.EPrice - 0.1, 2);
                }
            });
            this.PowerAppendUp = new DelegateCommand(() => {
                this.PowerAppend++;
            });
            this.PowerAppendDown = new DelegateCommand(() => {
                if (this.PowerAppend > 0) {
                    this.PowerAppend--;
                }
            });
            this.MaxTempUp = new DelegateCommand(() => {
                this.MaxTemp++;
            });
            this.MaxTempDown = new DelegateCommand(() => {
                if (this.MaxTemp > 0) {
                    this.MaxTemp--;
                }
            });
            this.AutoNoUiMinutesUp = new DelegateCommand(() => {
                this.AutoNoUiMinutes++;
            });
            this.AutoNoUiMinutesDown = new DelegateCommand(() => {
                if (this.AutoNoUiMinutes > 0) {
                    this.AutoNoUiMinutes--;
                }
            });
            this.HighCpuBaselineUp = new DelegateCommand(() => {
                this.HighCpuBaseline++;
            });
            this.HighCpuBaselineDown = new DelegateCommand(() => {
                if (this.HighCpuBaseline > 0) {
                    this.HighCpuBaseline--;
                }
            });
            this.HighCpuSecondsUp = new DelegateCommand(() => {
                this.HighCpuSeconds++;
            });
            this.HighCpuSecondsDown = new DelegateCommand(() => {
                if (this.HighCpuSeconds > 0) {
                    this.HighCpuSeconds--;
                }
            });
            NTMinerRoot.SetRefreshArgsAssembly(() => {
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
                NTMinerRoot.Instance.CurrentMineContext = NTMinerRoot.Instance.CreateMineContext();
                if (NTMinerRoot.Instance.CurrentMineContext != null) {
                    this.ArgsAssembly = NTMinerRoot.Instance.CurrentMineContext.CommandLine;
                }
                else {
                    this.ArgsAssembly = string.Empty;
                }
            });
            VirtualRoot.BuildEventPath<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                action: message => {
                    OnPropertyChanged(nameof(CoinVm));
                });
            AppContext.BuildCmdPath<RefreshAutoBootStartCommand>("刷新开机启动和自动挖矿的展示", LogEnum.DevConsole,
                action: message => {
                    MinerProfileData data = NTMinerRoot.CreateLocalRepository<MinerProfileData>().GetByKey(this.Id);
                    if (data != null) {
                        this.IsAutoBoot = data.IsAutoBoot;
                        this.IsAutoStart = data.IsAutoStart;
                    }
                });
            AppContext.BuildEventPath<MinerProfilePropertyChangedEvent>("MinerProfile设置变更后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    OnPropertyChanged(message.PropertyName);
                });

            VirtualRoot.BuildEventPath<LocalContextVmsReInitedEvent>("本地上下文视图模型集刷新后刷新界面", LogEnum.DevConsole,
                action: message => {
                    AllPropertyChanged();
                    if (CoinVm != null) {
                        CoinVm.OnPropertyChanged(nameof(CoinVm.Wallets));
                        CoinVm.CoinKernel?.CoinKernelProfile.SelectedDualCoin?.OnPropertyChanged(nameof(CoinVm.Wallets));
                        CoinVm.CoinProfile.OnPropertyChanged(nameof(CoinVm.CoinProfile.SelectedWallet));
                        CoinVm.CoinKernel?.CoinKernelProfile.SelectedDualCoin?.CoinProfile.OnPropertyChanged(nameof(CoinVm.CoinProfile.SelectedDualCoinWallet));
                    }
                });
#if DEBUG
            var elapsedMilliseconds = Write.Stopwatch.Stop();
            if (elapsedMilliseconds.ElapsedMilliseconds > 20) {
                Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
            }
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
                if (mainCoinPool.NoPool1) {
                    return true;
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
            get {
                string minerName = NTMinerRoot.Instance.MinerProfile.MinerName;
                // 群控模式时可能未指定群控矿工名，此时使用本地模式交换到注册表的本地矿工名
                if (string.IsNullOrEmpty(minerName)) {
                    return NTMinerRegistry.GetMinerName();
                }
                return minerName;
            }
            set {
                if (NTMinerRoot.Instance.MinerProfile.MinerName != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(MinerName), value);
                    NTMinerRegistry.SetMinerName(value);
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                    OnPropertyChanged(nameof(MinerName));
                }
            }
        }

        public bool IsShowInTaskbar {
            get => NTMinerRoot.Instance.MinerProfile.IsShowInTaskbar;
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsShowInTaskbar != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsShowInTaskbar), value);
                    OnPropertyChanged(nameof(IsShowInTaskbar));
                }
            }
        }

        public bool IsNoUi {
            get { return NTMinerRoot.Instance.MinerProfile.IsNoUi; }
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsNoUi != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsNoUi), value);
                    OnPropertyChanged(nameof(IsNoUi));
                }
            }
        }

        public bool IsAutoNoUi {
            get { return NTMinerRoot.Instance.MinerProfile.IsAutoNoUi; }
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsAutoNoUi != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoNoUi), value);
                    OnPropertyChanged(nameof(IsAutoNoUi));
                }
            }
        }

        public int AutoNoUiMinutes {
            get { return NTMinerRoot.Instance.MinerProfile.AutoNoUiMinutes; }
            set {
                if (NTMinerRoot.Instance.MinerProfile.AutoNoUiMinutes != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(AutoNoUiMinutes), value);
                    OnPropertyChanged(nameof(AutoNoUiMinutes));
                }
            }
        }

        public bool IsShowNotifyIcon {
            get => NTMinerRoot.Instance.MinerProfile.IsShowNotifyIcon;
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsShowNotifyIcon != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsShowNotifyIcon), value);
                    OnPropertyChanged(nameof(IsShowNotifyIcon));
                    AppContext.NotifyIcon?.RefreshIcon();
                }
            }
        }

        public bool IsCloseMeanExit {
            get => NTMinerRoot.Instance.MinerProfile.IsCloseMeanExit;
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsCloseMeanExit != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsCloseMeanExit), value);
                    OnPropertyChanged(nameof(IsCloseMeanExit));
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
            }
        }

        public bool IsAutoBoot {
            get => NTMinerRoot.Instance.MinerProfile.IsAutoBoot;
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsAutoBoot != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoBoot), value);
                    OnPropertyChanged(nameof(IsAutoBoot));
                }
            }
        }

        public bool IsAutoStart {
            get => NTMinerRoot.Instance.MinerProfile.IsAutoStart;
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsAutoStart != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoStart), value);
                    OnPropertyChanged(nameof(IsAutoStart));
                }
            }
        }

        public bool IsAutoDisableWindowsFirewall {
            get => NTMinerRoot.Instance.MinerProfile.IsAutoDisableWindowsFirewall;
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsAutoDisableWindowsFirewall != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoDisableWindowsFirewall), value);
                    OnPropertyChanged(nameof(IsAutoDisableWindowsFirewall));
                }
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

        public bool IsNoShareRestartComputer {
            get => NTMinerRoot.Instance.MinerProfile.IsNoShareRestartComputer;
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsNoShareRestartComputer != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsNoShareRestartComputer), value);
                    OnPropertyChanged(nameof(IsNoShareRestartComputer));
                }
            }
        }

        public int NoShareRestartComputerMinutes {
            get => NTMinerRoot.Instance.MinerProfile.NoShareRestartComputerMinutes;
            set {
                if (NTMinerRoot.Instance.MinerProfile.NoShareRestartComputerMinutes != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(NoShareRestartComputerMinutes), value);
                    OnPropertyChanged(nameof(NoShareRestartComputerMinutes));
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

        public int PeriodicRestartKernelMinutes {
            get => NTMinerRoot.Instance.MinerProfile.PeriodicRestartKernelMinutes;
            set {
                if (NTMinerRoot.Instance.MinerProfile.PeriodicRestartKernelMinutes != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(PeriodicRestartKernelMinutes), value);
                    OnPropertyChanged(nameof(PeriodicRestartKernelMinutes));
                    if (value < 0 || value > 60) {
                        throw new ValidationException("无效的值");
                    }
                }
            }
        }

        public int PeriodicRestartComputerMinutes {
            get => NTMinerRoot.Instance.MinerProfile.PeriodicRestartComputerMinutes;
            set {
                if (NTMinerRoot.Instance.MinerProfile.PeriodicRestartComputerMinutes != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(PeriodicRestartComputerMinutes), value);
                    OnPropertyChanged(nameof(PeriodicRestartComputerMinutes));
                    if (value < 0 || value > 60) {
                        throw new ValidationException("无效的值");
                    }
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

        public bool IsPowerAppend {
            get => NTMinerRoot.Instance.MinerProfile.IsPowerAppend;
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsPowerAppend != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsPowerAppend), value);
                    OnPropertyChanged(nameof(IsPowerAppend));
                }
            }
        }

        public int PowerAppend {
            get => NTMinerRoot.Instance.MinerProfile.PowerAppend;
            set {
                if (NTMinerRoot.Instance.MinerProfile.PowerAppend != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(PowerAppend), value);
                    OnPropertyChanged(nameof(PowerAppend));
                }
            }
        }

        public bool IsShowCommandLine {
            get { return NTMinerRoot.Instance.MinerProfile.IsShowCommandLine; }
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsShowCommandLine != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsShowCommandLine), value);
                    OnPropertyChanged(nameof(IsShowCommandLine));
                }
            }
        }

        private void CreateShortcut() {
            if (!VirtualRoot.IsMinerClient) {
                return;
            }
            bool isDo = !File.Exists(_linkFileFullName);
            if (!isDo) {
                string targetPath = WindowsShortcut.GetTargetPath(_linkFileFullName);
                isDo = !VirtualRoot.AppFileFullName.Equals(targetPath, StringComparison.OrdinalIgnoreCase);
            }
            if (isDo) {
                WindowsShortcut.CreateShortcut(_linkFileFullName, VirtualRoot.AppFileFullName, _linkFileDescription);
            }
        }

        public bool IsCreateShortcut {
            get { return NTMinerRoot.Instance.MinerProfile.IsCreateShortcut; }
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsCreateShortcut != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsCreateShortcut), value);
                    OnPropertyChanged(nameof(IsCreateShortcut));
                    if (VirtualRoot.IsMinerClient) {
                        if (value) {
                            CreateShortcut();
                        }
                        else {
                            File.Delete(_linkFileFullName);
                        }
                    }
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

        public int MaxTemp {
            get => NTMinerRoot.Instance.MinerProfile.MaxTemp;
            set {
                if (NTMinerRoot.Instance.MinerProfile.MaxTemp != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(MaxTemp), value);
                    OnPropertyChanged(nameof(MaxTemp));
                }
            }
        }

        public int AutoStartDelaySeconds {
            get => NTMinerRoot.Instance.MinerProfile.AutoStartDelaySeconds;
            set {
                if (NTMinerRoot.Instance.MinerProfile.AutoStartDelaySeconds != value) {
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(AutoStartDelaySeconds), value);
                    OnPropertyChanged(nameof(AutoStartDelaySeconds));
                }
            }
        }

        public bool IsAutoStopByCpu {
            get => NTMinerRoot.Instance.MinerProfile.IsAutoStopByCpu;
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsAutoStopByCpu != value) {
                    NTMinerRoot.Instance.CpuPackage.Reset();
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoStopByCpu), value);
                    OnPropertyChanged(nameof(IsAutoStopByCpu));
                }
            }
        }

        public int CpuStopTemperature {
            get => NTMinerRoot.Instance.MinerProfile.CpuStopTemperature;
            set {
                if (NTMinerRoot.Instance.MinerProfile.CpuStopTemperature != value) {
                    NTMinerRoot.Instance.CpuPackage.Reset();
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(CpuStopTemperature), value);
                    OnPropertyChanged(nameof(CpuStopTemperature));
                }
            }
        }

        public int CpuGETemperatureSeconds {
            get => NTMinerRoot.Instance.MinerProfile.CpuGETemperatureSeconds;
            set {
                if (NTMinerRoot.Instance.MinerProfile.CpuGETemperatureSeconds != value) {
                    NTMinerRoot.Instance.CpuPackage.Reset();
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(CpuGETemperatureSeconds), value);
                    OnPropertyChanged(nameof(CpuGETemperatureSeconds));
                }
            }
        }

        public bool IsAutoStartByCpu {
            get => NTMinerRoot.Instance.MinerProfile.IsAutoStartByCpu;
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsAutoStartByCpu != value) {
                    NTMinerRoot.Instance.CpuPackage.Reset();
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoStartByCpu), value);
                    OnPropertyChanged(nameof(IsAutoStartByCpu));
                }
            }
        }

        public int CpuStartTemperature {
            get => NTMinerRoot.Instance.MinerProfile.CpuStartTemperature;
            set {
                if (NTMinerRoot.Instance.MinerProfile.CpuStartTemperature != value) {
                    NTMinerRoot.Instance.CpuPackage.Reset();
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(CpuStartTemperature), value);
                    OnPropertyChanged(nameof(CpuStartTemperature));
                }
            }
        }

        public int CpuLETemperatureSeconds {
            get => NTMinerRoot.Instance.MinerProfile.CpuLETemperatureSeconds;
            set {
                if (NTMinerRoot.Instance.MinerProfile.CpuLETemperatureSeconds != value) {
                    NTMinerRoot.Instance.CpuPackage.Reset();
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(CpuLETemperatureSeconds), value);
                    OnPropertyChanged(nameof(CpuLETemperatureSeconds));
                }
            }
        }

        public bool IsRaiseHighCpuEvent {
            get => NTMinerRoot.Instance.MinerProfile.IsRaiseHighCpuEvent;
            set {
                if (NTMinerRoot.Instance.MinerProfile.IsRaiseHighCpuEvent != value) {
                    NTMinerRoot.Instance.CpuPackage.Reset();
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsRaiseHighCpuEvent), value);
                    OnPropertyChanged(nameof(IsRaiseHighCpuEvent));
                }
            }
        }

        public int HighCpuBaseline {
            get => NTMinerRoot.Instance.MinerProfile.HighCpuBaseline;
            set {
                if (NTMinerRoot.Instance.MinerProfile.HighCpuBaseline != value) {
                    NTMinerRoot.Instance.CpuPackage.Reset();
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(HighCpuBaseline), value);
                    OnPropertyChanged(nameof(HighCpuBaseline));
                }
            }
        }

        public int HighCpuSeconds {
            get => NTMinerRoot.Instance.MinerProfile.HighCpuSeconds;
            set {
                if (NTMinerRoot.Instance.MinerProfile.HighCpuSeconds != value) {
                    NTMinerRoot.Instance.CpuPackage.Reset();
                    NTMinerRoot.Instance.MinerProfile.SetMinerProfileProperty(nameof(HighCpuSeconds), value);
                    OnPropertyChanged(nameof(HighCpuSeconds));
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
                    value = AppContext.Instance.CoinVms.MainCoins.Where(a => a.IsSupported).OrderBy(a => a.Code).FirstOrDefault();
                }
                if (value != null) {
                    this.CoinId = value.Id;
                    OnPropertyChanged(nameof(CoinVm));
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                    AppContext.Instance.MinerProfileVm.OnPropertyChanged(nameof(AppContext.Instance.MinerProfileVm.IsAllMainCoinPoolIsUserMode));
                    foreach (var item in AppContext.GpuSpeedViewModels.Instance.List) {
                        item.OnPropertyChanged(nameof(item.GpuProfileVm));
                    }
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

        public bool IsWorkerOrMining {
            get {
                return IsMining || IsWorker;
            }
        }
    }
}
