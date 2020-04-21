using NTMiner.Core;
using NTMiner.Core.Profile;
using NTMiner.MinerStudio.Vms;
using NTMiner.Ws;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerProfileViewModel : ViewModelBase, IMinerProfile, IWsStateViewModel {
        public static readonly MinerProfileViewModel Instance = new MinerProfileViewModel();

        private readonly string _linkFileFullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "开源矿工.lnk");

        private string _argsAssembly;
        private bool _isMining = NTMinerContext.Instance.IsMining;
        private bool _isWsOnline;
        private string _wsDescription;
        private int _wsNextTrySecondsDelay;
        private DateTime _wsLastTryOn;
        private bool _isConnecting;
        private double _wsRetryIconAngle;

        public ICommand Up { get; private set; }
        public ICommand Down { get; private set; }
        public ICommand WsRetry { get; private set; }

        private readonly Dictionary<string, PropertyInfo> _propertyInfos = new Dictionary<string, PropertyInfo>();
        public MinerProfileViewModel() {
#if DEBUG
            NTStopwatch.Start();
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
            this.Up = new DelegateCommand<string>(propertyName => {
                if (!_propertyInfos.TryGetValue(propertyName, out PropertyInfo propertyInfo)) {
                    propertyInfo = this.GetType().GetProperty(propertyName);
                    if (propertyInfo == null) {
                        Write.DevError(() => $"类型{this.GetType().FullName}不具有名称为{propertyName}的属性");
                        return;
                    }
                    _propertyInfos.Add(propertyName, propertyInfo);
                }
                if (propertyInfo.PropertyType == typeof(int)) {
                    propertyInfo.SetValue(this, (int)propertyInfo.GetValue(this, null) + 1, null);
                }
                else if (propertyInfo.PropertyType == typeof(double)) {
                    propertyInfo.SetValue(this, Math.Round((double)propertyInfo.GetValue(this, null) + 0.1, 2), null);
                }
            });
            this.Down = new DelegateCommand<string>(propertyName => {
                if (!_propertyInfos.TryGetValue(propertyName, out PropertyInfo propertyInfo)) {
                    propertyInfo = this.GetType().GetProperty(propertyName);
                    if (propertyInfo == null) {
                        Write.DevError(() => $"类型{this.GetType().FullName}不具有名称为{propertyName}的属性");
                        return;
                    }
                    _propertyInfos.Add(propertyName, propertyInfo);
                }
                if (propertyInfo.PropertyType == typeof(int)) {
                    int value = (int)propertyInfo.GetValue(this, null);
                    if (value > 0) {
                        propertyInfo.SetValue(this, value - 1, null);
                    }
                }
                else if (propertyInfo.PropertyType == typeof(double)) {
                    double value = (double)propertyInfo.GetValue(this, null);
                    if (value > 0.1) {
                        propertyInfo.SetValue(this, Math.Round(value - 0.1, 2), null);
                    }
                }
            });
            this.WsRetry = new DelegateCommand(() => {
                RpcRoot.Client.NTMinerDaemonService.StartOrStopWsAsync(isResetFailCount: true);
                IsConnecting = true;
            });
            bool isRefreshed = false;
            if (ClientAppType.IsMinerClient) {
                VirtualRoot.AddEventPath<StartingMineFailedEvent>("开始挖矿失败", LogEnum.DevConsole,
                    action: message => {
                        IsMining = false;
                        Write.UserError(message.Message);
                    }, location: this.GetType());
                // 群控客户端已经有一个执行RefreshWsStateCommand命令的路径了
                VirtualRoot.AddCmdPath<RefreshWsStateCommand>(message => {
                    if (message.WsClientState != null) {
                        isRefreshed = true;
                        this.IsWsOnline = message.WsClientState.Status == WsClientStatus.Open;
                        if (message.WsClientState.ToOut) {
                            VirtualRoot.Out.ShowWarn(message.WsClientState.Description, autoHideSeconds: 3);
                        }
                        if (!message.WsClientState.ToOut || !this.IsWsOnline) {
                            this.WsDescription = message.WsClientState.Description;
                        }
                        if (!this.IsWsOnline) {
                            if (message.WsClientState.LastTryOn != DateTime.MinValue) {
                                this.WsLastTryOn = message.WsClientState.LastTryOn;
                            }
                            if (message.WsClientState.NextTrySecondsDelay > 0) {
                                WsNextTrySecondsDelay = message.WsClientState.NextTrySecondsDelay;
                            }
                        }
                    }
                }, this.GetType(), LogEnum.DevConsole);
                VirtualRoot.AddEventPath<Per1SecondEvent>("外网群控重试秒表倒计时", LogEnum.None, action: message => {
                    if (IsOuterUserEnabled && !IsWsOnline) {
                        if (WsNextTrySecondsDelay > 0) {
                            WsNextTrySecondsDelay--;
                        }
                        OnPropertyChanged(nameof(WsLastTryOnText));
                    }
                }, this.GetType());
                VirtualRoot.AddEventPath<WsServerOkEvent>("服务器Ws服务已可用", LogEnum.DevConsole, action: message => {
                    if (IsOuterUserEnabled && !IsWsOnline) {
                        StartOrStopWs();
                    }
                }, this.GetType());
                if (IsOuterUserEnabled) {
                    RpcRoot.Client.NTMinerDaemonService.GetWsDaemonStateAsync((WsClientState state, Exception e) => {
                        if (state != null && !isRefreshed) {
                            this.IsWsOnline = state.Status == WsClientStatus.Open;
                            this.WsDescription = state.Description;
                            if (state.NextTrySecondsDelay > 0) {
                                this.WsNextTrySecondsDelay = state.NextTrySecondsDelay;
                            }
                            this.WsLastTryOn = state.LastTryOn;
                        }
                    });
                }
            }
            NTMinerContext.SetRefreshArgsAssembly((reason) => {
                Write.DevDebug(() => $"RefreshArgsAssembly" + reason, ConsoleColor.Cyan);
#if DEBUG
                NTStopwatch.Start();
#endif
                #region 确保双挖权重在合法的范围内
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
                            NTMinerContext.Instance.MinerProfile.SetCoinKernelProfileProperty(coinKernelProfile.CoinKernelId, nameof(coinKernelProfile.DualCoinWeight), coinKernelProfile.DualCoinWeight);
                        }
                    }
                }
                #endregion
                NTMinerContext.Instance.CurrentMineContext = NTMinerContext.Instance.CreateMineContext();
                if (NTMinerContext.Instance.CurrentMineContext != null) {
                    this.ArgsAssembly = NTMinerContext.Instance.CurrentMineContext.CommandLine;
                }
                else {
                    this.ArgsAssembly = string.Empty;
                }
