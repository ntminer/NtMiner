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
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NTMiner {
    public partial class NTMinerRoot : INTMinerRoot {
        public List<IDelegateHandler> ContextHandlers { get; private set; } = new List<IDelegateHandler>();

        public event Action OnContextReInited;
        public event Action OnReRendContext;

        public DateTime CreatedOn { get; private set; }

        public IAppSettingSet AppSettingSet { get; private set; }

        #region cotr
        private NTMinerRoot() {
            CreatedOn = DateTime.Now;
        }
        #endregion
        public void GetJsonFileVersionAsync(string key, Action<string> callback) {
            Task.Factory.StartNew(() => {
                try {
                    AppSettingRequest request = new AppSettingRequest {
                        MessageId = Guid.NewGuid(),
                        Key = key
                    };
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message =
                            client.PostAsJsonAsync($"http://server.ntminer.com:3339/api/AppSetting/AppSetting", request);
                        DataResponse<AppSettingData> response =
                            message.Result.Content.ReadAsAsync<DataResponse<AppSettingData>>().Result;
                        string jsonFileVersion = string.Empty;
                        if (response.IsSuccess() && response.Data != null && response.Data.Value != null) {
                            if (response.Data.Value is string value) {
                                jsonFileVersion = value;
                            }
                        }

                        callback?.Invoke(jsonFileVersion);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine($"GetJsonFileVersionAsync({AssemblyInfo.ServerJsonFileName})失败 {e?.Message}");
                    callback?.Invoke(string.Empty);
                }
            });
        }

        private readonly bool _isServerJson = !DevMode.IsDebugMode || VirtualRoot.IsControlCenter || CommandLineArgs.WorkId != Guid.Empty;
        #region Init
        public void Init(Action callback) {
            Task.Factory.StartNew(() => {
                if (!_isServerJson) {
                    DoInit(callback);
                    return;
                }

                GetJsonFileVersionAsync(AssemblyInfo.ServerJsonFileName, (jsonFileVersion) => {
                    if (!string.IsNullOrEmpty(jsonFileVersion)) {
                        JsonFileVersion = jsonFileVersion;
                    }
                });
                string serverJson = SpecialPath.ReadServerJsonFile();
                string langJson = ClientId.ReadLocalLangJsonFile();
                if (CommandLineArgs.WorkId != Guid.Empty) {
                    try {
                        LocalJson.Instance.Init();
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }

                    GetFileAsync(AssemblyInfo.LangJsonFileUrl + "?t=" + DateTime.Now.Ticks, (data) => {
                        langJson = Encoding.UTF8.GetString(data);
                        ServerJson.Instance.Init(serverJson);
                        Language.Impl.LangJson.Instance.Init(langJson);
                        Logger.InfoDebugLine($"下载完成：{AssemblyInfo.LangJsonFileUrl}");
                        DoInit(callback);
                    });
                }
                else {
                    CountdownEvent countdown = new CountdownEvent(2);
                    GetFileAsync(AssemblyInfo.ServerJsonFileUrl + "?t=" + DateTime.Now.Ticks, (data) => {
                        serverJson = Encoding.UTF8.GetString(data);
                        Logger.InfoDebugLine($"下载完成：{AssemblyInfo.ServerJsonFileUrl}");
                        countdown.Signal();
                    });
                    GetFileAsync(AssemblyInfo.LangJsonFileUrl + "?t=" + DateTime.Now.Ticks, (data) => {
                        langJson = Encoding.UTF8.GetString(data);
                        Logger.InfoDebugLine($"下载完成：{AssemblyInfo.LangJsonFileUrl}");
                        countdown.Signal();
                    });
                    Task.Factory.StartNew(() => {
                        if (countdown.Wait(30 * 1000)) {
                            Logger.InfoDebugLine("json下载完成");
                            ServerJson.Instance.Init(serverJson);
                            Language.Impl.LangJson.Instance.Init(langJson);
                            DoInit(callback);
                        }
                        else {
                            Logger.InfoDebugLine("启动json下载超时");
                            ServerJson.Instance.Init(serverJson);
                            Language.Impl.LangJson.Instance.Init(langJson);
                            DoInit(callback);
                        }
                    });
                }
            });
        }

        private MinerProfile _minerProfile;
        private void DoInit(Action callback) {
            this.PackageDownloader = new PackageDownloader(this);
            this.AppSettingSet = new AppSettingSet(this);
            this.CalcConfigSet = new CalcConfigSet(this);

            ContextInit();

            this.KernelProfileSet = new KernelProfileSet(this);
            this.GpusSpeed = new GpusSpeed(this);
            this.CoinShareSet = new CoinShareSet(this);
            this.MineWorkSet = new MineWorkSet(this);
            this.MinerGroupSet = new MinerGroupSet(this);
            this.OverClockDataSet = new OverClockDataSet(this);
            this.ColumnsShowSet = new ColumnsShowSet(this);
            this._minerProfile = new MinerProfile(this, CommandLineArgs.WorkId);
            callback?.Invoke();
        }

        private void ContextInit() {
            this.SysDicSet = new SysDicSet(this, _isServerJson);
            this.SysDicItemSet = new SysDicItemSet(this, _isServerJson);
            this.CoinSet = new CoinSet(this, _isServerJson);
            this.GroupSet = new GroupSet(this, _isServerJson);
            this.CoinGroupSet = new CoinGroupSet(this, _isServerJson);
            this.PoolSet = new PoolSet(this, _isServerJson);
            this.CoinKernelSet = new CoinKernelSet(this, _isServerJson);
            this.PoolKernelSet = new PoolKernelSet(this, _isServerJson);
            this.KernelSet = new KernelSet(this, _isServerJson);
            this.KernelInputSet = new KernelInputSet(this, _isServerJson);
            this.KernelOutputSet = new KernelOutputSet(this, _isServerJson);
            this.KernelOutputFilterSet = new KernelOutputFilterSet(this, _isServerJson);
            this.KernelOutputTranslaterSet = new KernelOutputTranslaterSet(this, _isServerJson);
        }

        private void ContextReInit() {
            foreach (var handler in ContextHandlers) {
                VirtualRoot.UnPath(handler);
            }
            ContextHandlers.Clear();
            ContextInit();
            OnContextReInited?.Invoke();
            OnReRendContext?.Invoke();
        }
        #endregion


        public void GetTimeAsync(Action<DateTime> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message =
                            client.GetAsync($"http://server.ntminer.com:3339/api/AppSetting/GetTime");
                        DateTime response = message.Result.Content.ReadAsAsync<DateTime>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine($"GetTimeAsync失败 {e?.Message}");
                    callback?.Invoke(DateTime.Now);
                }
            });
        }

        #region Start
        public void Start() {
            GetTimeAsync((remoteTime) => {
                if (Math.Abs((DateTime.Now - remoteTime).TotalSeconds) < Timestamp.DesyncSeconds) {
                    Logger.OkDebugLine("时间同步");
                }
                else {
                    Logger.WarnDebugLine($"本机时间和服务器时间不同步，请调整，本地：{DateTime.Now}，服务器：{remoteTime}");
                }
            });
            NTMinerRegistry.SetLocation(ClientId.AppFileFullName);
            NTMinerRegistry.SetArguments(string.Join(" ", CommandLineArgs.Args));
            NTMinerRegistry.SetCurrentVersion(CurrentVersion.ToString());
            NTMinerRegistry.SetCurrentVersionTag(CurrentVersionTag);

            RefreshUserSet();

            Report.Init(this);

            #region 处理设置矿工名命令
            VirtualRoot.Accept<SetMinerNameCommand>(
                "处理设置矿工名命令",
                LogEnum.Console,
                action: message => {
                    SetMinerName(message.MinerName);
                    VirtualRoot.Happened(new MinerNameSetedEvent());
                });
            #endregion
            #region 增删改了用户后刷新守护进程的用户集
            VirtualRoot.On<UserAddedEvent>(
                "添加了新用户后刷新守护进程的用户集",
                LogEnum.Console,
                action: message => {
                    RefreshUserSet();
                });
            VirtualRoot.On<UserUpdatedEvent>(
                "更新了新用户后刷新守护进程的用户集",
                LogEnum.Console,
                action: message => {
                    RefreshUserSet();
                });
            VirtualRoot.On<UserRemovedEvent>(
                "移除了新用户后刷新守护进程的用户集",
                LogEnum.Console,
                action: message => {
                    RefreshUserSet();
                });
            #endregion
            #region 挖矿开始时将无份额内核重启份额计数置0
            int shareCount = 0;
            DateTime shareOn = DateTime.Now;
            VirtualRoot.On<MineStartedEvent>(
                "挖矿开始后将无份额内核重启份额计数置0，应用超频，启动NoDevFee，启动DevConsole，清理除当前外的Temp/Kernel",
                LogEnum.Console,
                action: message => {
                    // 将无份额内核重启份额计数置0
                    shareCount = 0;
                    shareOn = DateTime.Now;
                    Task.Factory.StartNew(() => {
                        try {
                            ICoinProfile coinProfile = MinerProfile.GetCoinProfile(message.MineContext.MainCoin.GetId());
                            if (coinProfile.IsOverClockEnabled) {
                                VirtualRoot.Execute(new CoinOverClockCommand(message.MineContext.MainCoin.GetId()));
                            }
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e.Message, e);
                        }
                    });
                    // 启动NoDevFee
                    var context = CurrentMineContext;
                    StartNoDevFeeRequest request = new StartNoDevFeeRequest {
                        ContextId = context.Id.GetHashCode(),
                        MinerName = context.MinerName,
                        Coin = context.MainCoin.Code,
                        OurWallet = context.MainCoinWallet,
                        TestWallet = context.MainCoin.TestWallet,
                        KernelName = context.Kernel.GetFullName()
                    };
                    Client.NTMinerDaemonService.StartNoDevFeeAsync(request, callback: null);
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
            VirtualRoot.On<Per10SecondEvent>(
                "每10秒钟检查是否需要重启",
                LogEnum.None,
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
            #region 启动5秒钟后优化windows环境
            VirtualRoot.On<HasBoot5SecondEvent>(
                "启动5秒钟后优化windows环境",
                LogEnum.Console,
                action: message => {
                    Task.Factory.StartNew(() => {
                        Windows.Error.DisableWindowsErrorUI();
                        Windows.Firewall.DisableFirewall();
                        Windows.UAC.DisableUAC();
                        Windows.WAU.DisableWAUAsync();
                        Windows.Defender.DisableAntiSpyware();
                        Windows.Power.PowerCfgOff();
                        Windows.BcdEdit.IgnoreAllFailures();
                    });
                });
            #endregion
            #region 每50分钟执行一次过期日志清理工作
            VirtualRoot.On<Per50MinuteEvent>(
                "每50分钟执行一次过期日志清理工作",
                LogEnum.Console,
                action: message => {
                    Cleaner.ClearKernelLogs();
                    Cleaner.ClearRootLogs();
                    Cleaner.ClearPackages();
                });
            #endregion
            #region 停止挖矿后停止NoDevFee
            VirtualRoot.On<MineStopedEvent>(
                "停止挖矿后停止NoDevFee",
                LogEnum.Console,
                 action: message => {
                     Client.NTMinerDaemonService.StopNoDevFeeAsync(callback: null);
                 });
            #endregion
            #region 周期确保守护进程在运行
            Daemon.DaemonUtil.RunNTMinerDaemon();
            VirtualRoot.On<Per20SecondEvent>(
                    "周期确保守护进程在运行",
                    LogEnum.None,
                    action: message => {
                        Daemon.DaemonUtil.RunNTMinerDaemon();
                        if (IsMining) {
                            var context = CurrentMineContext;
                            StartNoDevFeeRequest request = new StartNoDevFeeRequest {
                                ContextId = context.Id.GetHashCode(),
                                MinerName = context.MinerName,
                                Coin = context.MainCoin.Code,
                                OurWallet = context.MainCoinWallet,
                                TestWallet = context.MainCoin.TestWallet,
                                KernelName = context.Kernel.GetFullName()
                            };
                            Client.NTMinerDaemonService.StartNoDevFeeAsync(request, callback: null);
                        }
                    });
            #endregion
            #region 发生了用户活动时检查serverJson是否有新版本
            VirtualRoot.On<UserActionEvent>(
                    "发生了用户活动时检查serverJson是否有新版本",
                    LogEnum.Console,
                    action: message => {
                        if (!_isServerJson) {
                            return;
                        }
                        GetJsonFileVersionAsync(AssemblyInfo.ServerJsonFileName, (jsonFileVersion) => {
                            if (!string.IsNullOrEmpty(jsonFileVersion) && JsonFileVersion != jsonFileVersion) {
                                GetFileAsync(AssemblyInfo.ServerJsonFileUrl + "?t=" + DateTime.Now.Ticks, (data) => {
                                    string rawJson = Encoding.UTF8.GetString(data);
                                    Logger.InfoDebugLine($"下载完成：{AssemblyInfo.ServerJsonFileUrl} JsonFileVersion：{jsonFileVersion}");
                                    ServerJson.Instance.ReInit(rawJson);
                                    ContextReInit();
                                    JsonFileVersion = jsonFileVersion;
                                    Logger.InfoDebugLine("刷新完成");
                                });
                            }
                            else {
                                Write.DevLine("server.json没有新版本", ConsoleColor.Green);
                            }
                        });
                    });
            #endregion

            // 自动开始挖矿
            if ((this.MinerProfile.IsAutoStart || CommandLineArgs.IsAutoStart) && !this.IsMining) {
                this.StartMine(CommandLineArgs.WorkId);
            }
        }
        #endregion

        private void RefreshUserSet() {
            try {
                List<UserData> users = new List<UserData>();
                foreach (IUser item in MinerProfile.GetUsers()) {
                    if (item is UserData user) {
                        users.Add(user);
                    }
                    else {
                        users.Add(new UserData(item));
                    }
                }
                string json = VirtualRoot.JsonSerializer.Serialize(users);
                SpecialPath.WriteDaemonUsersJsonFile(json);
                Client.NTMinerDaemonService.RefreshUserSetAsync(callback: null);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        #region GetFileAsync
        public static void GetFileAsync(string fileUrl, Action<byte[]> callback) {
            Task.Factory.StartNew(() => {
                try {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(fileUrl));
                    webRequest.Method = "GET";
                    HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                    using (MemoryStream ms = new MemoryStream())
                    using (Stream stream = response.GetResponseStream()) {
                        byte[] buffer = new byte[1024];
                        int n = stream.Read(buffer, 0, buffer.Length);
                        while (n > 0) {
                            ms.Write(buffer, 0, n);
                            n = stream.Read(buffer, 0, buffer.Length);
                        }
                        byte[] data = new byte[ms.Length];
                        ms.Position = 0;
                        ms.Read(data, 0, data.Length);
                        callback?.Invoke(data);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(new byte[0]);
                }
            });
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
            Task.Factory.StartNew(() => {
                StopMine();
                callback?.Invoke();
            });
        }
        private void StopMine() {
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
        public void RestartMine() {
            this.StopMineAsync(() => {
                Logger.WarnWriteLine("正在重启内核");
                StartMine(CommandLineArgs.WorkId);
            });
        }
        #endregion

        #region StartMine
        public void StartMine(Guid workId) {
            try {
                if (workId != CommandLineArgs.WorkId) {
                    List<string> args = CommandLineArgs.Args;
                    if (CommandLineArgs.WorkId != Guid.Empty) {
                        for (int i = 0; i < args.Count; i++) {
                            if (args[i].StartsWith("workid=", StringComparison.OrdinalIgnoreCase)) {
                                args[i] = "workid=" + workId.ToString();
                                break;
                            }
                        }
                    }
                    else {
                        args.Add("workid=" + workId.ToString());
                    }
                    if (!CommandLineArgs.IsAutoStart) {
                        args.Add("--autostart");
                    }
                    Windows.Cmd.RunClose(ClientId.AppFileFullName, string.Join(" ", args));
                    UIThread.Execute(() => {
                        Application.Current.MainWindow?.Close();
                    });
                    return;
                }
                IWorkProfile minerProfile = this.MinerProfile;
                ICoin mainCoin;
                if (!this.CoinSet.TryGetCoin(minerProfile.CoinId, out mainCoin)) {
                    Write.UserLine("没有选择主挖币种。", ConsoleColor.Red);
                    return;
                }
                ICoinProfile coinProfile = minerProfile.GetCoinProfile(minerProfile.CoinId);
                IPool mainCoinPool;
                if (!this.PoolSet.TryGetPool(coinProfile.PoolId, out mainCoinPool)) {
                    Write.UserLine("没有选择主币矿池。", ConsoleColor.Red);
                    return;
                }
                ICoinKernel coinKernel;
                if (!this.CoinKernelSet.TryGetCoinKernel(coinProfile.CoinKernelId, out coinKernel)) {
                    Write.UserLine("没有选择挖矿内核。", ConsoleColor.Red);
                    return;
                }
                IKernel kernel;
                if (!this.KernelSet.TryGetKernel(coinKernel.KernelId, out kernel)) {
                    Write.UserLine("无效的挖矿内核。", ConsoleColor.Red);
                    return;
                }
                if (!kernel.IsSupported()) {
                    Write.UserLine($"该内核不支持{GpuSet.GpuType.GetDescription()}卡。", ConsoleColor.Red);
                    return;
                }
                IKernelInput kernelInput;
                if (!this.KernelInputSet.TryGetKernelInput(kernel.KernelInputId, out kernelInput)) {
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
                if (_currentMineContext != null) {
                    this.StopMine();
                }
                // kill上一个上下文的进程，上一个上下文进程名不一定和下一个相同
                if (_currentMineContext != null && _currentMineContext.Kernel != null) {
                    string processName = _currentMineContext.Kernel.GetProcessName();
                    Windows.TaskKill.Kill(processName);
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
                            StartMine(workId);
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
                    IMineContext mineContext = new MineContext(GetMinerName(), mainCoin, mainCoinPool, kernel, coinKernel, coinProfile.Wallet, commandLine);
                    if (coinKernelProfile.IsDualCoinEnabled) {
                        mineContext = new DualMineContext(mineContext, dualCoin, dualCoinPool, coinProfile.DualCoinWallet, coinKernelProfile.DualCoinWeight);
                    }
                    _currentMineContext = mineContext;
                    // kill这一个上下文的进程
                    Windows.TaskKill.Kill(mineContext.Kernel.GetProcessName());
                    MinerProcess.CreateProcessAsync(mineContext);
                    VirtualRoot.Happened(new MineStartedEvent(mineContext));
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
                    if (_gpuSet == null) {
                        _gpuSet = EmptyGpuSet.Instance;
                    }
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
