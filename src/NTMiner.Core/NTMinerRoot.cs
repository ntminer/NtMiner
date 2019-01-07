using Microsoft.Win32;
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using System.Windows;

namespace NTMiner {
    public partial class NTMinerRoot : INTMinerRoot {
        #region cotr
        private NTMinerRoot() {
            Task.Factory.StartNew(() => {
                object locker = new object();
                bool isServerJsonDownloaded = false;
                bool isLangJsonDownloaded = false;
                Global.Access<HasBoot5SecondEvent>(
                    Guid.Parse("5746e92f-8c79-4f91-a54d-90aa83d2bd1e"),
                    "检查server.json是否下载成功",
                    LogEnum.Log,
                    action: (message) => {
                        lock (locker) {
                            if (!isServerJsonDownloaded || !isLangJsonDownloaded) {
                                Init();
                            }
                            else {
                                Global.Logger.Info("下载成功了");
                            }
                        }
                    });
                using (WebClient webClient = new WebClient()) {
                    string jsonUrl = "https://minerjson.oss-cn-beijing.aliyuncs.com/server.json";
                    Global.DebugLine("下载：" + jsonUrl);
                    webClient.DownloadFileCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) => {
                        if (!e.Cancelled && e.Error == null) {
                            lock (locker) {
                                isServerJsonDownloaded = true;
                                if (isLangJsonDownloaded) {
                                    Init();
                                }
                            }
                        }
                    };
                    webClient.DownloadFileAsync(new Uri(jsonUrl), SpecialPath.NTMinerJsonFileFullName);
                }
                using (WebClient webClient = new WebClient()) {
                    string jsonUrl = "https://minerjson.oss-cn-beijing.aliyuncs.com/lang.json";
                    Global.DebugLine("下载：" + jsonUrl);
                    webClient.DownloadFileCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) => {
                        if (!e.Cancelled && e.Error == null) {
                            lock (locker) {
                                isLangJsonDownloaded = true;
                                if (isServerJsonDownloaded) {
                                    Init();
                                }
                            }
                        }
                    };
                    webClient.DownloadFileAsync(new Uri(jsonUrl), SpecialPath.LangJsonFileFullName);
                }
            });
        }
        #endregion

        #region Init
        public void Init() {
            if (!this._isInited) {
                lock (this._locker) {
                    if (!this._isInited) {
                        BootLog.Log("SystemRoo.PrivateInit start");
                        this.PackageDownloader = new PackageDownloader(this);
                        this.SysDicSet = new SysDicSet(this);
                        this.SysDicItemSet = new SysDicItemSet(this);
                        this.CoinSet = new CoinSet(this);
                        this.GroupSet = new GroupSet(this);
                        this.CoinGroupSet = new CoinGroupSet(this);
                        this.CalcConfigSet = new CalcConfigSet(this);
                        this.WalletSet = new WalletSet(this);
                        this.PoolSet = new PoolSet(this);
                        this.CoinKernelSet = new CoinKernelSet(this);
                        this.KernelSet = new KernelSet(this);
                        this.KernelProfileSet = new KernelProfileSet(this);
                        this.KernelOutputFilterSet = new KernelOutputFilterSet(this);
                        this.KernelOutputTranslaterSet = new KernelOutputTranslaterSet(this);
                        this.GpusSpeed = new GpusSpeed(this);
                        this.CoinShareSet = new CoinShareSet(this);
                        this.MineWorkSet = new MineWorkSet(this);
                        this.MinerGroupSet = new MinerGroupSet(this);
                        this._minerProfile = new MinerProfile(this);
                        this.CoinProfileSet = new CoinProfileSet(this);
                        this.CoinKernelProfileSet = new CoinKernelProfileSet(this);

                        Inited?.Invoke();
                        BootLog.Log("SystemRoo.PrivateInit end");
                        this._isInited = true;
                    }
                }
            }
        }
        #endregion

        private MinerProfile _minerProfile;
        private readonly object _locker = new object();
        private bool _isInited = false;
        private List<ServiceHost> _serviceHosts = null;
        #region Start
        public void Start() {
            BootLog.Log("开始启动Wcf服务");
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

            Global.DebugLine($"服务启动成功: {DateTime.Now}.");
            Global.DebugLine("服务列表：");
            foreach (var serviceHost in _serviceHosts) {
                foreach (var endpoint in serviceHost.Description.Endpoints) {
                    Global.DebugLine(endpoint.Address.Uri.ToString());
                }
            }
            BootLog.Log("Wcf服务启动完成");

            Server.TimeService.GetTime((remoteTime) => {
                if (Math.Abs((DateTime.Now - remoteTime).TotalSeconds) < Global.DesyncSeconds) {
                    Global.DebugLine("时间同步");
                }
                else {
                    Global.WriteLine($"本机时间和服务器时间不同步，请调整，本地：{DateTime.Now}，服务器：{remoteTime}", ConsoleColor.Red);
                }
            });

            Windows.Registry.SetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "Location", ClientId.AppFileFullName);
            Windows.Registry.SetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "Arguments", string.Join(" ", CommandLineArgs.Args));
            Windows.Registry.SetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "CurrentVersion", CurrentVersion.ToString());
            Windows.Registry.SetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "CurrentVersionTag", CurrentVersionTag);

            Report.Init(this);

            int shareCount = 0;
            DateTime shareOn = DateTime.Now;
            #region 挖矿开始时将无份额内核重启份额计数置0
            Global.Access<MineStartedEvent>(
                Guid.Parse("e69e8729-868b-4b5d-b120-2914fffddf90"),
                "挖矿开始时将无份额内核重启份额计数置0",
                LogEnum.None,
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
                            if ((DateTime.Now - shareOn).TotalHours > MinerProfile.PeriodicRestartComputerHours) {
                                Global.WriteLine($"每运行{MinerProfile.PeriodicRestartKernelHours}小时重启电脑", ConsoleColor.Red);
                                Windows.Power.Restart();
                                return;// 退出
                            }
                        }
                    }
                    catch (Exception e) {
                        Global.Logger.Error(e.Message, e);
                    }
                    #endregion

                    #region 周期重启内核
                    try {
                        if (IsMining && MinerProfile.IsPeriodicRestartKernel) {
                            if ((DateTime.Now - shareOn).TotalHours > MinerProfile.PeriodicRestartKernelHours) {
                                Global.WriteLine($"每运行{MinerProfile.PeriodicRestartKernelHours}小时重启内核", ConsoleColor.Red);
                                RestartMine();
                                return;// 退出
                            }
                        }
                    }
                    catch (Exception e) {
                        Global.Logger.Error(e.Message, e);
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
                                        Global.WriteLine($"{MinerProfile.NoShareRestartKernelMinutes}分钟收益没有增加重启内核", ConsoleColor.Red);
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
                        Global.Logger.Error(e.Message, e);
                    }
                    #endregion
                });
            #endregion
            #region 启动5秒钟后优化windows环境
            Global.Access<HasBoot5SecondEvent>(
                Guid.Parse("32c49476-8232-4130-bd81-52443ed4ab4e"),
                "启动5秒钟后优化windows环境",
                LogEnum.None,
                action: message => {
                    Task.Factory.StartNew(() => {
                        Windows.Error.DisableWindowsErrorUI();
                        Windows.Firewall.DisableFirewall();
                        Windows.UAC.DisableUAC();
                        Windows.WAU.DisableWAU();
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
                LogEnum.None,
                action: message => {
                    try {
                        foreach (var dir in Directory.GetDirectories(SpecialPath.KernelsDirFullName)) {
                            if (dir != CurrentMineContext.Kernel.GetKernelDirFullName()) {
                                try {
                                    Directory.Delete(Path.Combine(SpecialPath.DaemonFileFullName, dir), recursive: true);
                                }
                                catch (Exception e) {
                                    Global.Logger.Error(e.Message, e);
                                }
                            }
                        }
                    }
                    catch (Exception e) {
                        Global.Logger.Error(e.Message, e);
                    }
                });
            #endregion
            #region 挖矿开始后执行一次过期日志清理工作
            Global.Access<MineStartedEvent>(
                Guid.Parse("d27419f4-7eda-4dbf-bf25-e8c5f1efacb5"),
                "挖矿开始后执行一次过期日志清理工作",
                LogEnum.None,
                action: message => {
                    try {
                        List<string> toRemoves = new List<string>();
                        foreach (var file in Directory.GetFiles(SpecialPath.LogsDirFullName)) {
                            FileInfo fileInfo = new FileInfo(file);
                            if (fileInfo.LastWriteTime.AddDays(7) < DateTime.Now) {
                                toRemoves.Add(file);
                            }
                        }
                        if (toRemoves.Count == 0) {
                            Global.DebugLine("没有过期日志", ConsoleColor.Green);
                        }
                        else {
                            foreach (var item in toRemoves) {
                                File.Delete(item);
                            }
                        }
                    }
                    catch (Exception e) {
                        Global.Logger.Error(e.Message, e);
                    }
                });
            #endregion
            #region 启动10秒钟后自动开始挖矿
            if (this.MinerProfile.IsAutoStart || CommandLineArgs.IsAutoStart) {
                Global.Access<HasBoot10SecondEvent>(
                    Guid.Parse("53004081-ee0f-44a2-b848-95668e63aa8c"),
                    "启动10秒钟后自动开始挖矿",
                    LogEnum.Log,
                    action: (message) => {
                        if ((this.MinerProfile.IsAutoStart || CommandLineArgs.IsAutoStart) && !IsAutoStartCanceled && !this.IsMining) {
                            Execute.OnUIThread(() => {
                                this.StartMine(CommandLineArgs.WorkId);
                            });
                        }
                        if (IsAutoStartCanceled) {
                            Global.DebugLine("自动挖矿已取消", ConsoleColor.Red);
                        }
                    });
            }
            #endregion
            #region 开始挖矿后启动DevConsole
            if (DevMode.IsDevMode) {
                Global.Access<MineStartedEvent>(
                 Guid.Parse("638627D4-31EB-42F9-B92B-31B28D78B792"),
                "开始挖矿后启动DevConsole",
                LogEnum.Log,
                 action: message => {
                     string poolIp = CurrentMineContext.MainCoinPool.GetIp();
                     string consoleTitle = CurrentMineContext.MainCoinPool.Server;
                     Daemon.DaemonUtil.RunDevConsole(poolIp, consoleTitle);
                 });
            }
            #endregion
            #region 开始挖矿后启动NoDevFee
            Global.Access<MineStartedEvent>(
                 Guid.Parse("3362e34f-74a9-4db9-a550-f7779994b459"),
                "开始挖矿后启动NoDevFee",
                LogEnum.Log,
                 action: message => {
                     NTMinerClientDaemon.Instance.Start(null);
                 });
            #endregion
            #region 停止挖矿后停止NoDevFee
            Global.Access<MineStopedEvent>(
                 Guid.Parse("D693A381-0AAD-4768-BC6F-2EEDD9F321BC"),
                "停止挖矿后停止NoDevFee",
                LogEnum.Log,
                 action: message => {
                     NTMinerClientDaemon.Instance.Stop(null);
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
                            NTMinerClientDaemon.Instance.Start(null);
                        }
                    });
            }
            #endregion
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
                Global.Logger.Error(e.Message, e);
            }
            if (_currentMineContext != null) {
                StopMine(wait: false);
            }
        }
        #endregion

        #region StopMine
        public void StopMine(bool wait = true) {
            try {
                KillKernelProcess();
                Global.WriteLine("挖矿停止", ConsoleColor.Red);
                var mineContext = _currentMineContext;
                _currentMineContext = null;
                Global.Happened(new MineStopedEvent(mineContext));
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
            }
        }
        #endregion

        #region RestartMine
        public void RestartMine() {
            IMineContext mineContext = this.CurrentMineContext;
            this.StopMine();
            Global.WriteLine("正在重启", ConsoleColor.Red);
            StartMine(CommandLineArgs.WorkId);
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
                    Global.WriteLine("没有选择主挖币种。", ConsoleColor.Red);
                    return;
                }
                ICoinProfile coinProfile = this.CoinProfileSet.GetCoinProfile(minerProfile.CoinId);
                IPool mainCoinPool;
                if (!this.PoolSet.TryGetPool(coinProfile.PoolId, out mainCoinPool)) {
                    Global.WriteLine("没有选择主币矿池。", ConsoleColor.Red);
                    return;
                }
                ICoinKernel coinKernel;
                if (!this.CoinKernelSet.TryGetKernel(coinProfile.CoinKernelId, out coinKernel)) {
                    Global.WriteLine("没有选择挖矿内核。", ConsoleColor.Red);
                    return;
                }
                IKernel kernel;
                if (!this.KernelSet.TryGetKernel(coinKernel.KernelId, out kernel)) {
                    Global.WriteLine("无效的挖矿内核。", ConsoleColor.Red);
                    return;
                }
                if (!kernel.IsSupported()) {
                    Global.WriteLine($"该内核不支持{GpuSet.GpuType.GetDescription()}卡。", ConsoleColor.Red);
                    return;
                }
                if (string.IsNullOrEmpty(coinProfile.Wallet)) {
                    coinProfile.Wallet = mainCoin.TestWallet;
                }
                if (string.IsNullOrEmpty(coinProfile.Wallet)) {
                    Global.WriteLine("没有填写钱包地址。", ConsoleColor.Red);
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
                        Global.WriteLine("没有选择双挖币种。", ConsoleColor.Red);
                        return;
                    }
                    IPool dualCoinPool;
                    coinProfile = this.CoinProfileSet.GetCoinProfile(coinKernelProfile.DualCoinId);
                    if (!this.PoolSet.TryGetPool(coinProfile.DualCoinPoolId, out dualCoinPool)) {
                        Global.WriteLine("没有选择双挖矿池。", ConsoleColor.Red);
                        return;
                    }
                    if (string.IsNullOrEmpty(coinProfile.DualCoinWallet)) {
                        coinProfile.DualCoinWallet = dualCoin.TestWallet;
                    }
                    if (string.IsNullOrEmpty(coinProfile.DualCoinWallet)) {
                        Global.WriteLine("没有填写双挖钱包。", ConsoleColor.Red);
                        return;
                    }
                    mineContext = this.CreateDualMineContext(mineContext, dualCoin, dualCoinPool, coinProfile.DualCoinWallet, coinKernelProfile.DualCoinWeight);
                }
                if (_currentMineContext != null) {
                    this.StopMine();
                }
                KillKernelProcess();
                if (string.IsNullOrEmpty(kernel.Package)) {
                    Global.WriteLine(kernel.FullName + "没有内核包", ConsoleColor.Red);
                    this.StopMine();
                    return;
                }
                if (string.IsNullOrEmpty(kernel.Args)) {
                    Global.WriteLine(kernel.FullName + "没有配置运行参数", ConsoleColor.Red);
                    return;
                }
                string packageZipFileFullName = Path.Combine(SpecialPath.PackagesDirFullName, kernel.Package);
                if (!File.Exists(packageZipFileFullName)) {
                    Global.WriteLine(kernel.FullName + "本地内核包不存在，触发自动下载", ConsoleColor.Red);
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
                    KillKernelProcess();
                    Task.Factory.StartNew(() => {
                        MinerProcess.CreateProcess(mineContext);
                    });
                }
                if (!string.IsNullOrEmpty(mineContext.Kernel.Notice)) {
                    Global.WriteLine(mineContext.Kernel.Notice, ConsoleColor.Yellow);
                }
                Global.Happened(new MineStartedEvent(mineContext));
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
            }
        }
        #endregion

        #region KillKernelProcess
        private void KillKernelProcess() {
            if (_currentMineContext == null || _currentMineContext.Kernel == null) {
                return;
            }
            string processName = _currentMineContext.Kernel.GetProcessName();
            if (string.IsNullOrEmpty(processName)) {
                return;
            }
            try {
                Windows.TaskKill.Kill(processName);
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
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
            Global.DebugLine($"SetMinerProfileProperty({propertyName}, {value})", ConsoleColor.Yellow);
        }

        public object GetMineWorkProperty(string propertyName) {
            return _minerProfile.GetValue(propertyName);
        }

        public void SetCoinProfileProperty(Guid coinId, string propertyName, object value) {
            this.CoinProfileSet.SetCoinProfileProperty(coinId, propertyName, value);
            string coinCode = "意外的币种";
            ICoin coin;
            if (this.CoinSet.TryGetCoin(coinId, out coin)) {
                coinCode = coin.Code;
            }
            Global.DebugLine($"SetMinerProfileProperty({coinCode}, {propertyName}, {value})", ConsoleColor.Yellow);
        }

        public void SetCoinKernelProfileProperty(Guid coinKernelId, string propertyName, object value) {
            this.CoinKernelProfileSet.SetCoinKernelProfileProperty(coinKernelId, propertyName, value);
            string coinCode = "意外的币种";
            string kernelName = "意外的内核";
            ICoinKernel coinKernel;
            if (this.CoinKernelSet.TryGetKernel(coinKernelId, out coinKernel)) {
                ICoin coin;
                if (this.CoinSet.TryGetCoin(coinKernel.CoinId, out coin)) {
                    coinCode = coin.Code;
                }
                IKernel kernel;
                if (this.KernelSet.TryGetKernel(coinKernel.KernelId, out kernel)) {
                    kernelName = kernel.FullName;
                }
            }
            Global.DebugLine($"SetCoinKernelProfileProperty({coinCode}, {kernelName}, {propertyName}, {value})", ConsoleColor.Yellow);
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

        public int GpuAllId {
            get { return -1; }
        }

        public int SpeedHistoryLengthByMinute {
            get {
                return 10;
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
                        Global.Logger.Error(ex);
                    }
                    if (_gpuSet == null || _gpuSet.Count == 0) {
                        try {
                            _gpuSet = new AMDGpuSet(this);
                        }
                        catch (Exception ex) {
                            _gpuSet = EmptyGpuSet.Instance;
                            Global.Logger.Error(ex);
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

        public IKernelSet KernelSet { get; private set; }

        public IKernelProfileSet KernelProfileSet { get; private set; }

        public IGpusSpeed GpusSpeed { get; private set; }

        public ICoinShareSet CoinShareSet { get; private set; }

        public IKernelOutputFilterSet KernelOutputFilterSet { get; private set; }

        public IKernelOutputTranslaterSet KernelOutputTranslaterSet { get; private set; }

        public ICoinProfileSet CoinProfileSet { get; private set; }

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