#if DEBUG
                var milliseconds = NTStopwatch.Stop();
                if (milliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{milliseconds} {this.GetType().Name}.SetRefreshArgsAssembly");
                }
#endif
            });
            VirtualRoot.AddEventPath<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                action: message => {
                    OnPropertyChanged(nameof(CoinVm));
                }, location: this.GetType());
            AppRoot.AddCmdPath<RefreshAutoBootStartCommand>("刷新开机启动和自动挖矿的展示", LogEnum.DevConsole,
                action: message => {
                    MinerProfileData data = NTMinerContext.Instance.ServerContext.CreateLocalRepository<MinerProfileData>().GetByKey(this.Id);
                    if (data != null) {
                        this.IsAutoBoot = data.IsAutoBoot;
                        this.IsAutoStart = data.IsAutoStart;
                    }
                }, location: this.GetType());
            AppRoot.AddEventPath<MinerProfilePropertyChangedEvent>("MinerProfile设置变更后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    OnPropertyChanged(message.PropertyName);
                }, location: this.GetType());

            VirtualRoot.AddEventPath<LocalContextVmsReInitedEvent>("本地上下文视图模型集刷新后刷新界面", LogEnum.DevConsole,
                action: message => {
                    AllPropertyChanged();
                    if (CoinVm != null) {
                        CoinVm.OnPropertyChanged(nameof(CoinVm.Wallets));
                        CoinVm.CoinKernel?.CoinKernelProfile.SelectedDualCoin?.OnPropertyChanged(nameof(CoinVm.Wallets));
                        CoinVm.CoinProfile?.OnPropertyChanged(nameof(CoinVm.CoinProfile.SelectedWallet));
                        CoinVm.CoinKernel?.CoinKernelProfile.SelectedDualCoin?.CoinProfile?.OnPropertyChanged(nameof(CoinVm.CoinProfile.SelectedDualCoinWallet));
                    }
                }, location: this.GetType());
            VirtualRoot.AddEventPath<CoinVmAddedEvent>("Vm集添加了新币种后刷新MinerProfileVm内存", LogEnum.DevConsole, action: message => {
                OnPropertyChanged(nameof(CoinVm));
            }, this.GetType());
            VirtualRoot.AddEventPath<CoinVmRemovedEvent>("Vm集删除了新币种后刷新MinerProfileVm内存", LogEnum.DevConsole, action: message => {
                OnPropertyChanged(nameof(CoinVm));
            }, this.GetType());
