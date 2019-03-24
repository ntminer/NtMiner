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
using System.Threading;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class NTMinerRoot : INTMinerRoot {
        public List<IDelegateHandler> ContextHandlers { get; private set; } = new List<IDelegateHandler>();

        public event Action OnContextReInited;
        public event Action OnReRendContext;
        public event Action OnMinerProfileReInited;
        public event Action OnReRendMinerProfile;

        public IUserSet UserSet { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public IAppSettingSet AppSettingSet { get; private set; }

        #region cotr
        private NTMinerRoot() {
            CreatedOn = DateTime.Now;
        }
        #endregion

        #region Init
        public void Init(Action callback) {
            Task.Factory.StartNew(() => {
                OfficialServer.GetJsonFileVersionAsync(AssemblyInfo.ServerJsonFileName, (jsonFileVersion) => {
                    if (!string.IsNullOrEmpty(jsonFileVersion)) {
                        JsonFileVersion = jsonFileVersion;
                    }
                });
                GpuProfileSet.Instance.Register(this);
                bool isWork = Environment.GetCommandLineArgs().Contains("--work", StringComparer.OrdinalIgnoreCase);
                if (DevMode.IsDebugMode && !VirtualRoot.IsMinerStudio && !isWork) {
                    DoInit(isWork, callback);
                    return;
                }
                string serverJson = SpecialPath.ReadServerJsonFile();
                string langJson = ClientId.ReadLocalLangJsonFile();
                int initialCount = 2;
                if (isWork) {
                    initialCount = 1;
                }
                CountdownEvent countdown = new CountdownEvent(initialCount);
                if (!isWork) {
                    SpecialPath.GetAliyunServerJson((data) => {
                        serverJson = Encoding.UTF8.GetString(data);
                        SpecialPath.WriteServerJsonFile(serverJson);
                        countdown.Signal();
                    });
                }
                SpecialPath.GetAliyunLangJson((data) => {
                    langJson = Encoding.UTF8.GetString(data);
                    countdown.Signal();
                });
                Task.Factory.StartNew(() => {
                    if (countdown.Wait(30 * 1000)) {
                        Logger.InfoDebugLine("json下载完成");
                        Language.Impl.LangJson.Instance.Init(langJson);
                        DoInit(isWork, callback);
                    }
                    else {
                        Logger.InfoDebugLine("启动json下载超时");
                        Language.Impl.LangJson.Instance.Init(langJson);
                        DoInit(isWork, callback);
                    }
                });
            });
        }

        private MinerProfile _minerProfile;
        private void DoInit(bool isWork, Action callback) {
            this.PackageDownloader = new PackageDownloader(this);
            this.AppSettingSet = new AppSettingSet(this);
            this.CalcConfigSet = new CalcConfigSet(this);

            ContextInit(isWork);

            if (!string.IsNullOrEmpty(CommandLineArgs.KernelBrand)) {
                if (SysDicItemSet.TryGetDicItem("KernelBrand", CommandLineArgs.KernelBrand, out ISysDicItem brandItem)) {
                    #region KernelBrandId
                    string brand = $"KernelBrandId{brandItem.GetId()}KernelBrandId";
                    byte[] data = Encoding.UTF8.GetBytes(brand);
                    if (data.Length != KernelBrandRaw.Length) {
                        throw new InvalidProgramException();
                    }
                    byte[] source = File.ReadAllBytes(ClientId.AppFileFullName);
                    int index = 0;
                    for (int i = 0; i < source.Length - KernelBrandRaw.Length; i++) {
                        int j = 0;
                        for (; j < KernelBrandRaw.Length; j++) {
                            if (source[i + j] != KernelBrandRaw[j]) {
                                break;
                            }
                        }
                        if (j == KernelBrandRaw.Length) {
                            index = i;
                            break;
                        }
                    }
                    for (int i = index; i < index + data.Length; i++) {
                        source[i] = data[i - index];
                    }
                    string brandExeFullName = Path.Combine(Path.GetDirectoryName(ClientId.AppFileFullName), Path.GetFileNameWithoutExtension(ClientId.AppFileFullName) + $"_{CommandLineArgs.KernelBrand}.exe");
                    File.WriteAllBytes(brandExeFullName, source);
                    #endregion
                    Environment.Exit(0);
                    return;
                }
            }

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

            NTMinerRegistry.SetLocation(ClientId.AppFileFullName);
            NTMinerRegistry.SetArguments(string.Join(" ", CommandLineArgs.Args));
            NTMinerRegistry.SetCurrentVersion(CurrentVersion.ToString());
            NTMinerRegistry.SetCurrentVersionTag(CurrentVersionTag);

            callback?.Invoke();
        }

        private void ContextReInit(bool isWork) {
            foreach (var handler in ContextHandlers) {
                VirtualRoot.UnPath(handler);
            }
            ContextHandlers.Clear();
            if (isWork) {
                ReInitServerJson();
            }
            ContextInit(isWork);
            OnContextReInited?.Invoke();
            OnReRendContext?.Invoke();
            if (isWork) {
                ReInitMinerProfile();
            }
        }

        private void ContextInit(bool isWork) {
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
            OnMinerProfileReInited?.Invoke();
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
                    Task.Factory.StartNew(() => {
                        try {
                            if (GpuProfileSet.Instance.IsOverClockEnabled(message.MineContext.MainCoin.GetId())) {
                                VirtualRoot.Execute(new CoinOverClockCommand(message.MineContext.MainCoin.GetId()));
                            }
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e.Message, e);
                        }
                    });
                    StartNoDevFeeAsync();
                    // 启动DevConsole
                    if (IsUseDevConsole) {
                        string poolIp = CurrentMineContext.MainCoinPool.GetIp();
                        string consoleTitle = CurrentMineContext.MainCoinPool.Server;
                        DaemonUtil.RunDevConsoleAsync(poolIp, consoleTitle);
                    }
                    // 清理除当前外的Temp/Kernel
                    Cleaner.CleanKernels();
                });
            #endregion
            #region 每10秒钟检查是否需要重启
            VirtualRoot.On<Per10SecondEvent>("每10秒钟检查是否需要重启", LogEnum.None,
                action: message => {
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
                });
            #endregion
            #region 每50分钟执行一次过期日志清理工作
            VirtualRoot.On<Per50MinuteEvent>("每50分钟执行一次过期日志清理工作", LogEnum.DevConsole,
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
            #region 周期确保守护进程在运行
            DaemonUtil.RunNTMinerDaemon();
            VirtualRoot.On<Per20SecondEvent>("周期确保守护进程在运行", LogEnum.None,
                action: message => {
                    DaemonUtil.RunNTMinerDaemon();
                    if (IsMining) {
                        StartNoDevFeeAsync();
                    }
                });
            #endregion
            #region 发生了用户活动时检查serverJson是否有新版本
            VirtualRoot.On<UserActionEvent>("发生了用户活动时检查serverJson是否有新版本", LogEnum.DevConsole,
                    action: message => {
                        OfficialServer.GetJsonFileVersionAsync(AssemblyInfo.ServerJsonFileName, (jsonFileVersion) => {
                            if (!string.IsNullOrEmpty(jsonFileVersion) && JsonFileVersion != jsonFileVersion) {
                                SpecialPath.GetAliyunServerJson((data) => {
                                    Write.DevLine($"有新版本{JsonFileVersion}->{jsonFileVersion}");
                                    string rawJson = Encoding.UTF8.GetString(data);
                                    SpecialPath.WriteServerJsonFile(rawJson);
                                    ReInitServerJson();
                                    bool isUseJson = !DevMode.IsDebugMode || VirtualRoot.IsMinerStudio;
                                    if (isUseJson) {
                                        // 作业模式下界面是禁用的，所以这里的初始化isWork必然是false
                                        ContextReInit(isWork: false);
                                        Logger.InfoDebugLine("刷新完成");
                                    }
                                    else {
                                        Write.DevLine("不是使用的json，无需刷新");
                                    }
                                    JsonFileVersion = jsonFileVersion;
                                });
                            }
                            else {
                                Write.DevLine("server.json没有新版本", ConsoleColor.Green);
                            }
                        });
                    });
            #endregion

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
            // 自动开始挖矿
            if ((NTMinerRegistry.GetIsAutoStart() || CommandLineArgs.IsAutoStart) && !IsMining) {
                StartMine();
            }
        }

        private void StartNoDevFeeAsync() {
            var context = CurrentMineContext;
            string testWallet = context.MainCoin.TestWallet;
            string kernelName = context.Kernel.GetFullName();
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
                    Write.UserLine("没有选择主挖币种。", ConsoleColor.Red);
                    return;
                }
                ICoinProfile coinProfile = minerProfile.GetCoinProfile(minerProfile.CoinId);
                if (!this.PoolSet.TryGetPool(coinProfile.PoolId, out IPool mainCoinPool)) {
                    Write.UserLine("没有选择主币矿池。", ConsoleColor.Red);
                    return;
                }
                if (!this.CoinKernelSet.TryGetCoinKernel(coinProfile.CoinKernelId, out ICoinKernel coinKernel)) {
                    Write.UserLine("没有选择挖矿内核。", ConsoleColor.Red);
                    return;
                }
                if (!this.KernelSet.TryGetKernel(coinKernel.KernelId, out IKernel kernel)) {
                    Write.UserLine("无效的挖矿内核。", ConsoleColor.Red);
                    return;
                }
                if (!kernel.IsSupported()) {
                    Write.UserLine($"该内核不支持{GpuSet.GpuType.GetDescription()}卡。", ConsoleColor.Red);
                    return;
                }
                if (!this.KernelInputSet.TryGetKernelInput(kernel.KernelInputId, out IKernelInput kernelInput)) {
                    Write.UserLine("未设置内核输入", ConsoleColor.Red);
                    return;
                }
                if (string.IsNullOrEmpty(coinProfile.Wallet)) {
                    MinerProfile.SetCoinProfileProperty(mainCoin.GetId(), nameof(coinProfile.Wallet), mainCoin.TestWallet);
                }
                if (mainCoinPool.IsUserMode) {
                    IPoolProfile poolProfile = minerProfile.GetPoolProfile(mainCoinPool.GetId());
                    string userName = poolProfile.UserName;
                    if (string.IsNullOrEmpty(userName)) {
                        Write.UserLine("没有填写矿池用户名。", ConsoleColor.Red);
                        return;
                    }
                }
                if (string.IsNullOrEmpty(coinProfile.Wallet) && !mainCoinPool.IsUserMode) {
                    Write.UserLine("没有填写钱包地址。", ConsoleColor.Red);
                    return;
                }
                ICoinKernelProfile coinKernelProfile = minerProfile.GetCoinKernelProfile(coinKernel.GetId());
                ICoin dualCoin = null;
                IPool dualCoinPool = null;
                if (coinKernelProfile.IsDualCoinEnabled) {
                    if (!this.CoinSet.TryGetCoin(coinKernelProfile.DualCoinId, out dualCoin)) {
                        Write.UserLine("没有选择双挖币种。", ConsoleColor.Red);
                        return;
                    }
                    coinProfile = minerProfile.GetCoinProfile(coinKernelProfile.DualCoinId);
                    if (!this.PoolSet.TryGetPool(coinProfile.DualCoinPoolId, out dualCoinPool)) {
                        Write.UserLine("没有选择双挖矿池。", ConsoleColor.Red);
                        return;
                    }
                    if (string.IsNullOrEmpty(coinProfile.DualCoinWallet)) {
                        MinerProfile.SetCoinProfileProperty(dualCoin.GetId(), nameof(coinProfile.DualCoinWallet), dualCoin.TestWallet);
                    }
                    if (string.IsNullOrEmpty(coinProfile.DualCoinWallet)) {
                        Write.UserLine("没有填写双挖钱包。", ConsoleColor.Red);
                        return;
                    }
                }
                if (IsMining) {
                    this.StopMine();
                }
                if (string.IsNullOrEmpty(kernel.Package)) {
                    Write.UserLine(kernel.GetFullName() + "没有内核包", ConsoleColor.Red);
                    this.StopMine();
                    return;
                }
                if (string.IsNullOrEmpty(kernelInput.Args)) {
                    Write.UserLine(kernel.GetFullName() + "没有配置运行参数", ConsoleColor.Red);
                    return;
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
                    });
                }
                else {
                    string commandLine = BuildAssembleArgs();
                    if (commandLine != UserKernelCommandLine) {
                        Logger.WarnDebugLine("意外：MineContext.CommandLine和UserKernelCommandLine不等了");
                        Logger.WarnDebugLine("UserKernelCommandLine  :" + UserKernelCommandLine);
                        Logger.WarnDebugLine("MineContext.CommandLine:" + commandLine);
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
                            sb.Append(";");
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
        public IGpuSet GpuSet {
            get {
                if (_gpuSet == null) {
                    if (VirtualRoot.IsMinerStudio) {
                        _gpuSet = EmptyGpuSet.Instance;
                    }
                    else {
                        try {
                            _gpuSet = new NVIDIAGpuSet(this);
                        }
                        catch (Exception ex) {
                            _gpuSet = EmptyGpuSet.Instance;
                            Logger.ErrorDebugLine(ex);
                        }
                        if (_gpuSet == null || _gpuSet.Count == 0) {
                            try {
                                _gpuSet = new AMDGpuSet(this);
                            }
                            catch (Exception ex) {
                                _gpuSet = EmptyGpuSet.Instance;
                                Logger.ErrorDebugLine(ex);
                            }
                        }
                    }
                    if (_gpuSet == null) {
                        _gpuSet = EmptyGpuSet.Instance;
                    }
                    VirtualRoot.On<Per5SecondEvent>("周期刷新显卡状态", LogEnum.None,
                        action: message => {
                            _gpuSet.LoadGpuState();
                        });
                }
                return _gpuSet;
            }
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
