using NTMiner.Core;
using NTMiner.Core.Gpus;
using NTMiner.Core.Gpus.Impl;
using NTMiner.Core.Impl;
using NTMiner.Core.Kernels;
using NTMiner.Core.Kernels.Impl;
using NTMiner.Core.Profiles;
using NTMiner.Core.Profiles.Impl;
using NTMiner.Core.SysDics;
using NTMiner.Core.SysDics.Impl;
using NTMiner.ServiceContracts.DataObjects;
using NTMiner.ServiceContracts.MinerClient;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NTMiner {
    public partial class NTMinerRoot : INTMinerRoot {
        public DateTime CreatedOn { get; private set; }

        public IUserSet UserSet { get; private set; }

        #region cotr
        private NTMinerRoot() {
            CreatedOn = DateTime.Now;
        }
        #endregion

        #region Init
        private readonly object _locker = new object();
        private bool _isInited = false;
        public void Init(Action callback) {
            if (!this._isInited) {
                lock (this._locker) {
                    if (!this._isInited) {
                        string rawNTMinerJson = string.Empty;
                        if (File.Exists(SpecialPath.ServerJsonFileFullName)) {
                            rawNTMinerJson = File.ReadAllText(SpecialPath.ServerJsonFileFullName);
                        }
                        string rawLangJson = string.Empty;
                        if (File.Exists(ClientId.LocalLangJsonFileFullName)) {
                            rawLangJson = File.ReadAllText(ClientId.LocalLangJsonFileFullName);
                        }
                        if (CommandLineArgs.IsSkipDownloadJson) {
                            DoInit(rawNTMinerJson, rawLangJson, callback);
                            Global.Access<HasBoot5SecondEvent>(
                                Guid.Parse("546CCF96-D87E-4436-B236-0A9416DFE28D"),
                                "Debug打印",
                                LogEnum.Log,
                                action: (message) => {
                                    Global.Logger.OkDebugLine("已跳过从服务器下载Json");
                                });
                            _isInited = true;
                        }
                        else {
                            CountdownEvent countdown = new CountdownEvent(2);
                            GetFileAsync(AssemblyInfo.ServerJsonFileUrl + "?t=" + DateTime.Now.Ticks, (data) => {
                                rawNTMinerJson = Encoding.UTF8.GetString(data);
                                Global.Logger.InfoDebugLine($"下载完成：{AssemblyInfo.ServerJsonFileUrl}");
                                countdown.Signal();
                            });
                            Server.AppSettingService.GetAppSettingAsync(AssemblyInfo.ServerJsonFileName, response => {
                                if (response.IsSuccess() && response.Data != null && response.Data.Value != null) {
                                    if (response.Data.Value is ulong value) {
                                        JsonFileVersion = value;
                                    }
                                }
                                else {
                                    Global.Logger.ErrorDebugLine($"GetAppSettingAsync({AssemblyInfo.ServerJsonFileName})失败");
                                }
                            });
                            GetFileAsync(AssemblyInfo.LangJsonFileUrl + "?t=" + DateTime.Now.Ticks, (data) => {
                                rawLangJson = Encoding.UTF8.GetString(data);
                                Global.Logger.InfoDebugLine($"下载完成：{AssemblyInfo.LangJsonFileUrl}");
                                countdown.Signal();
                            });
                            Task.Factory.StartNew(() => {
                                if (countdown.Wait(30 * 1000)) {
                                    Global.Logger.InfoDebugLine("json下载完成");
                                    DoInit(rawNTMinerJson, rawLangJson, callback);
                                }
                                else {
                                    Global.Logger.InfoDebugLine("启动json下载超时");
                                    DoInit(rawNTMinerJson, rawLangJson, callback);
                                }
                                _isInited = true;
                            });
                        }
                    }
                }
            }
        }

        public void DoInit(string rawNTMinerJson, string rawLangJson, Action callback) {
            Global.Logger.InfoDebugLine("SystemRoo.PrivateInit start");
            ServerJson.Instance.Init(rawNTMinerJson);
            Language.Impl.LangJson.Instance.Init(rawLangJson);
            this.PackageDownloader = new PackageDownloader(this);
            this.SysDicSet = new SysDicSet(this);
            this.UserSet = new UserSet(SpecialPath.LocalDbFileFullName);
            this.SysDicItemSet = new SysDicItemSet(this);
            this.CoinSet = new CoinSet(this);
            this.GroupSet = new GroupSet(this);
            this.CoinGroupSet = new CoinGroupSet(this);
            this.CalcConfigSet = new CalcConfigSet(this);
            this.WalletSet = new WalletSet(this);
            this.PoolSet = new PoolSet(this);
            this.CoinKernelSet = new CoinKernelSet(this);
            this.PoolKernelSet = new PoolKernelSet(this);
            this.KernelSet = new KernelSet(this);
            this.KernelProfileSet = new KernelProfileSet(this);
            this.KernelInputSet = new KernelInputSet(this);
            this.KernelOutputSet = new KernelOutputSet(this);
            this.KernelOutputFilterSet = new KernelOutputFilterSet(this);
            this.KernelOutputTranslaterSet = new KernelOutputTranslaterSet(this);
            this.GpusSpeed = new GpusSpeed(this);
            this.CoinShareSet = new CoinShareSet(this);
            this.MineWorkSet = new MineWorkSet(this);
            this.MinerGroupSet = new MinerGroupSet(this);
            this._minerProfile = new MinerProfile(this);
            this.CoinProfileSet = new CoinProfileSet(this);
            this.PoolProfileSet = new PoolProfileSet(this);
            this.CoinKernelProfileSet = new CoinKernelProfileSet(this);

            callback?.Invoke();
            Global.Logger.InfoDebugLine("SystemRoo.PrivateInit end");
        }
        #endregion

        private MinerProfile _minerProfile;
        private List<ServiceHost> _serviceHosts = null;
        #region Start
        public void Start() {
            Global.Logger.InfoDebugLine("开始启动Wcf服务");
            string baseUrl = $"http://{Global.Localhost}:{Global.ClientPort}/";
            ServiceHost minerClientServiceHost = new ServiceHost(typeof(Core.Impl.MinerClientService));
            minerClientServiceHost.AddServiceEndpoint(typeof(IMinerClientService), ChannelFactory.BasicHttpBinding, new Uri(new Uri(baseUrl), nameof(IMinerClientService)));
            _serviceHosts = new List<ServiceHost>
            {
                minerClientServiceHost
            };
            foreach (var serviceHost in _serviceHosts) {
                ServiceMetadataBehavior serviceMetadata = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (serviceMetadata == null) {
                    serviceMetadata = new ServiceMetadataBehavior();
                    serviceHost.Description.Behaviors.Add(serviceMetadata);
                }
                serviceMetadata.HttpGetEnabled = false;

                serviceHost.Open();
            }

            Global.Logger.OkDebugLine($"服务启动成功: {DateTime.Now}.");
            Global.Logger.InfoDebugLine("服务列表：");
            foreach (var serviceHost in _serviceHosts) {
                foreach (var endpoint in serviceHost.Description.Endpoints) {
                    Global.Logger.InfoDebugLine(endpoint.Address.Uri.ToString());
                }
            }
            Global.Logger.OkDebugLine("Wcf服务启动完成");

            Server.TimeService.GetTimeAsync((remoteTime) => {
                if (Math.Abs((DateTime.Now - remoteTime).TotalSeconds) < Global.DesyncSeconds) {
                    Global.Logger.OkDebugLine("时间同步");
                }
                else {
                    Global.Logger.WarnDebugLine($"本机时间和服务器时间不同步，请调整，本地：{DateTime.Now}，服务器：{remoteTime}");
                }
            });

            NTMinerRegistry.SetLocation(ClientId.AppFileFullName);
            NTMinerRegistry.SetArguments(string.Join(" ", CommandLineArgs.Args));
            NTMinerRegistry.SetCurrentVersion(CurrentVersion.ToString());
            NTMinerRegistry.SetCurrentVersionTag(CurrentVersionTag);

            Report.Init(this);

            int shareCount = 0;
            DateTime shareOn = DateTime.Now;
            #region 挖矿开始时将无份额内核重启份额计数置0
            Global.Access<MineStartedEvent>(
                Guid.Parse("e69e8729-868b-4b5d-b120-2914fffddf90"),
                "挖矿开始时将无份额内核重启份额计数置0",
                LogEnum.Console,
                action: message => {
                    shareCount = 0;
                    shareOn = DateTime.Now;
                });
            #endregion
            #region 每10秒钟检查是否需要重启
            Global.Access<Per10SecondEvent>(
                Guid.Parse("16b3b7b4-5e6c-46b0-97a4-90e085614b78"),
                "每10秒钟检查是否需要重启",
                LogEnum.None,
                action: message => {
                    #region 重启电脑
                    try {
                        if (MinerProfile.IsPeriodicRestartComputer) {
                            if ((DateTime.Now - this.CreatedOn).TotalHours > MinerProfile.PeriodicRestartComputerHours) {
                                Global.Logger.WarnWriteLine($"每运行{MinerProfile.PeriodicRestartKernelHours}小时重启电脑");
                                Windows.Power.Restart();
                                return;// 退出
                            }
                        }
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                    }
                    #endregion

                    #region 周期重启内核
                    try {
                        if (IsMining && MinerProfile.IsPeriodicRestartKernel) {
                            if ((DateTime.Now - CurrentMineContext.CreatedOn).TotalHours > MinerProfile.PeriodicRestartKernelHours) {
                                Global.Logger.WarnWriteLine($"每运行{MinerProfile.PeriodicRestartKernelHours}小时重启内核");
                                RestartMine();
                                return;// 退出
                            }
                        }
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
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
                                        Global.Logger.WarnWriteLine($"{MinerProfile.NoShareRestartKernelMinutes}分钟收益没有增加重启内核");
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
                        Global.Logger.ErrorDebugLine(e.Message, e);
                    }
                    #endregion
                });
            #endregion
            #region 启动5秒钟后优化windows环境
            Global.Access<HasBoot5SecondEvent>(
                Guid.Parse("32c49476-8232-4130-bd81-52443ed4ab4e"),
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
            #region 挖矿开始后清理除当前外的Temp/Kernel
            Global.Access<MineStartedEvent>(
                Guid.Parse("57cf1fad-e75b-4eb1-8868-39953b21cced"),
                "挖矿开始后清理除当前外的Temp/Kernel",
                LogEnum.Console,
                action: message => {
                    Cleaner.CleanKernels();
                });
            #endregion
            #region 每50分钟执行一次过期日志清理工作
            Global.Access<Per50MinuteEvent>(
                Guid.Parse("d27419f4-7eda-4dbf-bf25-e8c5f1efacb5"),
                "每50分钟执行一次过期日志清理工作",
                LogEnum.Console,
                action: message => {
                    Cleaner.ClearKernelLogs();
                    Cleaner.ClearRootLogs();
                    Cleaner.ClearPackages();
                });
            #endregion
            #region 启动10秒钟后自动开始挖矿
            if (this.MinerProfile.IsAutoStart || CommandLineArgs.IsAutoStart) {
                Global.Access<HasBoot10SecondEvent>(
                    Guid.Parse("53004081-ee0f-44a2-b848-95668e63aa8c"),
                    "启动10秒钟后自动开始挖矿",
                    LogEnum.Console,
                    action: (message) => {
                        if ((this.MinerProfile.IsAutoStart || CommandLineArgs.IsAutoStart) && !IsAutoStartCanceled && !this.IsMining) {
                            Execute.OnUIThread(() => {
                                this.StartMine(CommandLineArgs.WorkId);
                            });
                        }
                        if (IsAutoStartCanceled) {
                            Global.Logger.WarnDebugLine("自动挖矿已取消");
                        }
                    });
            }
            #endregion
            #region 开始挖矿后启动DevConsole
            if (DevMode.IsDevMode) {
                Global.Access<MineStartedEvent>(
                 Guid.Parse("638627D4-31EB-42F9-B92B-31B28D78B792"),
                "开始挖矿后启动DevConsole",
                LogEnum.Console,
                 action: message => {
                     string poolIp = CurrentMineContext.MainCoinPool.GetIp();
                     string consoleTitle = CurrentMineContext.MainCoinPool.Server;
                     Daemon.DaemonUtil.RunDevConsoleAsync(poolIp, consoleTitle);
                 });
            }
            #endregion
            #region 开始挖矿后启动NoDevFee
            Global.Access<MineStartedEvent>(
                 Guid.Parse("3362e34f-74a9-4db9-a550-f7779994b459"),
                "开始挖矿后启动NoDevFee",
                LogEnum.Console,
                 action: message => {
                     NTMinerClientDaemon.Instance.StartAsync(callback: null);
                 });
            #endregion
            #region 停止挖矿后停止NoDevFee
            Global.Access<MineStopedEvent>(
                 Guid.Parse("D693A381-0AAD-4768-BC6F-2EEDD9F321BC"),
                "停止挖矿后停止NoDevFee",
                LogEnum.Console,
                 action: message => {
                     NTMinerClientDaemon.Instance.StopAsync(callback: null);
                 });
            #endregion
            #region 周期确保守护进程在运行
            Daemon.DaemonUtil.RunNTMinerDaemon();
            if (!CommandLineArgs.IsControlCenter) {
                Global.Access<Per20SecondEvent>(
                    Guid.Parse("F11D42E1-CEBA-4C7E-BB2E-3AC1EBB03F14"),
                    "周期确保守护进程在运行",
                    LogEnum.None,
                    action: message => {
                        Daemon.DaemonUtil.RunNTMinerDaemon();
                        if (IsMining) {
                            NTMinerClientDaemon.Instance.StartAsync(callback: null);
                        }
                    });
            }
            #endregion
            #region 发生了用户活动时检查serverJson是否有新版本
            Global.Access<UserActionEvent>(
                    Guid.Parse("C7FA9D54-4D16-47F5-AA81-4C76384F95B9"),
                    "发生了用户活动时检查serverJson是否有新版本",
                    LogEnum.Console,
                    action: message => {
                        if (DevMode.IsDebugMode) {
                            return;
                        }
                        Server.AppSettingService.GetAppSettingAsync(AssemblyInfo.ServerJsonFileName, response => {
                            if (response.IsSuccess() && response.Data != null && response.Data.Value is ulong value) {
                                if (JsonFileVersion != value) {
                                    GetFileAsync(AssemblyInfo.ServerJsonFileUrl + "?t=" + DateTime.Now.Ticks, (data) => {
                                        string rawNTMinerJson = Encoding.UTF8.GetString(data);
                                        Global.Logger.InfoDebugLine($"下载完成：{AssemblyInfo.ServerJsonFileUrl} JsonFileVersion：{Global.FromTimestamp(value)}");
                                        JsonFileVersion = value;
                                        ServerJson.Instance.ReInit(rawNTMinerJson);
                                        Global.Logger.InfoDebugLine("ServerJson数据集刷新完成");
                                        Execute.OnUIThread(() => {
                                            var refreshCommands = RefreshCommand.CreateRefreshCommands();
                                            foreach (var refreshCommand in refreshCommands) {
                                                Global.Execute(refreshCommand);
                                            }
                                            Global.Logger.InfoDebugLine("刷新完成");
                                        });
                                    });
                                }
                                else {
                                    Global.WriteDevLine("server.json没有新版本", ConsoleColor.Green);
                                }
                            }
                            else {
                                Global.Logger.ErrorDebugLine($"GetAppSettingAsync({AssemblyInfo.ServerJsonFileName})失败");
                            }
                        });
                    });
            #endregion
        }
        #endregion

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
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(new byte[0]);
                }
            });
        }
        #endregion

        #region Exit
        public void Exit() {
            try {
                if (_serviceHosts != null) {
                    foreach (var serviceHost in _serviceHosts) {
                        serviceHost.Close();
                    }
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
            if (_currentMineContext != null) {
                StopMineAsync();
            }
        }
        #endregion

        #region StopMine
        public void StopMineAsync(Action callback = null) {
            Task.Factory.StartNew(() => {
                try {
                    if (_currentMineContext != null && _currentMineContext.Kernel != null) {
                        string processName = _currentMineContext.Kernel.GetProcessName();
                        Windows.TaskKill.Kill(processName);
                    }
                    Global.Logger.WarnWriteLine("挖矿停止");
                    var mineContext = _currentMineContext;
                    _currentMineContext = null;
                    Global.Happened(new MineStopedEvent(mineContext));
                    callback?.Invoke();
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                }
            });
        }
        #endregion

        #region RestartMine
        public void RestartMine() {
            this.StopMineAsync(()=> {
                Global.Logger.WarnWriteLine("正在重启内核");
                StartMine(CommandLineArgs.WorkId);
            });
        }
        #endregion

        #region StartMine
        public void StartMine(Guid workId) {
            try {
                if (workId != CommandLineArgs.WorkId) {
                    List<string> args = CommandLineArgs.Args;
                    if (CommandLineArgs.IsWorker) {
                        for (int i = 0; i < args.Count; i++) {
                            if (args[i].StartsWith("--workid=", StringComparison.OrdinalIgnoreCase)) {
                                args[i] = "--workid=" + workId.ToString();
                                break;
                            }
                        }
                    }
                    else {
                        args.Add("--workid=" + workId.ToString());
                    }
                    if (!CommandLineArgs.IsAutoStart) {
                        args.Add("--autostart");
                    }
                    Windows.Cmd.RunClose(ClientId.AppFileFullName, string.Join(" ", args));
                    Application.Current.MainWindow.Close();
                    return;
                }
                IMinerProfile minerProfile = this.MinerProfile;
                ICoin mainCoin;
                if (!this.CoinSet.TryGetCoin(minerProfile.CoinId, out mainCoin)) {
                    Global.Logger.ErrorWriteLine("没有选择主挖币种。");
                    return;
                }
                ICoinProfile coinProfile = this.CoinProfileSet.GetCoinProfile(minerProfile.CoinId);
                IPool mainCoinPool;
                if (!this.PoolSet.TryGetPool(coinProfile.PoolId, out mainCoinPool)) {
                    Global.Logger.ErrorWriteLine("没有选择主币矿池。");
                    return;
                }
                ICoinKernel coinKernel;
                if (!this.CoinKernelSet.TryGetCoinKernel(coinProfile.CoinKernelId, out coinKernel)) {
                    Global.Logger.ErrorWriteLine("没有选择挖矿内核。");
                    return;
                }
                IKernel kernel;
                if (!this.KernelSet.TryGetKernel(coinKernel.KernelId, out kernel)) {
                    Global.Logger.ErrorWriteLine("无效的挖矿内核。");
                    return;
                }
                if (!kernel.IsSupported()) {
                    Global.Logger.ErrorWriteLine($"该内核不支持{GpuSet.GpuType.GetDescription()}卡。");
                    return;
                }
                IKernelInput kernelInput;
                if (!this.KernelInputSet.TryGetKernelInput(kernel.KernelInputId, out kernelInput)) {
                    Global.Logger.ErrorWriteLine("未设置内核输入");
                    return;
                }
                if (string.IsNullOrEmpty(coinProfile.Wallet)) {
                    coinProfile.Wallet = mainCoin.TestWallet;
                }
                if (mainCoinPool.IsUserMode) {
                    IPoolProfile poolProfile = PoolProfileSet.GetPoolProfile(mainCoinPool.GetId());
                    string userName = poolProfile.UserName;
                    if (string.IsNullOrEmpty(userName)) {
                        Global.Logger.ErrorWriteLine("没有填写矿池用户名。");
                        return;
                    }
                }
                if (string.IsNullOrEmpty(coinProfile.Wallet) && !mainCoinPool.IsUserMode) {
                    Global.Logger.ErrorWriteLine("没有填写钱包地址。");
                    return;
                }
                IMineContext mineContext = this.CreateMineContext(
                        minerProfile.MinerName,
                        mainCoin, mainCoinPool, kernel, coinKernel,
                        coinProfile.Wallet);
                ICoinKernelProfile coinKernelProfile = this.CoinKernelProfileSet.GetCoinKernelProfile(coinKernel.GetId());
                if (coinKernelProfile.IsDualCoinEnabled) {
                    ICoin dualCoin;
                    if (!this.CoinSet.TryGetCoin(coinKernelProfile.DualCoinId, out dualCoin)) {
                        Global.Logger.ErrorWriteLine("没有选择双挖币种。");
                        return;
                    }
                    IPool dualCoinPool;
                    coinProfile = this.CoinProfileSet.GetCoinProfile(coinKernelProfile.DualCoinId);
                    if (!this.PoolSet.TryGetPool(coinProfile.DualCoinPoolId, out dualCoinPool)) {
                        Global.Logger.ErrorWriteLine("没有选择双挖矿池。");
                        return;
                    }
                    if (string.IsNullOrEmpty(coinProfile.DualCoinWallet)) {
                        coinProfile.DualCoinWallet = dualCoin.TestWallet;
                    }
                    if (string.IsNullOrEmpty(coinProfile.DualCoinWallet)) {
                        Global.Logger.ErrorWriteLine("没有填写双挖钱包。");
                        return;
                    }
                    mineContext = this.CreateDualMineContext(mineContext, dualCoin, dualCoinPool, coinProfile.DualCoinWallet, coinKernelProfile.DualCoinWeight);
                }
                if (_currentMineContext != null) {
                    this.StopMineAsync();
                }
                // kill上一个上下文的进程，上一个上下文进程名不一定和下一个相同
                if (_currentMineContext != null && _currentMineContext.Kernel != null) {
                    string processName = _currentMineContext.Kernel.GetProcessName();
                    Windows.TaskKill.Kill(processName);
                }
                if (string.IsNullOrEmpty(kernel.Package)) {
                    Global.Logger.ErrorWriteLine(kernel.FullName + "没有内核包");
                    this.StopMineAsync();
                    return;
                }
                if (string.IsNullOrEmpty(kernelInput.Args)) {
                    Global.Logger.ErrorWriteLine(kernel.FullName + "没有配置运行参数");
                    return;
                }
                string packageZipFileFullName = Path.Combine(SpecialPath.PackagesDirFullName, kernel.Package);
                if (!File.Exists(packageZipFileFullName)) {
                    Global.Logger.WarnWriteLine(kernel.FullName + "本地内核包不存在，触发自动下载");
                    if (KernelDownloader == null) {
                        throw new InvalidProgramException("为赋值NTMinerRoot.KernelDownloader");
                    }
                    KernelDownloader.Download(kernel.GetId(), downloadComplete: (isSuccess, message) => {
                        if (isSuccess) {
                            StartMine(workId);
                        }
                    });
                    return;
                }
                else {
                    _currentMineContext = mineContext;
                    // kill这一个上下文的进程
                    Windows.TaskKill.Kill(mineContext.Kernel.GetProcessName());
                    MinerProcess.CreateProcessAsync(mineContext);
                }
                Global.Happened(new MineStartedEvent(mineContext));
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
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

        public IMinerProfile MinerProfile {
            get { return _minerProfile; }
        }

        public IMineWorkSet MineWorkSet { get; private set; }

        public IMinerGroupSet MinerGroupSet { get; private set; }

        public void SetMinerProfileProperty(string propertyName, object value) {
            _minerProfile.SetValue(propertyName, value);
            Global.Logger.InfoDebugLine($"SetMinerProfileProperty({propertyName}, {value})");
        }

        public object GetMineWorkProperty(string propertyName) {
            return _minerProfile.GetValue(propertyName);
        }

        public void SetCoinProfileProperty(Guid coinId, string propertyName, object value) {
            string coinCode = "意外的币种";
            ICoin coin;
            if (this.CoinSet.TryGetCoin(coinId, out coin)) {
                this.CoinProfileSet.SetCoinProfileProperty(coinId, propertyName, value);
                coinCode = coin.Code;
            }
            Global.Logger.InfoDebugLine($"SetCoinProfileProperty({coinCode}, {propertyName}, {value})");
        }

        public void SetPoolProfileProperty(Guid poolId, string propertyName, object value) {
            string poolName = "意外的矿池";
            IPool pool;
            if (this.PoolSet.TryGetPool(poolId, out pool)) {
                poolName = pool.Name;
                if (!pool.IsUserMode) {
                    Global.WriteDevLine("不是用户名密码模式矿池", ConsoleColor.Green);
                    return;
                }
                this.PoolProfileSet.SetPoolProfileProperty(poolId, propertyName, value);
            }
            Global.Logger.InfoDebugLine($"SetPoolProfileProperty({poolName}, {propertyName}, {value})");
        }

        public void SetCoinKernelProfileProperty(Guid coinKernelId, string propertyName, object value) {
            string coinCode = "意外的币种";
            string kernelName = "意外的内核";
            ICoinKernel coinKernel;
            if (this.CoinKernelSet.TryGetCoinKernel(coinKernelId, out coinKernel)) {
                ICoin coin;
                if (this.CoinSet.TryGetCoin(coinKernel.CoinId, out coin)) {
                    coinCode = coin.Code;
                }
                IKernel kernel;
                if (this.KernelSet.TryGetKernel(coinKernel.KernelId, out kernel)) {
                    kernelName = kernel.FullName;
                }
                this.CoinKernelProfileSet.SetCoinKernelProfileProperty(coinKernelId, propertyName, value);
            }
            Global.Logger.InfoDebugLine($"SetCoinKernelProfileProperty({coinCode}, {kernelName}, {propertyName}, {value})");
        }

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
                        sb.Append(g.Key).Append(" x ").Append(g.Count());
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
                        Global.Logger.ErrorDebugLine(ex);
                    }
                    if (_gpuSet == null || _gpuSet.Count == 0) {
                        try {
                            _gpuSet = new AMDGpuSet(this);
                        }
                        catch (Exception ex) {
                            _gpuSet = EmptyGpuSet.Instance;
                            Global.Logger.ErrorDebugLine(ex);
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

        public IWalletSet WalletSet { get; private set; }

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

        public ICoinProfileSet CoinProfileSet { get; private set; }

        public IPoolProfileSet PoolProfileSet { get; private set; }

        public ICoinKernelProfileSet CoinKernelProfileSet { get; private set; }

        public IMineContext CreateMineContext(
            string minerName,
            ICoin mainCoin,
            IPool mainCoinPool,
            IKernel kernel,
            ICoinKernel coinKernel,
            string mainCoinWallet) {
            return new MineContext(minerName, mainCoin, mainCoinPool, kernel, coinKernel, mainCoinWallet);
        }

        public IDualMineContext CreateDualMineContext(
            IMineContext mineContext,
            ICoin dualCoin,
            IPool dualCoinPool,
            string dualCoinWallet,
            double dualCoinWeight) {
            return new DualMineContext(mineContext, dualCoin, dualCoinPool, dualCoinWallet, dualCoinWeight);
        }
    }
}
