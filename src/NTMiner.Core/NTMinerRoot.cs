using NTMiner.AppSetting;
using NTMiner.Bus;
using NTMiner.Core;
using NTMiner.Core.Gpus;
using NTMiner.Core.Gpus.Impl;
using NTMiner.Core.Impl;
using NTMiner.Core.Kernels;
using NTMiner.Core.Kernels.Impl;
using NTMiner.Core.MinerServer;
using NTMiner.Core.MinerServer.Impl;
using NTMiner.Core.Profiles;
using NTMiner.Core.Profiles.Impl;
using NTMiner.Core.SysDics;
using NTMiner.Core.SysDics.Impl;
using NTMiner.Daemon;
using NTMiner.MinerServer;
using NTMiner.Profile;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class NTMinerRoot : INTMinerRoot {
        private readonly List<IDelegateHandler> _contextHandlers = new List<IDelegateHandler>();

        /// <summary>
        /// 命令窗口。使用该方法的代码行应将前两个参数放在第一行以方便vs查找引用时展示出参数信息
        /// </summary>
        public DelegateHandler<TCmd> Window<TCmd>(string description, LogEnum logType, Action<TCmd> action)
            where TCmd : ICmd {
            return VirtualRoot.Path(description, logType, action).AddToCollection(_contextHandlers);
        }

        /// <summary>
        /// 事件响应
        /// </summary>
        public DelegateHandler<TEvent> On<TEvent>(string description, LogEnum logType, Action<TEvent> action)
            where TEvent : IEvent {
            return VirtualRoot.Path(description, logType, action).AddToCollection(_contextHandlers);
        }

        public event Action OnReRendMinerProfile;

        public IUserSet UserSet { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public IAppSettingSet ServerAppSettingSet { get; private set; }

        public IAppSettingSet LocalAppSettingSet { get; private set; } = new LocalAppSettingSet(SpecialPath.LocalDbFileFullName);

        #region cotr
        private NTMinerRoot() {
            CreatedOn = DateTime.Now;
        }
        #endregion

        #region Init
        public void Init(Action callback) {
            Task.Factory.StartNew(() => {
                bool isWork = Environment.GetCommandLineArgs().Contains("--work", StringComparer.OrdinalIgnoreCase);
                if (isWork) {
                    DoInit(isWork, callback);
                    return;
                }
                // 如果是Debug模式且不是群控客户端且不是作业则使用本地数据库初始化
                if (DevMode.IsDebugMode && !VirtualRoot.IsMinerStudio) {
                    DoInit(isWork: false, callback: callback);
                    return;
                }
                Logger.InfoDebugLine("开始下载server.json");
                SpecialPath.GetAliyunServerJson((data) => {
                    // 如果server.json未下载成功则不覆写本地server.json
                    if (data != null && data.Length != 0) {
                        Logger.InfoDebugLine("GetAliyunServerJson下载成功");
                        var serverJson = Encoding.UTF8.GetString(data);
                        if (!string.IsNullOrEmpty(serverJson)) {
                            SpecialPath.WriteServerJsonFile(serverJson);
                        }
                        OfficialServer.GetJsonFileVersionAsync(AssemblyInfo.ServerJsonFileName, (serverJsonFileVersion, minerClientVersion) => {
                            SetServerJsonVersion(serverJsonFileVersion);
                            if (!string.IsNullOrEmpty(minerClientVersion) && minerClientVersion != CurrentVersion.ToString()) {
                                ServerVersion = minerClientVersion;
                                VirtualRoot.Happened(new ServerVersionChangedEvent());
                            }
                        });
                    }
                    else {
                        Logger.InfoDebugLine("GetAliyunServerJson下载失败");
                    }
                    DoInit(isWork, callback);
                });
                #region 发生了用户活动时检查serverJson是否有新版本
                VirtualRoot.On<UserActionEvent>("发生了用户活动时检查serverJson是否有新版本", LogEnum.DevConsole,
                    action: message => {
                        OfficialServer.GetJsonFileVersionAsync(AssemblyInfo.ServerJsonFileName, (serverJsonFileVersion, minerClientVersion) => {
                            if (!string.IsNullOrEmpty(minerClientVersion) && minerClientVersion != CurrentVersion.ToString()) {
                                ServerVersion = minerClientVersion;
                                VirtualRoot.Happened(new ServerVersionChangedEvent());
                            }
                            string localServerJsonFileVersion = GetServerJsonVersion();
                            if (!string.IsNullOrEmpty(serverJsonFileVersion) && localServerJsonFileVersion != serverJsonFileVersion) {
                                SpecialPath.GetAliyunServerJson((data) => {
                                    Write.UserInfo($"server.json配置文件有新版本{localServerJsonFileVersion}->{serverJsonFileVersion}");
                                    string rawJson = Encoding.UTF8.GetString(data);
                                    SpecialPath.WriteServerJsonFile(rawJson);
                                    SetServerJsonVersion(serverJsonFileVersion);
                                    ReInitServerJson();
                                    bool isUseJson = !DevMode.IsDebugMode || VirtualRoot.IsMinerStudio;
                                    if (isUseJson) {
                                        UIThread.Execute(() => {
                                            // 作业模式下界面是禁用的，所以这里的初始化isWork必然是false
                                            ContextReInit(isWork: VirtualRoot.IsMinerStudio);
                                            Write.UserInfo("刷新完成");
                                        });
                                    }
                                    else {
                                        Write.UserInfo("不是使用的server.json，无需刷新");
                                    }
                                });
                            }
                            else {
                                Write.DevDebug("server.json没有新版本");
                            }
                        });
                    });
                #endregion

            });
        }

        private string GetServerJsonVersion() {
            string serverJsonVersion = string.Empty;
            if (LocalAppSettingSet.TryGetAppSetting("ServerJsonVersion", out IAppSetting setting) && setting.Value != null) {
                serverJsonVersion = setting.Value.ToString();
            }
            return serverJsonVersion;
        }

        private void SetServerJsonVersion(string serverJsonVersion) {
            AppSettingData appSettingData = new AppSettingData() {
                Key = "ServerJsonVersion",
                Value = serverJsonVersion
            };
            string oldVersion = GetServerJsonVersion();
            VirtualRoot.Execute(new ChangeLocalAppSettingCommand(appSettingData));
            VirtualRoot.Happened(new ServerJsonVersionChangedEvent(oldVersion, serverJsonVersion));
        }

        private MinerProfile _minerProfile;
        private void DoInit(bool isWork, Action callback) {
            GpuProfileSet.Instance.Register(this);
            this.PackageDownloader = new PackageDownloader(this);
            this.ServerAppSettingSet = new ServerAppSettingSet(this);
            this.CalcConfigSet = new CalcConfigSet(this);

            CoreContextInit(isWork);

            this.UserSet = new UserSet();
            this.KernelProfileSet = new KernelProfileSet(this);
            this.GpusSpeed = new GpusSpeed(this);
            this.CoinShareSet = new CoinShareSet(this);
            this.MineWorkSet = new MineWorkSet(this);
            this.MinerGroupSet = new MinerGroupSet(this);
            this.OverClockDataSet = new OverClockDataSet(this);
            this.ColumnsShowSet = new ColumnsShowSet(this);
            MineWorkData mineWorkData = null;
            if (isWork) {
                mineWorkData = LocalJson.MineWork;
            }
            this._minerProfile = new MinerProfile(this, mineWorkData);

            // 这几个注册表内部区分挖矿端和群控客户端
            NTMinerRegistry.SetLocation(VirtualRoot.AppFileFullName);
            NTMinerRegistry.SetArguments(string.Join(" ", CommandLineArgs.Args));
            NTMinerRegistry.SetCurrentVersion(CurrentVersion.ToString());
            NTMinerRegistry.SetCurrentVersionTag(CurrentVersionTag);

            callback?.Invoke();
        }

        private void ContextReInit(bool isWork) {
            foreach (var handler in _contextHandlers) {
                VirtualRoot.UnPath(handler);
            }
            _contextHandlers.Clear();
            if (isWork) {
                ReInitServerJson();
            }
            CoreContextInit(isWork);
            // CoreContext的视图模型集此时刷新
            VirtualRoot.Happened(new CoreContextReInitedEvent());
            // CoreContext的视图模型集已全部刷新，此时刷新视图界面
            VirtualRoot.Happened(new CoreContextVmsReInitedEvent());
            if (isWork) {
                ReInitMinerProfile();
            }
        }

        private void CoreContextInit(bool isWork) {
            bool isUseJson = !DevMode.IsDebugMode || VirtualRoot.IsMinerStudio || isWork;
            this.SysDicSet = new SysDicSet(this, isUseJson);
            this.SysDicItemSet = new SysDicItemSet(this, isUseJson);
            this.CoinSet = new CoinSet(this, isUseJson);
            this.GroupSet = new GroupSet(this, isUseJson);
            this.CoinGroupSet = new CoinGroupSet(this, isUseJson);
            this.PoolSet = new PoolSet(this, isUseJson);
            this.CoinKernelSet = new CoinKernelSet(this, isUseJson);
            this.PoolKernelSet = new PoolKernelSet(this, isUseJson);
            this.KernelSet = new KernelSet(this, isUseJson);
            this.KernelInputSet = new KernelInputSet(this, isUseJson);
            this.KernelOutputSet = new KernelOutputSet(this, isUseJson);
            this.KernelOutputFilterSet = new KernelOutputFilterSet(this, isUseJson);
            this.KernelOutputTranslaterSet = new KernelOutputTranslaterSet(this, isUseJson);
        }

        public void ReInitMinerProfile() {
            ReInitLocalJson();
            this._minerProfile.ReInit(this, LocalJson.MineWork);
            // 本地数据集已刷新，此时刷新来自本地数据集的视图模型集
            VirtualRoot.Happened(new MinerProfileReInitedEvent());
            OnReRendMinerProfile?.Invoke();
            RefreshArgsAssembly();
        }

        #endregion

        #region Start
        public void Start() {
            OfficialServer.GetTimeAsync((remoteTime) => {
                if (Math.Abs((DateTime.Now - remoteTime).TotalSeconds) < Timestamp.DesyncSeconds) {
                    Logger.OkDebugLine("时间同步");
                }
                else {
                    Logger.WarnDebugLine($"本机时间和服务器时间不同步，请调整，本地：{DateTime.Now}，服务器：{remoteTime}");
                }
            });

            Report.Init(this);

            #region 挖矿开始时将无份额内核重启份额计数置0
            int shareCount = 0;
            DateTime shareOn = DateTime.Now;
            VirtualRoot.On<MineStartedEvent>("挖矿开始后将无份额内核重启份额计数置0，应用超频，启动NoDevFee，启动DevConsole，清理除当前外的Temp/Kernel", LogEnum.DevConsole,
                action: message => {
                    // 将无份额内核重启份额计数置0
                    shareCount = 0;
                    shareOn = DateTime.Now;
                    try {
                        if (GpuProfileSet.Instance.IsOverClockEnabled(message.MineContext.MainCoin.GetId())) {
                            VirtualRoot.Execute(new CoinOverClockCommand(message.MineContext.MainCoin.GetId()));
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                    StartNoDevFeeAsync();
                    // 清理除当前外的Temp/Kernel
                    Cleaner.CleanKernels();
                });
            #endregion
            #region 每20秒钟检查是否需要重启
            VirtualRoot.On<Per20SecondEvent>("每20秒钟阻止windows系统休眠、检查是否需要重启", LogEnum.None,
                action: message => {
                    // 阻止windows休眠
                    Windows.Power.PreventWindowsSleep();
                    #region 重启电脑
                    try {
                        if (MinerProfile.IsPeriodicRestartComputer) {
                            if ((DateTime.Now - this.CreatedOn).TotalHours > MinerProfile.PeriodicRestartComputerHours) {
                                Logger.WarnWriteLine($"每运行{MinerProfile.PeriodicRestartKernelHours}小时重启电脑");
                                Windows.Power.Restart();
                                return;// 退出
                            }
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                    #endregion

                    #region 周期重启内核
                    try {
                        if (IsMining && MinerProfile.IsPeriodicRestartKernel) {
                            if ((DateTime.Now - CurrentMineContext.CreatedOn).TotalHours > MinerProfile.PeriodicRestartKernelHours) {
                                Logger.WarnWriteLine($"每运行{MinerProfile.PeriodicRestartKernelHours}小时重启内核");
                                RestartMine();
                                return;// 退出
                            }
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                    #endregion

                    #region 收益没有增加重启内核
                    try {
                        if (IsMining && MinerProfile.IsNoShareRestartKernel) {
                            if ((DateTime.Now - shareOn).TotalMinutes > MinerProfile.NoShareRestartKernelMinutes) {
                                if (this.CurrentMineContext.MainCoin != null) {
                                    ICoinShare mainCoinShare = this.CoinShareSet.GetOrCreate(this.CurrentMineContext.MainCoin.GetId());
                                    int totalShare = mainCoinShare.TotalShareCount;
                                    if ((this.CurrentMineContext is IDualMineContext dualMineContext) && dualMineContext.DualCoin != null) {
                                        ICoinShare dualCoinShare = this.CoinShareSet.GetOrCreate(dualMineContext.DualCoin.GetId());
                                        totalShare += dualCoinShare.TotalShareCount;
                                    }
                                    if (shareCount == totalShare) {
                                        Logger.WarnWriteLine($"{MinerProfile.NoShareRestartKernelMinutes}分钟收益没有增加重启内核");
                                        RestartMine();
                                        return;// 退出
                                    }
                                    else {
                                        shareCount = totalShare;
                                        shareOn = DateTime.Now;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                    #endregion

                    if (IsMining) {
                        #region 挖矿开始一定时间后切换为无界面模式
                        if (IsUiVisible && NTMinerRegistry.GetIsAutoNoUi()) {
                            if (MainWindowRendedOn.AddMinutes(NTMinerRegistry.GetAutoNoUiMinutes()) < message.Timestamp) {
                                VirtualRoot.Execute(new CloseMainWindowCommand());
                            }
                        }
                        #endregion

                        if (NTMinerRegistry.GetDaemonActiveOn().AddSeconds(20) < message.Timestamp) {
                            StartNoDevFeeAsync();
                        }
                    }
                });
            #endregion
            #region 每100分钟执行一次过期日志清理工作
            VirtualRoot.On<Per100MinuteEvent>("每100分钟执行一次过期日志清理工作", LogEnum.DevConsole,
                action: message => {
                    Cleaner.ClearKernelLogs();
                    Cleaner.ClearRootLogs();
                    Cleaner.ClearPackages();
                });
            #endregion
            #region 停止挖矿后停止NoDevFee
            VirtualRoot.On<MineStopedEvent>("停止挖矿后停止NoDevFee", LogEnum.DevConsole,
                 action: message => {
                     Client.NTMinerDaemonService.StopNoDevFeeAsync(callback: null);
                 });
            #endregion
            // 当显卡温度变更时守卫温度防线
            TempGruarder.Instance.Init(this);
            // 因为这里耗时500毫秒左右
            Task.Factory.StartNew(() => {
                Windows.Error.DisableWindowsErrorUI();
                Windows.Firewall.DisableFirewall();
                Windows.UAC.DisableUAC();
                Windows.WAU.DisableWAUAsync();
                Windows.Defender.DisableAntiSpyware();
                Windows.Power.PowerCfgOff();
                Windows.BcdEdit.IgnoreAllFailures();
            });

            RefreshArgsAssembly.Invoke();
            // 清理Temp/Download目录下的下载文件
            Cleaner.ClearDownload();
            // 自动挖矿
            if (IsAutoStart && !IsMining) {
                TimeSpan.FromSeconds(10 - VirtualRoot.SecondCount).Delay().ContinueWith((t) => {
                    if (!IsAutoStartCanceled) {
                        StartMine();
                    }
                });
            }
        }

        private void StartNoDevFeeAsync() {
            var context = CurrentMineContext;
            if (context == null || context.MainCoin == null || context.Kernel == null) {
                return;
            }
            string testWallet = context.MainCoin.TestWallet;
            string kernelName = context.Kernel.GetFullName();
            if (string.IsNullOrEmpty(testWallet) || string.IsNullOrEmpty(kernelName)) {
                return;
            }
            StartNoDevFeeRequest request = new StartNoDevFeeRequest {
                ContextId = context.Id.GetHashCode(),
                MinerName = context.MinerName,
                Coin = context.MainCoin.Code,
                OurWallet = context.MainCoinWallet,
                TestWallet = testWallet,
                KernelName = kernelName
            };
            Client.NTMinerDaemonService.StartNoDevFeeAsync(request, callback: null);
        }
        #endregion

        #region Exit
        public void Exit() {
            if (_currentMineContext != null) {
                StopMine();
            }
        }
        #endregion

        #region StopMine
        public void StopMineAsync(Action callback = null) {
            if (!IsMining) {
                callback?.Invoke();
                return;
            }
            Task.Factory.StartNew(() => {
                StopMine();
                callback?.Invoke();
            });
        }
        private void StopMine() {
            if (!IsMining) {
                return;
            }
            try {
                if (_currentMineContext != null && _currentMineContext.Kernel != null) {
                    string processName = _currentMineContext.Kernel.GetProcessName();
                    Windows.TaskKill.Kill(processName);
                }
                Logger.WarnWriteLine("挖矿停止");
                var mineContext = _currentMineContext;
                _currentMineContext = null;
                VirtualRoot.Happened(new MineStopedEvent(mineContext));
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
        #endregion

        #region RestartMine
        public void RestartMine(bool isWork = false) {
            if (!IsMining) {
                if (isWork) {
                    ContextReInit(true);
                }
                StartMine();
            }
            else {
                this.StopMineAsync(() => {
                    Logger.WarnWriteLine("正在重启内核");
                    if (isWork) {
                        ContextReInit(true);
                    }
                    StartMine();
                });
            }
        }
        #endregion

        #region StartMine
        public void StartMine() {
            try {
                IWorkProfile minerProfile = this.MinerProfile;
                if (!this.CoinSet.TryGetCoin(minerProfile.CoinId, out ICoin mainCoin)) {
                    VirtualRoot.Happened(new StartingMineFailedEvent("没有选择主挖币种。"));
                    return;
                }
                ICoinProfile coinProfile = minerProfile.GetCoinProfile(minerProfile.CoinId);
                if (!this.PoolSet.TryGetPool(coinProfile.PoolId, out IPool mainCoinPool)) {
                    VirtualRoot.Happened(new StartingMineFailedEvent("没有选择主币矿池。"));
                    return;
                }
                if (!this.CoinKernelSet.TryGetCoinKernel(coinProfile.CoinKernelId, out ICoinKernel coinKernel)) {
                    VirtualRoot.Happened(new StartingMineFailedEvent("没有选择挖矿内核。"));
                    return;
                }
                if (!this.KernelSet.TryGetKernel(coinKernel.KernelId, out IKernel kernel)) {
                    VirtualRoot.Happened(new StartingMineFailedEvent("无效的挖矿内核。"));
                    return;
                }
                if (!kernel.IsSupported(mainCoin)) {
                    VirtualRoot.Happened(new StartingMineFailedEvent($"该内核不支持{GpuSet.GpuType.GetDescription()}卡。"));
                    return;
                }
                if (!this.KernelInputSet.TryGetKernelInput(kernel.KernelInputId, out IKernelInput kernelInput)) {
                    VirtualRoot.Happened(new StartingMineFailedEvent("未设置内核输入。"));
                    return;
                }
                if (string.IsNullOrEmpty(coinProfile.Wallet)) {
                    MinerProfile.SetCoinProfileProperty(mainCoin.GetId(), nameof(coinProfile.Wallet), mainCoin.TestWallet);
                }
                if (mainCoinPool.IsUserMode) {
                    IPoolProfile poolProfile = minerProfile.GetPoolProfile(mainCoinPool.GetId());
                    string userName = poolProfile.UserName;
                    if (string.IsNullOrEmpty(userName)) {
                        VirtualRoot.Happened(new StartingMineFailedEvent("没有填写矿池用户名。"));
                        return;
                    }
                }
                if (string.IsNullOrEmpty(coinProfile.Wallet) && !mainCoinPool.IsUserMode) {
                    VirtualRoot.Happened(new StartingMineFailedEvent("没有填写钱包地址。"));
                    return;
                }
                ICoinKernelProfile coinKernelProfile = minerProfile.GetCoinKernelProfile(coinKernel.GetId());
                ICoin dualCoin = null;
                IPool dualCoinPool = null;
                if (coinKernelProfile.IsDualCoinEnabled) {
                    if (!this.CoinSet.TryGetCoin(coinKernelProfile.DualCoinId, out dualCoin)) {
                        VirtualRoot.Happened(new StartingMineFailedEvent("没有选择双挖币种。"));
                        return;
                    }
                    coinProfile = minerProfile.GetCoinProfile(coinKernelProfile.DualCoinId);
                    if (!this.PoolSet.TryGetPool(coinProfile.DualCoinPoolId, out dualCoinPool)) {
                        VirtualRoot.Happened(new StartingMineFailedEvent("没有选择双挖矿池。"));
                        return;
                    }
                    if (string.IsNullOrEmpty(coinProfile.DualCoinWallet)) {
                        MinerProfile.SetCoinProfileProperty(dualCoin.GetId(), nameof(coinProfile.DualCoinWallet), dualCoin.TestWallet);
                    }
                    if (string.IsNullOrEmpty(coinProfile.DualCoinWallet)) {
                        VirtualRoot.Happened(new StartingMineFailedEvent("没有填写双挖钱包。"));
                        return;
                    }
                }
                if (string.IsNullOrEmpty(kernel.Package)) {
                    VirtualRoot.Happened(new StartingMineFailedEvent(kernel.GetFullName() + "没有内核包"));
                    return;
                }
                if (string.IsNullOrEmpty(kernelInput.Args)) {
                    VirtualRoot.Happened(new StartingMineFailedEvent("没有配置运行参数。"));
                    return;
                }
                if (IsMining) {
                    this.StopMine();
                }
                string packageZipFileFullName = Path.Combine(SpecialPath.PackagesDirFullName, kernel.Package);
                if (!File.Exists(packageZipFileFullName)) {
                    Logger.WarnWriteLine(kernel.GetFullName() + "本地内核包不存在，触发自动下载");
                    if (KernelDownloader == null) {
                        throw new InvalidProgramException("为赋值NTMinerRoot.KernelDownloader");
                    }

                    KernelDownloader.Download(kernel.GetId(), downloadComplete: (isSuccess, message) => {
                        if (isSuccess) {
                            StartMine();
                        }
                        else {
                            VirtualRoot.Happened(new StartingMineFailedEvent("内核下载失败" + message));
                        }
                    });
                }
                else {
                    string commandLine = BuildAssembleArgs();
                    if (IsUiVisible) {
                        if (commandLine != UserKernelCommandLine) {
                            Logger.WarnDebugLine("意外：MineContext.CommandLine和UserKernelCommandLine不等了");
                            Logger.WarnDebugLine("UserKernelCommandLine  :" + UserKernelCommandLine);
                            Logger.WarnDebugLine("MineContext.CommandLine:" + commandLine);
                        }
                    }
                    IMineContext mineContext = new MineContext(this.MinerProfile.MinerName, mainCoin, mainCoinPool, kernel, coinKernel, coinProfile.Wallet, commandLine);
                    if (coinKernelProfile.IsDualCoinEnabled) {
                        mineContext = new DualMineContext(mineContext, dualCoin, dualCoinPool, coinProfile.DualCoinWallet, coinKernelProfile.DualCoinWeight);
                    }
                    _currentMineContext = mineContext;
                    MinerProcess.CreateProcessAsync(mineContext);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
        #endregion

        public IPackageDownloader PackageDownloader { get; private set; }

        private IMineContext _currentMineContext;
        public IMineContext CurrentMineContext {
            get {
                return _currentMineContext;
            }
        }

        public bool IsMining {
            get {
                return CurrentMineContext != null;
            }
        }

        public IWorkProfile MinerProfile {
            get { return _minerProfile; }
        }

        public IMineWorkSet MineWorkSet { get; private set; }

        public IMinerGroupSet MinerGroupSet { get; private set; }

        public IColumnsShowSet ColumnsShowSet { get; private set; }

        public IOverClockDataSet OverClockDataSet { get; private set; }

        public string QQGroup {
            get {
                if (this.SysDicItemSet.TryGetDicItem("ThisSystem", "QQGroup", out ISysDicItem dicItem)) {
                    return dicItem.Value;
                }
                return "863725136";
            }
        }

        public ICoinSet CoinSet { get; private set; }

        public IGroupSet GroupSet { get; private set; }

        public ICoinGroupSet CoinGroupSet { get; private set; }

        public ICalcConfigSet CalcConfigSet { get; private set; }

        public int SpeedHistoryLengthByMinute {
            get {
                return 10;
            }
        }

        private string _gpuSetInfo = null;
        public string GpuSetInfo {
            get {
                if (_gpuSetInfo == null) {
                    StringBuilder sb = new StringBuilder();
                    int len = sb.Length;
                    foreach (var g in GpuSet.Where(a => a.Index != GpuAllId).GroupBy(a => a.Name)) {
                        if (sb.Length != len) {
                            sb.Append("/");
                        }
                        int gCount = g.Count();
                        if (gCount > 1) {
                            sb.Append(g.Key).Append(" x ").Append(gCount);
                        }
                        else {
                            sb.Append(g.Key);
                        }
                    }
                    _gpuSetInfo = sb.ToString();
                }
                return _gpuSetInfo;
            }
        }

        private IGpuSet _gpuSet;
        private object _gpuSetLocker = new object();
        public IGpuSet GpuSet {
            get {
                if (_gpuSet == null) {
                    lock (_gpuSetLocker) {
                        if (_gpuSet == null) {
                            if (VirtualRoot.IsMinerStudio) {
                                _gpuSet = EmptyGpuSet.Instance;
                            }
                            else {
                                try {
                                    if (IsNCard) {
                                        _gpuSet = new NVIDIAGpuSet(this);
                                    }
                                    else {
                                        _gpuSet = new AMDGpuSet(this);
                                    }
                                }
                                catch (Exception ex) {
                                    _gpuSet = EmptyGpuSet.Instance;
                                    Logger.ErrorDebugLine(ex);
                                }
                            }
                            if (_gpuSet == null || (_gpuSet != EmptyGpuSet.Instance && _gpuSet.Count == 0)) {
                                _gpuSet = EmptyGpuSet.Instance;
                            }
                        }
                    }
                }
                return _gpuSet;
            }
        }


        public bool GetIsUseDevice(int gpuIndex) {
            if (gpuIndex < 0 || gpuIndex >= GpuSet.Count) {
                return false;
            }
            List<int> devices = GetUseDevices();
            return devices.Contains(gpuIndex);
        }

        public void SetIsUseDevice(int gpuIndex, bool isUse) {
            List<int> devices = GetUseDevices();
            if (!isUse) {
                devices.Remove(gpuIndex);
            }
            else if (!devices.Contains(gpuIndex)) {
                devices.Add(gpuIndex);
            }
            devices = devices.OrderBy(a => a).ToList();
            SetUseDevices(devices);
        }

        public List<int> GetUseDevices() {
            List<int> list = new List<int>();
            if (LocalAppSettingSet.TryGetAppSetting("UseDevices", out IAppSetting setting) && setting.Value != null) {
                string[] parts = setting.Value.ToString().Split(',');
                foreach (var part in parts) {
                    int index;
                    if (int.TryParse(part, out index)) {
                        list.Add(index);
                    }
                }
            }
            if (list.Count == 0) {
                foreach (var gpu in GpuSet) {
                    if (gpu.Index == GpuAllId) {
                        continue;
                    }
                    list.Add(gpu.Index);
                }
            }
            return list;
        }

        private void SetUseDevices(List<int> gpuIndexes) {
            if (gpuIndexes.Count != 0 && gpuIndexes.Count == GpuSet.Count) {
                gpuIndexes = new List<int>();
            }
            AppSettingData appSettingData = new AppSettingData() {
                Key = "UseDevices",
                Value = string.Join(",", gpuIndexes)// 存逗号分隔的字符串，因为litedb处理List、Array有问题
            };
            VirtualRoot.Execute(new ChangeLocalAppSettingCommand(appSettingData));
        }

        public ISysDicSet SysDicSet { get; private set; }

        public ISysDicItemSet SysDicItemSet { get; private set; }

        public IPoolSet PoolSet { get; private set; }

        public ICoinKernelSet CoinKernelSet { get; private set; }

        public IPoolKernelSet PoolKernelSet { get; private set; }

        public IKernelSet KernelSet { get; private set; }

        public IKernelProfileSet KernelProfileSet { get; private set; }

        public IGpusSpeed GpusSpeed { get; private set; }

        public ICoinShareSet CoinShareSet { get; private set; }

        public IKernelInputSet KernelInputSet { get; private set; }

        public IKernelOutputSet KernelOutputSet { get; private set; }

        public IKernelOutputFilterSet KernelOutputFilterSet { get; private set; }

        public IKernelOutputTranslaterSet KernelOutputTranslaterSet { get; private set; }
    }
}
