using Microsoft.Win32;
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
using NTMiner.MinerServer;
using NTMiner.Profile;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class NTMinerRoot : INTMinerRoot {
        private readonly List<IDelegateHandler> _serverContextHandlers = new List<IDelegateHandler>();

        /// <summary>
        /// 命令窗口。使用该方法的代码行应将前两个参数放在第一行以方便vs查找引用时展示出参数信息
        /// </summary>
        public DelegateHandler<TCmd> ServerContextWindow<TCmd>(string description, LogEnum logType, Action<TCmd> action)
            where TCmd : ICmd {
            return VirtualRoot.Path(description, logType, action).AddToCollection(_serverContextHandlers);
        }

        /// <summary>
        /// 事件响应
        /// </summary>
        public DelegateHandler<TEvent> ServerContextOn<TEvent>(string description, LogEnum logType, Action<TEvent> action)
            where TEvent : IEvent {
            return VirtualRoot.Path(description, logType, action).AddToCollection(_serverContextHandlers);
        }

        public IUserSet UserSet { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public IAppSettingSet ServerAppSettingSet { get; private set; }

        private IAppSettingSet _appSettingSet;
        public IAppSettingSet LocalAppSettingSet {
            get {
                if (_appSettingSet == null) {
                    _appSettingSet = new LocalAppSettingSet(SpecialPath.LocalDbFileFullName);
                }
                return _appSettingSet;
            }
        }

        #region cotr
        private NTMinerRoot() {
            CreatedOn = DateTime.Now;
        }
        #endregion

        #region Init
        public void Init(Action callback) {
            Task.Factory.StartNew(() => {
                bool isWork = Environment.GetCommandLineArgs().Contains("--work", StringComparer.OrdinalIgnoreCase);
                if (isWork) { // 是作业
                    DoInit(isWork, callback);
                    if (VirtualRoot.IsMinerClient) {
                        NTMinerRegistry.SetIsLastIsWork(true);
                    }
                }
                else { // 不是作业
                    if (VirtualRoot.IsMinerClient) {
                        NTMinerRegistry.SetIsLastIsWork(false);
                    }
                    // 如果是Debug模式且不是群控客户端则使用本地数据库初始化
                    bool useLocalDb = DevMode.IsDebugMode && !VirtualRoot.IsMinerStudio;
                    if (useLocalDb) {
                        DoInit(isWork: false, callback: callback);
                    }
                    else {
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
                                    AppVersionChangedEvent.PublishIfNewVersion(minerClientVersion);
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
                                RefreshServerJsonFile();
                            });
                        #endregion
                    }
                }
            });
            // 因为这个操作大概需要200毫秒
            Task.Factory.StartNew(() => {
                NVIDIAGpuSet.NvmlInit();
            });
        }

        private void RefreshServerJsonFile() {
            OfficialServer.GetJsonFileVersionAsync(AssemblyInfo.ServerJsonFileName, (serverJsonFileVersion, minerClientVersion) => {
                AppVersionChangedEvent.PublishIfNewVersion(minerClientVersion);
                string localServerJsonFileVersion = GetServerJsonVersion();
                if (!string.IsNullOrEmpty(serverJsonFileVersion) && localServerJsonFileVersion != serverJsonFileVersion) {
                    SpecialPath.GetAliyunServerJson((data) => {
                        Write.UserInfo($"更新配置{localServerJsonFileVersion}->{serverJsonFileVersion}");
                        string rawJson = Encoding.UTF8.GetString(data);
                        SpecialPath.WriteServerJsonFile(rawJson);
                        SetServerJsonVersion(serverJsonFileVersion);
                        ReInitServerJson();
                        // 作业模式下界面是禁用的，所以这里的初始化isWork必然是false
                        ContextReInit(isWork: VirtualRoot.IsMinerStudio);
                        Write.UserInfo("更新成功");
                    });
                }
                else {
                    Write.DevDebug("server.json没有新版本");
                }
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
            this.ServerAppSettingSet = new ServerAppSettingSet(this);
            this.CalcConfigSet = new CalcConfigSet(this);

            ServerContextInit(isWork);

            this.WorkerEventSet = new WorkerEventSet(this);
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

            if (VirtualRoot.IsMinerClient) {
                OfficialServer.GetTimeAsync((remoteTime) => {
                    if (Math.Abs((DateTime.Now - remoteTime).TotalSeconds) < Timestamp.DesyncSeconds) {
                        Logger.OkDebugLine("时间同步");
                    }
                    else {
                        Write.UserWarn($"本机时间和服务器时间不同步，请调整，本地：{DateTime.Now}，服务器：{remoteTime}");
                    }
                });

                Report.Init(this);
                Link();
                // 当显卡温度变更时守卫温度防线
                TempGruarder.Instance.Init(this);
                // 因为这里耗时500毫秒左右
                Task.Factory.StartNew(() => {
                    Windows.Error.DisableWindowsErrorUI();
                    if (NTMinerRegistry.GetIsAutoDisableWindowsFirewall()) {
                        Windows.Firewall.DisableFirewall();
                    }
                    Windows.UAC.DisableUAC();
                    Windows.WAU.DisableWAUAsync();
                    Windows.Defender.DisableAntiSpyware();
                    Windows.Power.PowerCfgOff();
                    Windows.BcdEdit.IgnoreAllFailures();
                });
            }

            callback?.Invoke();
        }

        private void ContextReInit(bool isWork) {
            foreach (var handler in _serverContextHandlers) {
                VirtualRoot.UnPath(handler);
            }
            _serverContextHandlers.Clear();
            if (isWork) {
                ReInitServerJson();
            }
            ServerContextInit(isWork);
            // CoreContext的视图模型集此时刷新
            VirtualRoot.Happened(new ServerContextReInitedEvent());
            // CoreContext的视图模型集已全部刷新，此时刷新视图界面
            VirtualRoot.Happened(new ServerContextVmsReInitedEvent());
            if (isWork) {
                ReInitMinerProfile();
            }
        }

        // ServerContext对应server.json
        private void ServerContextInit(bool isWork) {
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
            this.FileWriterSet = new FileWriterSet(this, isUseJson);
            this.FragmentWriterSet = new FragmentWriterSet(this, isUseJson);
            this.PackageSet = new PackageSet(this, isUseJson);
            this.KernelInputSet = new KernelInputSet(this, isUseJson);
            this.KernelOutputSet = new KernelOutputSet(this, isUseJson);
            this.KernelOutputFilterSet = new KernelOutputFilterSet(this, isUseJson);
            this.KernelOutputTranslaterSet = new KernelOutputTranslaterSet(this, isUseJson);
            this.KernelOutputKeywordSet = new KernelOutputKeywordSet(this, isUseJson);
        }

        // MinerProfile对应local.litedb或local.json
        public void ReInitMinerProfile() {
            ReInitLocalJson();
            this._minerProfile.ReInit(this, LocalJson.MineWork);
            // 本地数据集已刷新，此时刷新本地数据集的视图模型集
            VirtualRoot.Happened(new LocalContextReInitedEvent());
            // 本地数据集的视图模型已刷新，此时刷新本地数据集的视图界面
            VirtualRoot.Happened(new LocalContextVmsReInitedEvent());
            RefreshArgsAssembly();
        }

        #endregion

        private void Link() {
            VirtualRoot.Window<RegCmdHereCommand>("处理注册右键打开windows命令行菜单命令", LogEnum.DevConsole,
                action: message => {
                    try {
                        RegCmdHere();
                        VirtualRoot.Happened(new RegCmdHereEvent(true, "windows右键命令行添加成功"));
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                        VirtualRoot.Happened(new RegCmdHereEvent(false, "windows右键命令行添加失败"));
                    }
                });
            #region 挖矿开始时将无份额内核重启份额计数置0
            int shareCount = 0;
            DateTime shareOn = DateTime.Now;
            VirtualRoot.On<MineStartedEvent>("挖矿开始后将无份额内核重启份额计数置0，应用超频", LogEnum.DevConsole,
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
                        Logger.ErrorDebugLine(e);
                    }
                });
            #endregion
            #region 每20秒钟检查是否需要重启
            VirtualRoot.On<Per20SecondEvent>("每20秒钟检查是否需要重启", LogEnum.None,
                action: message => {
                    #region 重启电脑
                    try {
                        if (MinerProfile.IsPeriodicRestartComputer) {
                            if ((DateTime.Now - this.CreatedOn).TotalMinutes > 60 * MinerProfile.PeriodicRestartComputerHours + MinerProfile.PeriodicRestartComputerMinutes) {
                                Logger.WarnWriteLine($"每运行{MinerProfile.PeriodicRestartKernelHours}小时{MinerProfile.PeriodicRestartComputerMinutes}分钟重启电脑");
                                Windows.Power.Restart(10);
                                VirtualRoot.Execute(new CloseNTMinerCommand());
                                return;// 退出
                            }
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                    #endregion

                    #region 周期重启内核
                    try {
                        if (IsMining && MinerProfile.IsPeriodicRestartKernel) {
                            if ((DateTime.Now - CurrentMineContext.CreatedOn).TotalMinutes > 60 * MinerProfile.PeriodicRestartKernelHours + MinerProfile.PeriodicRestartKernelMinutes) {
                                Logger.WarnWriteLine($"每运行{MinerProfile.PeriodicRestartKernelHours}小时{MinerProfile.PeriodicRestartKernelMinutes}分钟重启内核");
                                RestartMine();
                                return;// 退出
                            }
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                    #endregion

                    #region 收益没有增加重启内核
                    try {
                        if (IsMining) {
                            if (MinerProfile.IsNoShareRestartComputer) {
                                if ((DateTime.Now - shareOn).TotalMinutes > MinerProfile.NoShareRestartComputerMinutes) {
                                    if (this.CurrentMineContext.MainCoin != null) {
                                        ICoinShare mainCoinShare = this.CoinShareSet.GetOrCreate(this.CurrentMineContext.MainCoin.GetId());
                                        int totalShare = mainCoinShare.TotalShareCount;
                                        if ((this.CurrentMineContext is IDualMineContext dualMineContext) && dualMineContext.DualCoin != null) {
                                            ICoinShare dualCoinShare = this.CoinShareSet.GetOrCreate(dualMineContext.DualCoin.GetId());
                                            totalShare += dualCoinShare.TotalShareCount;
                                        }
                                        if (shareCount == totalShare) {
                                            Logger.WarnWriteLine($"{MinerProfile.NoShareRestartComputerMinutes}分钟收益没有增加重启电脑");
                                            Windows.Power.Restart(10);
                                            VirtualRoot.Execute(new CloseNTMinerCommand());
                                            return;// 退出
                                        }
                                        else {
                                            shareCount = totalShare;
                                            shareOn = DateTime.Now;
                                        }
                                    }
                                }
                            }
                            if (MinerProfile.IsNoShareRestartKernel) {
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
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                    #endregion
                });
            #endregion
            VirtualRoot.On<Per10SecondEvent>("周期刷新显卡状态", LogEnum.None,
                action: message => {
                    // 因为遇到显卡系统状态变更时可能费时
                    Task.Factory.StartNew(() => {
                        GpuSet.LoadGpuState();
                    });
                });
        }

        // 在Windows右键上下文菜单中添加“命令行”菜单
        private static void RegCmdHere() {
            string cmdHere = "SOFTWARE\\Classes\\Directory\\background\\shell\\cmd_here";
            string cmdHereCommand = cmdHere + "\\command";
            string cmdPrompt = "SOFTWARE\\Classes\\Folder\\shell\\cmdPrompt";
            string cmdPromptCommand = cmdPrompt + "\\command";
            Windows.WinRegistry.SetValue(Registry.LocalMachine, cmdHere, "", "命令行");
            Windows.WinRegistry.SetValue(Registry.LocalMachine, cmdHere, "Icon", "cmd.exe");
            Windows.WinRegistry.SetValue(Registry.LocalMachine, cmdHereCommand, "", "\"cmd.exe\"");
            Windows.WinRegistry.SetValue(Registry.LocalMachine, cmdPrompt, "", "命令行");
            Windows.WinRegistry.SetValue(Registry.LocalMachine, cmdPromptCommand, "", "\"cmd.exe\" \"cd %1\"");
            cmdHere = "SOFTWARE\\Classes\\Directory\\shell\\cmd_here";
            cmdHereCommand = cmdHere + "\\command";
            Windows.WinRegistry.SetValue(Registry.LocalMachine, cmdHere, "", "命令行");
            Windows.WinRegistry.SetValue(Registry.LocalMachine, cmdHere, "Icon", "cmd.exe");
            Windows.WinRegistry.SetValue(Registry.LocalMachine, cmdHereCommand, "", "\"cmd.exe\"");
        }

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
                for (int i = 1; i <= 10; i++) {
                    if (i % 2 == 0) {
                        Write.UserInfo($"显卡性能恢复中{i * 10}%");
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                Write.UserOk("显卡性能恢复完毕");
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
                    Task.Factory.StartNew(() => {
                        Windows.TaskKill.Kill(processName, waitForExit: true);
                        Logger.EventWriteLine("挖矿停止");
                    });
                }
                else {
                    Logger.EventWriteLine("挖矿停止");
                }
                var mineContext = _currentMineContext;
                _currentMineContext = null;
                VirtualRoot.Happened(new MineStopedEvent(mineContext));
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
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
                    Logger.EventWriteLine("正在重启内核");
                    if (isWork) {
                        ContextReInit(true);
                    }
                    StartMine();
                });
            }
            NTMinerRegistry.SetIsLastIsWork(isWork);
        }
        #endregion

        #region StartMine
        public void StartMine() {
            try {
                Logger.EventWriteLine("开始挖矿");
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
                    Logger.WarnWriteLine(kernel.GetFullName() + "本地内核包不存在，开始自动下载");
                    VirtualRoot.Execute(new ShowKernelDownloaderCommand(kernel.GetId(), downloadComplete: (isSuccess, message) => {
                        if (isSuccess) {
                            StartMine();
                        }
                        else {
                            VirtualRoot.Happened(new StartingMineFailedEvent("内核下载：" + message));
                        }
                    }));
                }
                else {
                    string commandLine = BuildAssembleArgs(out Dictionary<string, string> parameters, out Dictionary<Guid, string> fileWriters, out Dictionary<Guid, string> fragments);
                    if (IsUiVisible) {
                        if (commandLine != UserKernelCommandLine) {
                            Logger.WarnDebugLine("意外：MineContext.CommandLine和UserKernelCommandLine不等了");
                            Logger.WarnDebugLine("UserKernelCommandLine  :" + UserKernelCommandLine);
                            Logger.WarnDebugLine("MineContext.CommandLine:" + commandLine);
                        }
                    }
                    IMineContext mineContext = new MineContext(
                        this.MinerProfile.MinerName, mainCoin,
                        mainCoinPool, kernel, coinKernel,
                        coinProfile.Wallet, commandLine,
                        parameters, fragments, fileWriters);
                    if (coinKernelProfile.IsDualCoinEnabled) {
                        mineContext = new DualMineContext(
                            mineContext, dualCoin, dualCoinPool,
                            coinProfile.DualCoinWallet,
                            coinKernelProfile.DualCoinWeight,
                            parameters, fragments, fileWriters);
                    }
                    _currentMineContext = mineContext;
                    MinerProcess.CreateProcessAsync(mineContext);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
        #endregion
        
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

        public ICoinSet CoinSet { get; private set; }

        public IGroupSet GroupSet { get; private set; }

        public ICoinGroupSet CoinGroupSet { get; private set; }

        public ICalcConfigSet CalcConfigSet { get; private set; }

        #region GpuSetInfo
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
                    if (_gpuSetInfo.Length == 0) {
                        _gpuSetInfo = "无";
                    }
                }
                return _gpuSetInfo;
            }
        }
        #endregion

        #region GpuSet
        private static bool IsNCard {
            get {
                try {
                    foreach (ManagementBaseObject item in new ManagementObjectSearcher("SELECT Caption FROM Win32_VideoController").Get()) {
                        foreach (var property in item.Properties) {
                            if ((property.Value ?? string.Empty).ToString().Contains("NVIDIA")) {
                                return true;
                            }
                        }
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                return false;
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
        #endregion

        public ISysDicSet SysDicSet { get; private set; }

        public ISysDicItemSet SysDicItemSet { get; private set; }

        public IPoolSet PoolSet { get; private set; }

        public ICoinKernelSet CoinKernelSet { get; private set; }

        public IPoolKernelSet PoolKernelSet { get; private set; }

        public IKernelSet KernelSet { get; private set; }

        public IFileWriterSet FileWriterSet { get; private set; }

        public IFragmentWriterSet FragmentWriterSet { get; private set; }

        public IPackageSet PackageSet { get; private set; }

        public IKernelProfileSet KernelProfileSet { get; private set; }

        public IGpusSpeed GpusSpeed { get; private set; }

        public ICoinShareSet CoinShareSet { get; private set; }

        public IKernelInputSet KernelInputSet { get; private set; }

        public IKernelOutputSet KernelOutputSet { get; private set; }

        public IKernelOutputFilterSet KernelOutputFilterSet { get; private set; }

        public IKernelOutputTranslaterSet KernelOutputTranslaterSet { get; private set; }

        public IWorkerEventSet WorkerEventSet { get; private set; }

        public IKernelOutputKeywordSet KernelOutputKeywordSet { get; private set; }
    }
}
