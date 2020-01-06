using Microsoft.Win32;
using NTMiner.AppSetting;
using NTMiner.Core;
using NTMiner.Core.Cpus;
using NTMiner.Core.Cpus.Impl;
using NTMiner.Core.Gpus;
using NTMiner.Core.Gpus.Impl;
using NTMiner.Core.Impl;
using NTMiner.Core.Kernels;
using NTMiner.Core.MinerServer;
using NTMiner.Core.MinerServer.Impl;
using NTMiner.Core.Profiles;
using NTMiner.Core.Profiles.Impl;
using NTMiner.KernelOutputKeyword;
using NTMiner.Profile;
using NTMiner.ServerMessage;
using NTMiner.User;
using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class NTMinerRoot : INTMinerRoot {
        public IUserSet UserSet { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public IAppSettingSet ServerAppSettingSet { get; private set; }

        private NTMinerRoot() {
            CreatedOn = DateTime.Now;
            if (VirtualRoot.IsMinerClient) {
                SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            }
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e) {
            VirtualRoot.ThisLocalInfo(nameof(NTMinerRoot), $"Windows SessionSwitch, Reason:{e.Reason}");
        }

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
                    bool useLocalDb = DevMode.IsDevMode && !VirtualRoot.IsMinerStudio;
                    if (useLocalDb) {
                        DoInit(isWork: false, callback: callback);
                    }
                    else {
                        Logger.InfoDebugLine(nameof(GetAliyunServerJson));
                        GetAliyunServerJson((data) => {
                            // 如果server.json未下载成功则不覆写本地server.json
                            if (data != null && data.Length != 0) {
                                Logger.InfoDebugLine($"{nameof(GetAliyunServerJson)}成功");
                                var serverJson = Encoding.UTF8.GetString(data);
                                if (!string.IsNullOrEmpty(serverJson)) {
                                    SpecialPath.WriteServerJsonFile(serverJson);
                                }
                                OfficialServer.GetJsonFileVersionAsync(EntryAssemblyInfo.ServerJsonFileName, serverState => {
                                    SetServerJsonVersion(serverState.JsonFileVersion);
                                    AppVersionChangedEvent.PublishIfNewVersion(serverState.MinerClientVersion);
                                    if (Math.Abs((long)Timestamp.GetTimestamp() - (long)serverState.Time) < Timestamp.DesyncSeconds) {
                                        Logger.OkDebugLine($"本机和服务器时间一致或相差不超过 {Timestamp.DesyncSeconds.ToString()} 秒");
                                    }
                                    else {
                                        Write.UserWarn($"本机和服务器时间不同步，请调整，本地：{DateTime.Now.ToString()}，服务器：{Timestamp.FromTimestamp(serverState.Time).ToString()}。此问题不影响挖矿。");
                                    }
                                });
                            }
                            else {
                                if (!File.Exists(SpecialPath.ServerJsonFileFullName)) {
                                    VirtualRoot.ThisLocalError(nameof(NTMinerRoot), "配置文件下载失败，这是第一次运行开源矿工，配置文件至少需要成功下载一次，请检查网络是否可用", OutEnum.Warn);
                                }
                                else {
                                    VirtualRoot.ThisLocalWarn(nameof(NTMinerRoot), "配置文件下载失败，使用最近一次成功下载的配置文件", OutEnum.Warn);
                                }
                            }
                            DoInit(isWork, callback);
                        });
                        #region 发生了用户活动时检查serverJson是否有新版本
                        VirtualRoot.AddEventPath<UserActionEvent>("发生了用户活动时检查serverJson是否有新版本", LogEnum.DevConsole,
                            action: message => {
                                RefreshServerJsonFile();
                            }, location: this.GetType());
                        #endregion
                    }
                }
                VirtualRoot.ThisLocalInfo(nameof(NTMinerRoot), $"启动{VirtualRoot.AppName}");
            });
        }

        private MinerProfile _minerProfile;
        private void DoInit(bool isWork, Action callback) {
            IsJsonServer = !DevMode.IsDevMode || VirtualRoot.IsMinerStudio || isWork;
            this.ReporterDataProvider = new ReportDataProvider();
            this.ServerAppSettingSet = new ServerAppSettingSet();
            this.CalcConfigSet = new CalcConfigSet(this);
            this.ServerContext = new ServerContext();
            this.GpuProfileSet = new GpuProfileSet(this);
            this.UserSet = new UserSet();
            this.KernelProfileSet = new KernelProfileSet(this);
            this.GpusSpeed = new GpusSpeed(this);
            this.CoinShareSet = new CoinShareSet(this);
            this.MineWorkSet = new MineWorkSet();
            this.MinerGroupSet = new MinerGroupSet();
            this.NTMinerWalletSet = new NTMinerWalletSet();
            this.OverClockDataSet = new OverClockDataSet(this);
            this.ColumnsShowSet = new ColumnsShowSet();
            this.ServerMessageSet = new ServerMessageSet(VirtualRoot.LocalDbFileFullName, isServer: false);
            // 作业和在群控客户端管理作业时
            IsJsonLocal = isWork || VirtualRoot.IsMinerStudio;
            this._minerProfile = new MinerProfile(this);
            var cpuPackage = new CpuPackage(_minerProfile);
            this.CpuPackage = cpuPackage;

            // 这几个注册表内部区分挖矿端和群控客户端
            NTMinerRegistry.SetLocation(VirtualRoot.AppFileFullName);
            NTMinerRegistry.SetArguments(string.Join(" ", CommandLineArgs.Args));
            NTMinerRegistry.SetCurrentVersion(EntryAssemblyInfo.CurrentVersion.ToString());
            NTMinerRegistry.SetCurrentVersionTag(EntryAssemblyInfo.CurrentVersionTag);

            if (VirtualRoot.IsMinerClient) {
                VirtualRoot.LocalIpSet.InitOnece();
                Link();
                // 当显卡温度变更时守卫温度防线
                TempGruarder.Instance.Init(this);
                // 因为这里耗时500毫秒左右
                Task.Factory.StartNew(() => {
                    Windows.Error.DisableWindowsErrorUI();
                    Windows.UAC.DisableUAC();
                    Windows.WAU.DisableWAUAsync();
                    Windows.Defender.DisableAntiSpyware();
                    Windows.Power.PowerCfgOff();
                    Windows.BcdEdit.IgnoreAllFailures();
                });
            }

            callback?.Invoke();
            cpuPackage.Init();
        }

        // MinerProfile对应local.litedb或local.json
        // 群控客户端管理作业时调用该方法切换MinerProfile上下文
        public void ReInitMinerProfile() {
            ReInitLocalJson();
            this._minerProfile.ReInit(this);
            // 本地数据集已刷新，此时刷新本地数据集的视图模型集
            VirtualRoot.RaiseEvent(new LocalContextReInitedEvent());
            // 本地数据集的视图模型已刷新，此时刷新本地数据集的视图界面
            VirtualRoot.RaiseEvent(new LocalContextVmsReInitedEvent());
            RefreshArgsAssembly();
        }

        private void RefreshServerJsonFile() {
            OfficialServer.GetJsonFileVersionAsync(EntryAssemblyInfo.ServerJsonFileName, serverState => {
                AppVersionChangedEvent.PublishIfNewVersion(serverState.MinerClientVersion);
                string localServerJsonFileVersion = GetServerJsonVersion();
                if (!string.IsNullOrEmpty(serverState.JsonFileVersion) && localServerJsonFileVersion != serverState.JsonFileVersion) {
                    GetAliyunServerJson((data) => {
                        string rawJson = Encoding.UTF8.GetString(data);
                        SpecialPath.WriteServerJsonFile(rawJson);
                        SetServerJsonVersion(serverState.JsonFileVersion);
                        // 作业模式下界面是禁用的，所以这里的初始化isWork必然是false
                        ContextReInit(isWork: VirtualRoot.IsMinerStudio);
                        VirtualRoot.ThisLocalInfo(nameof(NTMinerRoot), $"刷新server.json配置", toConsole: true);
                    });
                }
                else {
                    Write.DevDebug("server.json没有新版本");
                }
            });
        }

        #endregion

        public string GetServerJsonVersion() {
            string serverJsonVersion = string.Empty;
            if (VirtualRoot.LocalAppSettingSet.TryGetAppSetting(NTKeyword.ServerJsonVersionAppSettingKey, out IAppSetting setting) && setting.Value != null) {
                serverJsonVersion = setting.Value.ToString();
            }
            return serverJsonVersion;
        }

        private void SetServerJsonVersion(string serverJsonVersion) {
            AppSettingData appSettingData = new AppSettingData() {
                Key = NTKeyword.ServerJsonVersionAppSettingKey,
                Value = serverJsonVersion
            };
            string oldVersion = GetServerJsonVersion();
            VirtualRoot.Execute(new SetLocalAppSettingCommand(appSettingData));
            VirtualRoot.RaiseEvent(new ServerJsonVersionChangedEvent(oldVersion, serverJsonVersion));
        }

        #region private methods

        private void ContextReInit(bool isWork) {
            ReInitServerJson();
            IsJsonServer = !DevMode.IsDevMode || VirtualRoot.IsMinerStudio || isWork;
            this.ServerContext.ReInit();
            // CoreContext的视图模型集此时刷新
            VirtualRoot.RaiseEvent(new ServerContextReInitedEvent());
            // CoreContext的视图模型集已全部刷新，此时刷新视图界面
            VirtualRoot.RaiseEvent(new ServerContextVmsReInitedEvent());
            if (isWork) {
                // 有可能是由非作业切换为作业，所以需要对IsJsonLocal赋值
                IsJsonLocal = true;
                ReInitMinerProfile();
            }
        }

        private void Link() {
            VirtualRoot.AddCmdPath<RegCmdHereCommand>(action: message => {
                try {
                    Windows.Cmd.RegCmdHere(); 
                    VirtualRoot.ThisLocalInfo(nameof(NTMinerRoot), "添加windows右键命令行成功", OutEnum.Success);
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    VirtualRoot.ThisLocalError(nameof(NTMinerRoot), "添加windows右键命令行失败", OutEnum.Warn);
                }
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<UnRegCmdHereCommand>(action: message => {
                try {
                    Windows.Cmd.UnRegCmdHere();
                    VirtualRoot.ThisLocalInfo(nameof(NTMinerRoot), "移除windows右键命令行成功", OutEnum.Success);
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    VirtualRoot.ThisLocalError(nameof(NTMinerRoot), "移除windows右键命令行失败", OutEnum.Warn);
                }
            }, location: this.GetType());
            VirtualRoot.AddEventPath<Per1MinuteEvent>("每1分钟阻止系统休眠", LogEnum.None,
                action: message => {
                    Windows.Power.PreventSleep();
                }, location: this.GetType());
            #region 挖矿开始时将无份额内核重启份额计数置0
            int shareCount = 0;
            DateTime shareOn = DateTime.Now;
            VirtualRoot.AddEventPath<MineStartedEvent>("挖矿开始后将无份额内核重启份额计数置0", LogEnum.DevConsole,
                action: message => {
                    // 将无份额内核重启份额计数置0
                    shareCount = 0;
                    if (!message.MineContext.IsRestart) {
                        // 当不是内核重启时更新shareOn，如果是内核重启不用更新shareOn从而给不干扰无内核矿机重启的逻辑
                        shareOn = DateTime.Now;
                    }
                }, location: this.GetType());
            #endregion
            #region 每20秒钟检查是否需要重启
            VirtualRoot.AddEventPath<Per20SecondEvent>("每20秒钟检查是否需要重启", LogEnum.None,
                action: message => {
                    #region 重启电脑
                    try {
                        if (MinerProfile.IsPeriodicRestartComputer) {
                            if ((DateTime.Now - this.CreatedOn).TotalMinutes > 60 * MinerProfile.PeriodicRestartComputerHours + MinerProfile.PeriodicRestartComputerMinutes) {
                                string content = $"每运行{MinerProfile.PeriodicRestartKernelHours.ToString()}小时{MinerProfile.PeriodicRestartComputerMinutes.ToString()}分钟重启电脑";
                                VirtualRoot.ThisLocalWarn(nameof(NTMinerRoot), content, toConsole: true);
                                Windows.Power.Restart(60);
                                VirtualRoot.Execute(new CloseNTMinerCommand(content));
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
                        if (IsMining && MinerProfile.IsPeriodicRestartKernel && LockedMineContext.MineStartedOn != DateTime.MinValue) {
                            if ((DateTime.Now - LockedMineContext.MineStartedOn).TotalMinutes > 60 * MinerProfile.PeriodicRestartKernelHours + MinerProfile.PeriodicRestartKernelMinutes) {
                                VirtualRoot.ThisLocalWarn(nameof(NTMinerRoot), $"每运行{MinerProfile.PeriodicRestartKernelHours.ToString()}小时{MinerProfile.PeriodicRestartKernelMinutes.ToString()}分钟重启内核", toConsole: true);
                                RestartMine();
                                return;// 退出
                            }
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                    #endregion

                    #region 无份额重启内核
                    try {
                        if (IsMining && this.LockedMineContext.MainCoin != null) {
                            int totalShare = 0;
                            bool restartComputer = MinerProfile.IsNoShareRestartComputer && (DateTime.Now - shareOn).TotalMinutes > MinerProfile.NoShareRestartComputerMinutes;
                            bool restartKernel = MinerProfile.IsNoShareRestartKernel && (DateTime.Now - shareOn).TotalMinutes > MinerProfile.NoShareRestartKernelMinutes;
                            if (restartComputer || restartKernel) {
                                ICoinShare mainCoinShare = this.CoinShareSet.GetOrCreate(this.LockedMineContext.MainCoin.GetId());
                                totalShare = mainCoinShare.TotalShareCount;
                                if ((this.LockedMineContext is IDualMineContext dualMineContext) && dualMineContext.DualCoin != null) {
                                    ICoinShare dualCoinShare = this.CoinShareSet.GetOrCreate(dualMineContext.DualCoin.GetId());
                                    totalShare += dualCoinShare.TotalShareCount;
                                }
                                // 如果份额没有增加
                                if (shareCount == totalShare) {
                                    if (restartComputer) {
                                        if (!MinerProfile.IsAutoBoot || !MinerProfile.IsAutoStart) {
                                            VirtualRoot.Execute(new SetAutoStartCommand(true, true));
                                        }
                                        string content = $"{MinerProfile.NoShareRestartComputerMinutes.ToString()}分钟无份额重启电脑";
                                        VirtualRoot.ThisLocalWarn(nameof(NTMinerRoot), content, toConsole: true);
                                        Windows.Power.Restart(60);
                                        VirtualRoot.Execute(new CloseNTMinerCommand(content));
                                        return;// 退出
                                    }
                                    // 产生过份额或者已经两倍重启内核时间了
                                    if (restartKernel && (totalShare > 0 || (DateTime.Now - shareOn).TotalMinutes > 2 * MinerProfile.NoShareRestartKernelMinutes)) {
                                        VirtualRoot.ThisLocalWarn(nameof(NTMinerRoot), $"{MinerProfile.NoShareRestartKernelMinutes.ToString()}分钟无份额重启内核", toConsole: true);
                                        RestartMine();
                                        return;// 退出
                                    }
                                }
                                if (totalShare > shareCount) {
                                    shareCount = totalShare;
                                    shareOn = DateTime.Now;
                                }
                            }
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                    #endregion
                }, location: this.GetType());
            #endregion
            VirtualRoot.AddEventPath<Per10SecondEvent>("周期刷新显卡状态", LogEnum.None,
                action: message => {
                    // 因为遇到显卡系统状态变更时可能费时
                    Task.Factory.StartNew(() => {
                        GpuSet.LoadGpuState();
                    });
                }, location: this.GetType());
        }
        #endregion

        #region Exit
        public void Exit() {
            if (LockedMineContext != null) {
                StopMine(StopMineReason.ApplicationExit);
            }
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
            _computer?.Close();
        }
        #endregion

        public StopMineReason StopReason { get; private set; }
        #region StopMine
        public void StopMineAsync(StopMineReason stopReason, Action callback = null) {
            if (!IsMining) {
                callback?.Invoke();
                return;
            }
            Task.Factory.StartNew(() => {
                StopMine(stopReason);
                callback?.Invoke();
            });
        }
        private void StopMine(StopMineReason stopReason) {
            this.StopReason = stopReason;
            LockedMineContext?.Close();
            if (!IsMining) {
                return;
            }
            try {
                var mineContext = LockedMineContext;
                LockedMineContext = null;
                VirtualRoot.ThisLocalWarn(nameof(NTMinerRoot), $"挖矿停止。原因：{stopReason.GetDescription()}", toConsole: true);
                VirtualRoot.RaiseEvent(new MineStopedEvent(mineContext, stopReason));
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
        #endregion

        #region RestartMine
        public void RestartMine(bool isWork = false) {
            if (isWork) {
                ContextReInit(true);
            }
            StartMine(isRestart: true);
            NTMinerRegistry.SetIsLastIsWork(isWork);
        }
        #endregion

        private bool GetProfileData(
            out ICoin mainCoin, out ICoinProfile mainCoinProfile, out IPool mainCoinPool, out ICoinKernel mainCoinKernel, out IKernel kernel, 
            out IKernelInput kernelInput, out IKernelOutput kernelOutput, out string errorMsg) {
            mainCoinProfile = null;
            mainCoinPool = null;
            mainCoinKernel = null;
            kernel = null;
            kernelInput = null;
            kernelOutput = null;
            errorMsg = string.Empty;
            IWorkProfile minerProfile = this.MinerProfile;
            if (!ServerContext.CoinSet.TryGetCoin(minerProfile.CoinId, out mainCoin)) {
                errorMsg = "没有选择主挖币种。";
                return false;
            }
            mainCoinProfile = minerProfile.GetCoinProfile(minerProfile.CoinId);
            if (!ServerContext.PoolSet.TryGetPool(mainCoinProfile.PoolId, out mainCoinPool)) {
                errorMsg = "没有选择主币矿池。";
                return false;
            }
            if (!ServerContext.CoinKernelSet.TryGetCoinKernel(mainCoinProfile.CoinKernelId, out mainCoinKernel)) {
                errorMsg = "没有选择挖矿内核。";
                return false;
            }
            if (!ServerContext.KernelSet.TryGetKernel(mainCoinKernel.KernelId, out kernel)) {
                errorMsg = "无效的挖矿内核。";
                return false;
            }
            if (!ServerContext.KernelInputSet.TryGetKernelInput(kernel.KernelInputId, out kernelInput)) {
                errorMsg = "未设置内核输入。";
                return false;
            }
            if (!ServerContext.KernelOutputSet.TryGetKernelOutput(kernel.KernelOutputId, out kernelOutput)) {
                errorMsg = "未设置内核输出。";
                return false;
            }
            return true;
        }

        #region StartMine
        public void StartMine(bool isRestart = false) {
            try {
                if (IsMining && isRestart) {
                    if (LockedMineContext.IsClosed) {
                        throw new InvalidProgramException();
                    }
                    LockedMineContext.Start(isRestart: true);
                    VirtualRoot.ThisLocalInfo(nameof(NTMinerRoot), "重启内核", toConsole: true);
                }
                else {
                    LockedMineContext?.Close();
                    if (IsMining) {
                        // 如果已经处在挖矿中了又点击了开始挖矿按钮（通常发生在群控端）
                        this.StopMine(StopMineReason.InStartMine);
                    }
                    IWorkProfile minerProfile = this.MinerProfile;
                    if (!GetProfileData(out ICoin mainCoin, out ICoinProfile mainCoinProfile, out IPool mainCoinPool, out ICoinKernel mainCoinKernel,
                        out IKernel kernel, out IKernelInput kernelInput, out IKernelOutput kernelOutput, out string errorMsg)) {
                        VirtualRoot.RaiseEvent(new StartingMineFailedEvent(errorMsg));
                        return;
                    }
                    if (!kernel.IsSupported(mainCoin)) {
                        VirtualRoot.RaiseEvent(new StartingMineFailedEvent($"该内核不支持{GpuSet.GpuType.GetDescription()}卡。"));
                        return;
                    }
                    if (string.IsNullOrEmpty(mainCoinProfile.Wallet)) {
                        MinerProfile.SetCoinProfileProperty(mainCoin.GetId(), nameof(mainCoinProfile.Wallet), mainCoin.TestWallet);
                    }
                    if (mainCoinPool.IsUserMode) {
                        IPoolProfile poolProfile = minerProfile.GetPoolProfile(mainCoinPool.GetId());
                        string userName = poolProfile.UserName;
                        if (string.IsNullOrEmpty(userName)) {
                            VirtualRoot.RaiseEvent(new StartingMineFailedEvent("没有填写矿池用户名。"));
                            return;
                        }
                    }
                    if (string.IsNullOrEmpty(mainCoinProfile.Wallet) && !mainCoinPool.IsUserMode) {
                        VirtualRoot.RaiseEvent(new StartingMineFailedEvent("没有填写钱包地址。"));
                        return;
                    }
                    ICoinKernelProfile coinKernelProfile = minerProfile.GetCoinKernelProfile(mainCoinKernel.GetId());
                    ICoin dualCoin = null;
                    IPool dualCoinPool = null;
                    ICoinProfile dualCoinProfile = null;
                    if (coinKernelProfile.IsDualCoinEnabled) {
                        if (!ServerContext.CoinSet.TryGetCoin(coinKernelProfile.DualCoinId, out dualCoin)) {
                            VirtualRoot.RaiseEvent(new StartingMineFailedEvent("没有选择双挖币种。"));
                            return;
                        }
                        dualCoinProfile = minerProfile.GetCoinProfile(coinKernelProfile.DualCoinId);
                        if (!ServerContext.PoolSet.TryGetPool(dualCoinProfile.DualCoinPoolId, out dualCoinPool)) {
                            VirtualRoot.RaiseEvent(new StartingMineFailedEvent("没有选择双挖矿池。"));
                            return;
                        }
                        if (string.IsNullOrEmpty(dualCoinProfile.DualCoinWallet)) {
                            MinerProfile.SetCoinProfileProperty(dualCoin.GetId(), nameof(dualCoinProfile.DualCoinWallet), dualCoin.TestWallet);
                        }
                        if (string.IsNullOrEmpty(dualCoinProfile.DualCoinWallet)) {
                            VirtualRoot.RaiseEvent(new StartingMineFailedEvent("没有填写双挖钱包。"));
                            return;
                        }
                    }
                    if (string.IsNullOrEmpty(kernel.Package)) {
                        VirtualRoot.RaiseEvent(new StartingMineFailedEvent(kernel.GetFullName() + "没有内核包"));
                        return;
                    }
                    if (string.IsNullOrEmpty(kernelInput.Args)) {
                        VirtualRoot.RaiseEvent(new StartingMineFailedEvent("没有配置运行参数。"));
                        return;
                    }
                    string packageZipFileFullName = Path.Combine(SpecialPath.PackagesDirFullName, kernel.Package);
                    if (!File.Exists(packageZipFileFullName)) {
                        VirtualRoot.ThisLocalInfo(nameof(NTMinerRoot), kernel.GetFullName() + "本地内核包不存在，开始自动下载", toConsole: true);
                        VirtualRoot.Execute(new ShowKernelDownloaderCommand(kernel.GetId(), downloadComplete: (isSuccess, message) => {
                            if (isSuccess) {
                                StartMine(isRestart);
                            }
                            else {
                                VirtualRoot.RaiseEvent(new StartingMineFailedEvent("内核下载：" + message));
                            }
                        }));
                    }
                    else {
                        LockedMineContext = CreateMineContext();
                        if (CurrentMineContext == null) {
                            CurrentMineContext = LockedMineContext;
                        }
                        if (LockedMineContext == null) {
                            return;
                        }
                        LockedMineContext.Start(isRestart: false);
                        VirtualRoot.ThisLocalInfo(nameof(NTMinerRoot), "开始挖矿", toConsole: true);
                        if (LockedMineContext.UseDevices.Length != GpuSet.Count) {
                            VirtualRoot.ThisLocalWarn(nameof(NTMinerRoot), "未启用全部显卡挖矿", toConsole: true);
                        }
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
        #endregion

        private IMineContext _currentMineContext;
        public IMineContext CurrentMineContext {
            get { return _currentMineContext; }
            set {
                if (_currentMineContext != value) {
                    _currentMineContext = value;
                    VirtualRoot.RaiseEvent(new CurrentMineContextChangedEvent());
                }
            }
        }

        private IMineContext _lockedMineContext;
        public IMineContext LockedMineContext {
            get {
                return _lockedMineContext;
            }
            private set {
                if (_lockedMineContext != value) {
                    _currentMineContext?.Close();
                    _lockedMineContext = value;
                }
            }
        }

        public bool IsMining {
            get {
                return LockedMineContext != null;
            }
        }

        public IReportDataProvider ReporterDataProvider { get; private set; }

        public IServerContext ServerContext { get; private set; }

        public IGpuProfileSet GpuProfileSet { get; private set; }

        public IWorkProfile MinerProfile {
            get { return _minerProfile; }
        }

        public IMineWorkSet MineWorkSet { get; private set; }

        public IMinerGroupSet MinerGroupSet { get; private set; }

        public INTMinerWalletSet NTMinerWalletSet { get; private set; }

        public IColumnsShowSet ColumnsShowSet { get; private set; }

        public IOverClockDataSet OverClockDataSet { get; private set; }

        public ICalcConfigSet CalcConfigSet { get; private set; }

        #region GpuSetInfo
        private string _gpuSetInfo = null;
        public string GpuSetInfo {
            get {
                if (_gpuSetInfo == null) {
                    StringBuilder sb = new StringBuilder();
                    int len = sb.Length;
                    foreach (var g in GpuSet.AsEnumerable().Where(a => a.Index != GpuAllId).GroupBy(a => a.Name)) {
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
                    using (var mos = new ManagementObjectSearcher("SELECT Caption FROM Win32_VideoController")) {
                        foreach (ManagementBaseObject item in mos.Get()) {
                            foreach (var property in item.Properties) {
                                if ((property.Value ?? string.Empty).ToString().Contains("NVIDIA")) {
                                    return true;
                                }
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
        private readonly object _gpuSetLocker = new object();
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
                                        _gpuSet = new AMDGpuSet();
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

        public ICpuPackage CpuPackage { get; private set; }

        public IKernelProfileSet KernelProfileSet { get; private set; }

        public IGpusSpeed GpusSpeed { get; private set; }

        public ICoinShareSet CoinShareSet { get; private set; }

        private IKernelOutputKeywordSet _kernelOutputKeywordSet;
        public IKernelOutputKeywordSet KernelOutputKeywordSet {
            get {
                if (_kernelOutputKeywordSet == null) {
                    _kernelOutputKeywordSet = new KernelOutputKeywordSet(VirtualRoot.LocalDbFileFullName, isServer: false);
                }
                return _kernelOutputKeywordSet;
            }
        }

        public IServerMessageSet ServerMessageSet { get; private set; }
    }
}