#if DEBUG
            var elapsedMilliseconds = NTStopwatch.Stop();
            if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
            }
#endif
        }

        #region IWsStateViewModel的成员

        // 由守护进程根据外网群控是否正常更新
        public bool IsWsOnline {
            get => _isWsOnline;
            set {
                if (_isWsOnline != value) {
                    _isWsOnline = value;
                    OnPropertyChanged(nameof(IsWsOnline));
                    OnPropertyChanged(nameof(WsStateText));
                    OnPropertyChanged(nameof(WsNextTrySecondsDelayVisible));
                }
            }
        }

        public string WsDescription {
            get {
                if (!IsOuterUserEnabled) {
                    return "未启用";
                }
                if (string.IsNullOrEmpty(OuterUserId)) {
                    return "未填写用户";
                }
                if (string.IsNullOrEmpty(_wsDescription)) {
                    return WsStateText;
                }
                return _wsDescription;
            }
            set {
                if (_wsDescription != value) {
                    _wsDescription = value;
                    OnPropertyChanged(nameof(WsDescription));
                }
            }
        }

        public int WsNextTrySecondsDelay {
            get {
                if (_wsNextTrySecondsDelay < 0) {
                    return 0;
                }
                return _wsNextTrySecondsDelay;
            }
            set {
                if (_wsNextTrySecondsDelay != value) {
                    _wsNextTrySecondsDelay = value;
                    OnPropertyChanged(nameof(WsNextTrySecondsDelay));
                    OnPropertyChanged(nameof(WsNextTrySecondsDelayText));
                    OnPropertyChanged(nameof(WsNextTrySecondsDelayVisible));
                    IsConnecting = value <= 0;
                }
            }
        }

        public DateTime WsLastTryOn {
            get => _wsLastTryOn;
            set {
                if (_wsLastTryOn != value) {
                    _wsLastTryOn = value;
                    OnPropertyChanged(nameof(WsLastTryOn));
                    OnPropertyChanged(nameof(WsLastTryOnText));
                }
            }
        }

        public bool IsConnecting {
            get => _isConnecting;
            set {
                if (_isConnecting != value) {
                    _isConnecting = value;
                    OnPropertyChanged(nameof(IsConnecting));
                    OnPropertyChanged(nameof(WsRetryText));
                    if (value) {
                        VirtualRoot.SetInterval(TimeSpan.FromMilliseconds(100), perCallback: () => {
                            WsRetryIconAngle += 40;
                        }, stopCallback: () => {
                            WsRetryIconAngle = 0;
                            IsConnecting = false;
                        }, timeout: TimeSpan.FromSeconds(10), requestStop: () => {
                            return !IsConnecting;
                        });
                    }
                }
            }
        }

        #endregion

        public double WsRetryIconAngle {
            get { return _wsRetryIconAngle; }
            set {
                _wsRetryIconAngle = value;
                OnPropertyChanged(nameof(WsRetryIconAngle));
            }
        }

        public string WsRetryText {
            get {
                if (IsConnecting) {
                    return "重试中";
                }
                return "立即重试";
            }
        }

        public string WsLastTryOnText {
            get {
                if (IsWsOnline || WsLastTryOn == DateTime.MinValue) {
                    return string.Empty;
                }
                return Timestamp.GetTimeSpanText(WsLastTryOn);
            }
        }

        public string WsNextTrySecondsDelayText {
            get {
                int seconds = WsNextTrySecondsDelay;
                if (!IsOuterUserEnabled || IsWsOnline) {
                    return string.Empty;
                }
                if (seconds >= 3600) {
                    return $"{(seconds / 3600).ToString()} 小时 {(seconds % 3600 / 60).ToString()} 分钟 {(seconds % 3600 % 60).ToString()} 秒后";
                }
                if (seconds > 60) {
                    return $"{(seconds / 60).ToString()} 分 {(seconds % 60).ToString()} 秒后";
                }
                return $"{seconds.ToString()} 秒后";
            }
        }

        public Visibility WsNextTrySecondsDelayVisible {
            get {
                if (!IsOuterUserEnabled || IsWsOnline) {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }

        public string WsStateText {
            get {
                if (IsWsOnline) {
                    return "连接服务器成功";
                }
                return "离线";
            }
        }

        public bool IsOuterUserEnabled {
            get => NTMinerContext.Instance.MinerProfile.IsOuterUserEnabled;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsOuterUserEnabled != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsOuterUserEnabled), value);
                    OnPropertyChanged(nameof(IsOuterUserEnabled));
                    OnPropertyChanged(nameof(WsNextTrySecondsDelayVisible));
                    StartOrStopWs();
                }
            }
        }

        public string OuterUserId {
            get => NTMinerContext.Instance.MinerProfile.OuterUserId;
            set {
                if (NTMinerContext.Instance.MinerProfile.OuterUserId != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(OuterUserId), value);
                    OnPropertyChanged(nameof(OuterUserId));
                    OnPropertyChanged(nameof(OuterUserText));
                    StartOrStopWs();
                }
            }
        }

        public string OuterUserText {
            get {
                if (string.IsNullOrEmpty(OuterUserId)) {
                    return "群控";
                }
                return OuterUserId;
            }
        }

        public void StartOrStopWs() {
            // 只要外网启用状态变更或用户变更就调用，不管是启用还是禁用也不管用户是否正确是否为空都要调用
            // 未启用时以及不正确的用户时调用是为了通知守护进程关闭连接
            RpcRoot.Client.NTMinerDaemonService.StartOrStopWsAsync(isResetFailCount: false);
        }

        // 是否主矿池和备矿池都是用户名密码模式的矿池
        public bool IsAllMainCoinPoolIsUserMode {
            get {
                if (CoinVm == null || CoinVm.CoinProfile == null) {
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
                        return mainCoinPool.IsUserMode;
                    }
                    return mainCoinPool.IsUserMode && mainCoinPool1.IsUserMode;
                }
                return mainCoinPool.IsUserMode;
            }
        }

        public IMineWork MineWork {
            get {
                return NTMinerContext.Instance.MinerProfile.MineWork;
            }
        }

        public bool IsFreeClient {
            get {
                return MineWork == null || ClientAppType.IsMinerStudio;
            }
        }

        public Guid Id {
            get { return NTMinerContext.Instance.MinerProfile.GetId(); }
        }

        public Guid GetId() {
            return this.Id;
        }

        public string MinerName {
            get {
                string minerName = NTMinerContext.Instance.MinerProfile.MinerName;
                return minerName;
            }
            set {
                if (NTMinerContext.Instance.MinerProfile.MinerName != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(MinerName), value);
                    NTMinerContext.RefreshArgsAssembly.Invoke("MinerProfile上放置的挖矿矿机名发生了变更");
                    OnPropertyChanged(nameof(MinerName));
                }
            }
        }

        public bool IsShowInTaskbar {
            get => NTMinerContext.Instance.MinerProfile.IsShowInTaskbar;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsShowInTaskbar != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsShowInTaskbar), value);
                    OnPropertyChanged(nameof(IsShowInTaskbar));
                }
            }
        }

        public bool IsNoUi {
            get { return NTMinerContext.Instance.MinerProfile.IsNoUi; }
            set {
                if (NTMinerContext.Instance.MinerProfile.IsNoUi != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsNoUi), value);
                    OnPropertyChanged(nameof(IsNoUi));
                }
            }
        }

        public bool IsAutoNoUi {
            get { return NTMinerContext.Instance.MinerProfile.IsAutoNoUi; }
            set {
                if (NTMinerContext.Instance.MinerProfile.IsAutoNoUi != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoNoUi), value);
                    OnPropertyChanged(nameof(IsAutoNoUi));
                }
            }
        }

        public int AutoNoUiMinutes {
            get { return NTMinerContext.Instance.MinerProfile.AutoNoUiMinutes; }
            set {
                if (NTMinerContext.Instance.MinerProfile.AutoNoUiMinutes != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(AutoNoUiMinutes), value);
                    OnPropertyChanged(nameof(AutoNoUiMinutes));
                }
            }
        }

        public bool IsShowNotifyIcon {
            get => NTMinerContext.Instance.MinerProfile.IsShowNotifyIcon;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsShowNotifyIcon != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsShowNotifyIcon), value);
                    OnPropertyChanged(nameof(IsShowNotifyIcon));
                    AppRoot.NotifyIcon?.RefreshIcon();
                }
            }
        }

        public bool IsCloseMeanExit {
            get => NTMinerContext.Instance.MinerProfile.IsCloseMeanExit;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsCloseMeanExit != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsCloseMeanExit), value);
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
            get => NTMinerContext.Instance.MinerProfile.IsAutoBoot;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsAutoBoot != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoBoot), value);
                    OnPropertyChanged(nameof(IsAutoBoot));
                }
            }
        }

        public bool IsAutoStart {
            get => NTMinerContext.Instance.MinerProfile.IsAutoStart;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsAutoStart != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoStart), value);
                    OnPropertyChanged(nameof(IsAutoStart));
                }
            }
        }

        public bool IsAutoDisableWindowsFirewall {
            get => NTMinerContext.Instance.MinerProfile.IsAutoDisableWindowsFirewall;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsAutoDisableWindowsFirewall != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoDisableWindowsFirewall), value);
                    OnPropertyChanged(nameof(IsAutoDisableWindowsFirewall));
                }
            }
        }

        public bool IsDisableUAC {
            get => NTMinerContext.Instance.MinerProfile.IsDisableUAC;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsDisableUAC != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsDisableUAC), value);
                    OnPropertyChanged(nameof(IsDisableUAC));
                }
            }
        }

        public bool IsDisableWAU {
            get => NTMinerContext.Instance.MinerProfile.IsDisableWAU;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsDisableWAU != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsDisableWAU), value);
                    OnPropertyChanged(nameof(IsDisableWAU));
                }
            }
        }

        public bool IsDisableAntiSpyware {
            get => NTMinerContext.Instance.MinerProfile.IsDisableAntiSpyware;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsDisableAntiSpyware != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsDisableAntiSpyware), value);
                    OnPropertyChanged(nameof(IsDisableAntiSpyware));
                }
            }
        }

        public bool IsNoShareRestartKernel {
            get => NTMinerContext.Instance.MinerProfile.IsNoShareRestartKernel;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsNoShareRestartKernel != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsNoShareRestartKernel), value);
                    OnPropertyChanged(nameof(IsNoShareRestartKernel));
                }
            }
        }

        public int NoShareRestartKernelMinutes {
            get => NTMinerContext.Instance.MinerProfile.NoShareRestartKernelMinutes;
            set {
                if (NTMinerContext.Instance.MinerProfile.NoShareRestartKernelMinutes != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(NoShareRestartKernelMinutes), value);
                    OnPropertyChanged(nameof(NoShareRestartKernelMinutes));
                }
            }
        }

        public bool IsNoShareRestartComputer {
            get => NTMinerContext.Instance.MinerProfile.IsNoShareRestartComputer;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsNoShareRestartComputer != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsNoShareRestartComputer), value);
                    OnPropertyChanged(nameof(IsNoShareRestartComputer));
                }
            }
        }

        public int NoShareRestartComputerMinutes {
            get => NTMinerContext.Instance.MinerProfile.NoShareRestartComputerMinutes;
            set {
                if (NTMinerContext.Instance.MinerProfile.NoShareRestartComputerMinutes != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(NoShareRestartComputerMinutes), value);
                    OnPropertyChanged(nameof(NoShareRestartComputerMinutes));
                }
            }
        }

        public bool IsPeriodicRestartKernel {
            get => NTMinerContext.Instance.MinerProfile.IsPeriodicRestartKernel;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsPeriodicRestartKernel != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsPeriodicRestartKernel), value);
                    OnPropertyChanged(nameof(IsPeriodicRestartKernel));
                }
            }
        }

        public int PeriodicRestartKernelHours {
            get => NTMinerContext.Instance.MinerProfile.PeriodicRestartKernelHours;
            set {
                if (NTMinerContext.Instance.MinerProfile.PeriodicRestartKernelHours != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(PeriodicRestartKernelHours), value);
                    OnPropertyChanged(nameof(PeriodicRestartKernelHours));
                }
            }
        }

        public bool IsPeriodicRestartComputer {
            get => NTMinerContext.Instance.MinerProfile.IsPeriodicRestartComputer;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsPeriodicRestartComputer != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsPeriodicRestartComputer), value);
                    OnPropertyChanged(nameof(IsPeriodicRestartComputer));
                }
            }
        }

        public int PeriodicRestartComputerHours {
            get => NTMinerContext.Instance.MinerProfile.PeriodicRestartComputerHours;
            set {
                if (NTMinerContext.Instance.MinerProfile.PeriodicRestartComputerHours != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(PeriodicRestartComputerHours), value);
                    OnPropertyChanged(nameof(PeriodicRestartComputerHours));
                }
            }
        }

        public bool IsAutoRestartKernel {
            get => NTMinerContext.Instance.MinerProfile.IsAutoRestartKernel;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsAutoRestartKernel != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoRestartKernel), value);
                    OnPropertyChanged(nameof(IsAutoRestartKernel));
                }
            }
        }

        public int AutoRestartKernelTimes {
            get => NTMinerContext.Instance.MinerProfile.AutoRestartKernelTimes;
            set {
                if (value < 3) {
                    value = 3;
                }
                if (NTMinerContext.Instance.MinerProfile.AutoRestartKernelTimes != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(AutoRestartKernelTimes), value);
                    OnPropertyChanged(nameof(AutoRestartKernelTimes));
                }
            }
        }

        public bool IsSpeedDownRestartComputer {
            get => NTMinerContext.Instance.MinerProfile.IsSpeedDownRestartComputer;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsSpeedDownRestartComputer != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsSpeedDownRestartComputer), value);
                    OnPropertyChanged(nameof(IsSpeedDownRestartComputer));
                }
            }
        }

        public int PeriodicRestartKernelMinutes {
            get => NTMinerContext.Instance.MinerProfile.PeriodicRestartKernelMinutes;
            set {
                if (NTMinerContext.Instance.MinerProfile.PeriodicRestartKernelMinutes != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(PeriodicRestartKernelMinutes), value);
                    OnPropertyChanged(nameof(PeriodicRestartKernelMinutes));
                    if (value < 0 || value > 60) {
                        throw new ValidationException("无效的值");
                    }
                }
            }
        }

        public int PeriodicRestartComputerMinutes {
            get => NTMinerContext.Instance.MinerProfile.PeriodicRestartComputerMinutes;
            set {
                if (NTMinerContext.Instance.MinerProfile.PeriodicRestartComputerMinutes != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(PeriodicRestartComputerMinutes), value);
                    OnPropertyChanged(nameof(PeriodicRestartComputerMinutes));
                    if (value < 0 || value > 60) {
                        throw new ValidationException("无效的值");
                    }
                }
            }
        }

        public int RestartComputerSpeedDownPercent {
            get => NTMinerContext.Instance.MinerProfile.RestartComputerSpeedDownPercent;
            set {
                if (NTMinerContext.Instance.MinerProfile.RestartComputerSpeedDownPercent != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(RestartComputerSpeedDownPercent), value);
                    OnPropertyChanged(nameof(RestartComputerSpeedDownPercent));
                }
            }
        }

        public bool IsNetUnavailableStopMine {
            get => NTMinerContext.Instance.MinerProfile.IsNetUnavailableStopMine;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsNetUnavailableStopMine != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsNetUnavailableStopMine), value);
                    OnPropertyChanged(nameof(IsNetUnavailableStopMine));
                    if (value) {
                        IsNetAvailableStartMine = true;
                    }
                }
            }
        }

        public int NetUnavailableStopMineMinutes {
            get => NTMinerContext.Instance.MinerProfile.NetUnavailableStopMineMinutes;
            set {
                if (NTMinerContext.Instance.MinerProfile.NetUnavailableStopMineMinutes != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(NetUnavailableStopMineMinutes), value);
                    OnPropertyChanged(nameof(NetUnavailableStopMineMinutes));
                }
            }
        }

        public bool IsNetAvailableStartMine {
            get => NTMinerContext.Instance.MinerProfile.IsNetAvailableStartMine;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsNetAvailableStartMine != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsNetAvailableStartMine), value);
                    OnPropertyChanged(nameof(IsNetAvailableStartMine));
                }
            }
        }

        public int NetAvailableStartMineSeconds {
            get => NTMinerContext.Instance.MinerProfile.NetAvailableStartMineSeconds;
            set {
                if (NTMinerContext.Instance.MinerProfile.NetAvailableStartMineSeconds != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(NetAvailableStartMineSeconds), value);
                    OnPropertyChanged(nameof(NetAvailableStartMineSeconds));
                }
            }
        }

        public bool IsEChargeEnabled {
            get => NTMinerContext.Instance.MinerProfile.IsEChargeEnabled;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsEChargeEnabled != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsEChargeEnabled), value);
                    OnPropertyChanged(nameof(IsEChargeEnabled));
                }
            }
        }

        public double EPrice {
            get => NTMinerContext.Instance.MinerProfile.EPrice;
            set {
                if (NTMinerContext.Instance.MinerProfile.EPrice != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(EPrice), value);
                    OnPropertyChanged(nameof(EPrice));
                }
            }
        }

        public bool IsPowerAppend {
            get => NTMinerContext.Instance.MinerProfile.IsPowerAppend;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsPowerAppend != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsPowerAppend), value);
                    OnPropertyChanged(nameof(IsPowerAppend));
                }
            }
        }

        public int PowerAppend {
            get => NTMinerContext.Instance.MinerProfile.PowerAppend;
            set {
                if (NTMinerContext.Instance.MinerProfile.PowerAppend != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(PowerAppend), value);
                    OnPropertyChanged(nameof(PowerAppend));
                }
            }
        }

        public bool IsShowCommandLine {
            get { return NTMinerContext.Instance.MinerProfile.IsShowCommandLine; }
            set {
                if (NTMinerContext.Instance.MinerProfile.IsShowCommandLine != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsShowCommandLine), value);
                    OnPropertyChanged(nameof(IsShowCommandLine));
                }
            }
        }

        private void CreateShortcut() {
            if (!ClientAppType.IsMinerClient) {
                return;
            }
            bool isDo = !File.Exists(_linkFileFullName);
            if (!isDo) {
                string targetPath = WindowsShortcut.GetTargetPath(_linkFileFullName);
                isDo = !VirtualRoot.AppFileFullName.Equals(targetPath, StringComparison.OrdinalIgnoreCase);
            }
            if (isDo) {
                WindowsShortcut.CreateShortcut(_linkFileFullName, VirtualRoot.AppFileFullName);
            }
        }

        public bool IsCreateShortcut {
            get { return NTMinerContext.Instance.MinerProfile.IsCreateShortcut; }
            set {
                if (NTMinerContext.Instance.MinerProfile.IsCreateShortcut != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsCreateShortcut), value);
                    OnPropertyChanged(nameof(IsCreateShortcut));
                    if (ClientAppType.IsMinerClient) {
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
            get => NTMinerContext.Instance.MinerProfile.CoinId;
            set {
                if (NTMinerContext.Instance.MinerProfile.CoinId != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(CoinId), value);
                    OnPropertyChanged(nameof(CoinId));
                }
            }
        }

        public int MaxTemp {
            get => NTMinerContext.Instance.MinerProfile.MaxTemp;
            set {
                if (NTMinerContext.Instance.MinerProfile.MaxTemp != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(MaxTemp), value);
                    OnPropertyChanged(nameof(MaxTemp));
                }
            }
        }

        public int AutoStartDelaySeconds {
            get => NTMinerContext.Instance.MinerProfile.AutoStartDelaySeconds;
            set {
                if (NTMinerContext.Instance.MinerProfile.AutoStartDelaySeconds != value) {
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(AutoStartDelaySeconds), value);
                    OnPropertyChanged(nameof(AutoStartDelaySeconds));
                }
            }
        }

        public bool IsAutoStopByCpu {
            get => NTMinerContext.Instance.MinerProfile.IsAutoStopByCpu;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsAutoStopByCpu != value) {
                    NTMinerContext.Instance.CpuPackage.Reset();
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoStopByCpu), value);
                    OnPropertyChanged(nameof(IsAutoStopByCpu));
                }
            }
        }

        public int CpuStopTemperature {
            get => NTMinerContext.Instance.MinerProfile.CpuStopTemperature;
            set {
                if (NTMinerContext.Instance.MinerProfile.CpuStopTemperature != value) {
                    NTMinerContext.Instance.CpuPackage.Reset();
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(CpuStopTemperature), value);
                    OnPropertyChanged(nameof(CpuStopTemperature));
                }
            }
        }

        public int CpuGETemperatureSeconds {
            get => NTMinerContext.Instance.MinerProfile.CpuGETemperatureSeconds;
            set {
                if (NTMinerContext.Instance.MinerProfile.CpuGETemperatureSeconds != value) {
                    NTMinerContext.Instance.CpuPackage.Reset();
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(CpuGETemperatureSeconds), value);
                    OnPropertyChanged(nameof(CpuGETemperatureSeconds));
                }
            }
        }

        public bool IsAutoStartByCpu {
            get => NTMinerContext.Instance.MinerProfile.IsAutoStartByCpu;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsAutoStartByCpu != value) {
                    NTMinerContext.Instance.CpuPackage.Reset();
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsAutoStartByCpu), value);
                    OnPropertyChanged(nameof(IsAutoStartByCpu));
                }
            }
        }

        public int CpuStartTemperature {
            get => NTMinerContext.Instance.MinerProfile.CpuStartTemperature;
            set {
                if (NTMinerContext.Instance.MinerProfile.CpuStartTemperature != value) {
                    NTMinerContext.Instance.CpuPackage.Reset();
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(CpuStartTemperature), value);
                    OnPropertyChanged(nameof(CpuStartTemperature));
                }
            }
        }

        public int CpuLETemperatureSeconds {
            get => NTMinerContext.Instance.MinerProfile.CpuLETemperatureSeconds;
            set {
                if (NTMinerContext.Instance.MinerProfile.CpuLETemperatureSeconds != value) {
                    NTMinerContext.Instance.CpuPackage.Reset();
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(CpuLETemperatureSeconds), value);
                    OnPropertyChanged(nameof(CpuLETemperatureSeconds));
                }
            }
        }

        public bool IsRaiseHighCpuEvent {
            get => NTMinerContext.Instance.MinerProfile.IsRaiseHighCpuEvent;
            set {
                if (NTMinerContext.Instance.MinerProfile.IsRaiseHighCpuEvent != value) {
                    NTMinerContext.Instance.CpuPackage.Reset();
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(IsRaiseHighCpuEvent), value);
                    OnPropertyChanged(nameof(IsRaiseHighCpuEvent));
                }
            }
        }

        public int HighCpuBaseline {
            get => NTMinerContext.Instance.MinerProfile.HighCpuBaseline;
            set {
                if (NTMinerContext.Instance.MinerProfile.HighCpuBaseline != value) {
                    NTMinerContext.Instance.CpuPackage.Reset();
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(HighCpuBaseline), value);
                    OnPropertyChanged(nameof(HighCpuBaseline));
                }
            }
        }

        public int HighCpuSeconds {
            get => NTMinerContext.Instance.MinerProfile.HighCpuSeconds;
            set {
                if (NTMinerContext.Instance.MinerProfile.HighCpuSeconds != value) {
                    NTMinerContext.Instance.CpuPackage.Reset();
                    NTMinerContext.Instance.MinerProfile.SetMinerProfileProperty(nameof(HighCpuSeconds), value);
                    OnPropertyChanged(nameof(HighCpuSeconds));
                }
            }
        }

        public CoinViewModel CoinVm {
            get {
                if (!AppRoot.CoinVms.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm) || !coinVm.IsSupported) {
                    coinVm = AppRoot.CoinVms.MainCoins.Where(a => a.IsSupported).FirstOrDefault();
                    if (coinVm != null) {
                        CoinId = coinVm.Id;
                    }
                }
                return coinVm;
            }
            set {
                if (value == null) {
                    value = AppRoot.CoinVms.MainCoins.Where(a => a.IsSupported).OrderBy(a => a.Code).FirstOrDefault();
                }
                if (value != null) {
                    this.CoinId = value.Id;
                    OnPropertyChanged(nameof(CoinVm));
                    NTMinerContext.RefreshArgsAssembly.Invoke("MinerProfile上引用的主挖币种发生了切换");
                    AppRoot.MinerProfileVm.OnPropertyChanged(nameof(AppRoot.MinerProfileVm.IsAllMainCoinPoolIsUserMode));
                    foreach (var item in AppRoot.GpuSpeedViewModels.Instance.List) {
                        item.OnPropertyChanged(nameof(item.GpuProfileVm));
                    }
                }
            }
        }

        public bool IsWorker {
            get {
                return MineWork != null && !ClientAppType.IsMinerStudio;
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
