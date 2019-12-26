using NTMiner.Core;
using NTMiner.Core.Gpus;
using NTMiner.Core.Kernels;
using NTMiner.Hub;
using NTMiner.MinerClient;
using NTMiner.Profile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class NTMinerRoot : INTMinerRoot {
        #region CreateMineContext
        public IMineContext CreateMineContext() {
            if (!GetProfileData(out ICoin mainCoin, out ICoinProfile mainCoinProfile, out IPool mainCoinPool, out ICoinKernel mainCoinKernel, out IKernel kernel,
                out IKernelInput kernelInput, out IKernelOutput kernelOutput, out string _)) {
                return null;
            }
            if (!kernel.IsSupported(mainCoin)) {
                return null;
            }
            ICoinKernelProfile coinKernelProfile = this.MinerProfile.GetCoinKernelProfile(mainCoinProfile.CoinKernelId);
            string poolKernelArgs = string.Empty;
            IPoolKernel poolKernel = ServerContext.PoolKernelSet.AsEnumerable().FirstOrDefault(a => a.PoolId == mainCoinPool.GetId() && a.KernelId == kernel.GetId());
            if (poolKernel != null) {
                poolKernelArgs = poolKernel.Args;
            }
            string kernelArgs = kernelInput.Args;
            string coinKernelArgs = mainCoinKernel.Args;
            string customArgs = coinKernelProfile.CustomArgs ?? string.Empty;
            var parameters = new Dictionary<string, string>();
            var fileWriters = new Dictionary<Guid, string>();
            var fragments = new Dictionary<Guid, string>();
            parameters.Add(NTKeyword.MainCoinParameterName, mainCoin.Code);
            string userName = string.Empty;
            string password = NTKeyword.PasswordDefaultValue;
            string wallet = mainCoinProfile.Wallet;
            if (mainCoinPool.IsUserMode) {
                IPoolProfile poolProfile = MinerProfile.GetPoolProfile(mainCoinPool.GetId());
                password = poolProfile.Password;
                if (string.IsNullOrEmpty(password)) {
                    password = NTKeyword.PasswordDefaultValue;
                }
                userName = poolProfile.UserName;
                wallet = poolProfile.UserName;
            }
            else {
                userName = wallet;
            }
            parameters.Add(NTKeyword.UserNameParameterName, userName);
            parameters.Add(NTKeyword.PasswordParameterName, password);
            parameters.Add(NTKeyword.WalletParameterName, wallet);
            parameters.Add(NTKeyword.HostParameterName, mainCoinPool.GetHost());
            parameters.Add(NTKeyword.PortParameterName, mainCoinPool.GetPort().ToString());
            parameters.Add(NTKeyword.PoolParameterName, mainCoinPool.Server);
            string minerName = $"{mainCoinPool.MinerNamePrefix}{this.MinerProfile.MinerName}{mainCoinPool.MinerNamePostfix}";
            parameters.Add(NTKeyword.WorkerParameterName, minerName);
            if (mainCoinKernel.IsSupportPool1 && !mainCoinPool.NoPool1) {
                parameters.Add(NTKeyword.Worker1ParameterName, minerName);
                if (ServerContext.PoolSet.TryGetPool(mainCoinProfile.PoolId1, out IPool mainCoinPool1)) {
                    parameters.Add(NTKeyword.Host1ParameterName, mainCoinPool1.GetHost());
                    parameters.Add(NTKeyword.Port1ParameterName, mainCoinPool1.GetPort().ToString());
                    parameters.Add(NTKeyword.Pool1ParameterName, mainCoinPool1.Server);
                    if (mainCoinPool1.IsUserMode) {
                        IPoolProfile poolProfile1 = MinerProfile.GetPoolProfile(mainCoinPool1.GetId());
                        string password1 = poolProfile1.Password;
                        if (string.IsNullOrEmpty(password1)) {
                            password1 = NTKeyword.PasswordDefaultValue;
                        }
                        parameters.Add(NTKeyword.UserName1ParameterName, poolProfile1.UserName);
                        parameters.Add(NTKeyword.Password1ParameterName, password1);
                    }
                    else {
                        parameters.Add(NTKeyword.Wallet1ParameterName, mainCoinProfile.Wallet);
                    }
                }
            }
            string devicesArgs = GetDevicesArgs(kernelInput);
            // 这里不要考虑{logfile}，{logfile}往后推迟
            if (coinKernelProfile.IsDualCoinEnabled && kernelInput.IsSupportDualMine) {
                Guid dualCoinGroupId = mainCoinKernel.DualCoinGroupId;
                if (dualCoinGroupId != Guid.Empty) {
                    if (this.ServerContext.CoinSet.TryGetCoin(coinKernelProfile.DualCoinId, out ICoin dualCoin)) {
                        ICoinProfile dualCoinProfile = this.MinerProfile.GetCoinProfile(dualCoin.GetId());
                        if (ServerContext.PoolSet.TryGetPool(dualCoinProfile.DualCoinPoolId, out IPool dualCoinPool)) {
                            string dualUserName = string.Empty;
                            string dualPassword = NTKeyword.PasswordDefaultValue;
                            string dualWallet = dualCoinProfile.DualCoinWallet;
                            parameters.Add(NTKeyword.DualCoinParameterName, dualCoin.Code);
                            if (dualCoinPool.IsUserMode) {
                                IPoolProfile dualPoolProfile = MinerProfile.GetPoolProfile(dualCoinPool.GetId());
                                dualPassword = dualPoolProfile.Password;
                                if (string.IsNullOrEmpty(dualPassword)) {
                                    dualPassword = NTKeyword.PasswordDefaultValue;
                                }
                                dualUserName = dualPoolProfile.UserName;
                                dualWallet = dualPoolProfile.UserName;
                            }
                            else {
                                dualUserName = dualWallet;
                            }
                            parameters.Add(NTKeyword.DualUserNameParameterName, dualUserName);
                            parameters.Add(NTKeyword.DualPasswordParameterName, dualPassword);
                            parameters.Add(NTKeyword.DualWalletParameterName, dualWallet);
                            parameters.Add(NTKeyword.DualHostParameterName, dualCoinPool.GetHost());
                            parameters.Add(NTKeyword.DualPortParameterName, dualCoinPool.GetPort().ToString());
                            parameters.Add(NTKeyword.DualPoolParameterName, dualCoinPool.Server);

                            kernelArgs = mainCoinKernel.DualFullArgs;
                            AssembleArgs(parameters, ref kernelArgs, isDual: true);
                            AssembleArgs(parameters, ref poolKernelArgs, isDual: true);
                            AssembleArgs(parameters, ref customArgs, isDual: true);

                            string dualWeightArg;
                            if (!string.IsNullOrEmpty(kernelInput.DualWeightArg)) {
                                if (coinKernelProfile.IsAutoDualWeight && kernelInput.IsAutoDualWeight) {
                                    dualWeightArg = string.Empty;
                                }
                                else {
                                    dualWeightArg = $"{kernelInput.DualWeightArg} {Convert.ToInt32(coinKernelProfile.DualCoinWeight).ToString()}";
                                }
                            }
                            else {
                                dualWeightArg = string.Empty;
                            }
                            StringBuilder dualSb = new StringBuilder();
                            dualSb.Append(kernelArgs);
                            if (!string.IsNullOrEmpty(dualWeightArg)) {
                                dualSb.Append(" ").Append(dualWeightArg);
                            }
                            if (!string.IsNullOrEmpty(poolKernelArgs)) {
                                dualSb.Append(" ").Append(poolKernelArgs);
                            }
                            BuildFragments(mainCoinKernel, parameters, out fileWriters, out fragments);
                            foreach (var fragment in fragments.Values) {
                                dualSb.Append(" ").Append(fragment);
                            }
                            if (!string.IsNullOrEmpty(customArgs)) {
                                dualSb.Append(" ").Append(customArgs);
                            }
                            if (!string.IsNullOrEmpty(devicesArgs)) {
                                dualSb.Append(" ").Append(devicesArgs);
                            }

                            // 注意：这里退出
                            return new DualMineContext(
                                new MineContext(
                                    this.MinerProfile.MinerName,
                                    mainCoin,
                                    mainCoinPool,
                                    kernel,
                                    kernelInput,
                                    kernelOutput,
                                    mainCoinKernel,
                                    wallet,
                                    dualSb.ToString(),
                                    parameters,
                                    fragments,
                                    fileWriters,
                                    GpuSet.GetUseDevices()),
                                dualCoin,
                                dualCoinPool,
                                dualWallet,
                                coinKernelProfile.DualCoinWeight,
                                parameters,
                                fragments,
                                fileWriters,
                                GpuSet.GetUseDevices());
                        }
                    }
                }
            }
            AssembleArgs(parameters, ref kernelArgs, isDual: false);
            AssembleArgs(parameters, ref coinKernelArgs, isDual: false);
            AssembleArgs(parameters, ref poolKernelArgs, isDual: false);
            AssembleArgs(parameters, ref customArgs, isDual: false);
            StringBuilder sb = new StringBuilder();
            sb.Append(kernelArgs);
            if (!string.IsNullOrEmpty(coinKernelArgs)) {
                sb.Append(" ").Append(coinKernelArgs);
            }
            if (!string.IsNullOrEmpty(poolKernelArgs)) {
                sb.Append(" ").Append(poolKernelArgs);
            }
            if (!string.IsNullOrEmpty(devicesArgs)) {
                sb.Append(" ").Append(devicesArgs);
            }
            BuildFragments(mainCoinKernel, parameters, out fileWriters, out fragments);
            foreach (var fragment in fragments.Values) {
                sb.Append(" ").Append(fragment);
            }
            if (!string.IsNullOrEmpty(customArgs)) {
                sb.Append(" ").Append(customArgs);
            }

            return new MineContext(
                this.MinerProfile.MinerName,
                mainCoin,
                mainCoinPool,
                kernel,
                kernelInput,
                kernelOutput,
                mainCoinKernel,
                wallet,
                sb.ToString(),
                parameters,
                fragments,
                fileWriters,
                GpuSet.GetUseDevices());
        }

        private string GetDevicesArgs(IKernelInput kernelInput) {
            string devicesArgs = string.Empty;
            if (!string.IsNullOrWhiteSpace(kernelInput.DevicesArg)) {
                int[] useDevices = this.GpuSet.GetUseDevices();
                if ((useDevices.Length != 0 && useDevices.Length != GpuSet.Count) || kernelInput.IsDeviceAllNotEqualsNone) {
                    string separator = kernelInput.DevicesSeparator;
                    // 因为空格在界面上不易被人读取所以以关键字代替空格
                    if (kernelInput.DevicesSeparator == NTKeyword.SpaceKeyword) {
                        separator = " ";
                    }
                    List<string> gpuIndexes = new List<string>();
                    foreach (var index in useDevices) {
                        int i = index;
                        if (kernelInput.DeviceBaseIndex != 0) {
                            i = index + kernelInput.DeviceBaseIndex;
                        }
                        string nText = NTKeyword.GetIndexChar(i, separator);
                        gpuIndexes.Add(nText);
                    }
                    switch (GpuSet.GpuType) {
                        case GpuType.Empty:
                            break;
                        case GpuType.NVIDIA:
                            devicesArgs = $"{kernelInput.DevicesArg} {string.Join(separator, gpuIndexes.Select(a => $"{kernelInput.NDevicePrefix}{a}{kernelInput.NDevicePostfix}"))}";
                            break;
                        case GpuType.AMD:
                            devicesArgs = $"{kernelInput.DevicesArg} {string.Join(separator, gpuIndexes.Select(a => $"{kernelInput.ADevicePrefix}{a}{kernelInput.ADevicePostfix}"))}";
                            break;
                        default:
                            break;
                    }
                }
            }
            return devicesArgs;
        }

        private static void AssembleArgs(Dictionary<string, string> prms, ref string args, bool isDual) {
            if (string.IsNullOrEmpty(args)) {
                args = string.Empty;
                return;
            }
            args = args.Replace("{" + NTKeyword.MainCoinParameterName + "}", prms[NTKeyword.MainCoinParameterName]);
            if (prms.ContainsKey(NTKeyword.WalletParameterName)) {
                args = args.Replace("{" + NTKeyword.WalletParameterName + "}", prms[NTKeyword.WalletParameterName]);
            }
            if (prms.ContainsKey(NTKeyword.UserNameParameterName)) {
                args = args.Replace("{" + NTKeyword.UserNameParameterName + "}", prms[NTKeyword.UserNameParameterName]);
            }
            if (prms.ContainsKey(NTKeyword.PasswordParameterName)) {
                args = args.Replace("{" + NTKeyword.PasswordParameterName + "}", prms[NTKeyword.PasswordParameterName]);
            }
            args = args.Replace("{" + NTKeyword.HostParameterName + "}", prms[NTKeyword.HostParameterName]);
            args = args.Replace("{" + NTKeyword.PortParameterName + "}", prms[NTKeyword.PortParameterName]);
            args = args.Replace("{" + NTKeyword.PoolParameterName + "}", prms[NTKeyword.PoolParameterName]);
            args = args.Replace("{" + NTKeyword.WorkerParameterName + "}", prms[NTKeyword.WorkerParameterName]);
            if (isDual) {
                args = args.Replace("{" + NTKeyword.DualCoinParameterName + "}", prms[NTKeyword.DualCoinParameterName]);
                args = args.Replace("{" + NTKeyword.DualWalletParameterName + "}", prms[NTKeyword.DualWalletParameterName]);
                args = args.Replace("{" + NTKeyword.DualUserNameParameterName + "}", prms[NTKeyword.DualUserNameParameterName]);
                args = args.Replace("{" + NTKeyword.DualPasswordParameterName + "}", prms[NTKeyword.DualPasswordParameterName]);
                args = args.Replace("{" + NTKeyword.DualHostParameterName + "}", prms[NTKeyword.DualHostParameterName]);
                args = args.Replace("{" + NTKeyword.DualPortParameterName + "}", prms[NTKeyword.DualPortParameterName]);
                args = args.Replace("{" + NTKeyword.DualPoolParameterName + "}", prms[NTKeyword.DualPoolParameterName]);
            }
            // 这里不要考虑{logfile}，{logfile}往后推迟
        }

        private void BuildFragments(ICoinKernel coinKernel, Dictionary<string, string> parameters, out Dictionary<Guid, string> fileWriters, out Dictionary<Guid, string> fragments) {
            fileWriters = new Dictionary<Guid, string>();
            fragments = new Dictionary<Guid, string>();
            try {
                if (coinKernel.FragmentWriterIds != null && coinKernel.FragmentWriterIds.Count != 0) {
                    foreach (var writerId in coinKernel.FragmentWriterIds) {
                        if (ServerContext.FragmentWriterSet.TryGetFragmentWriter(writerId, out IFragmentWriter writer)) {
                            BuildFragment(parameters, fileWriters, fragments, writer);
                        }
                    }
                }
                if (coinKernel.FileWriterIds != null && coinKernel.FileWriterIds.Count != 0) {
                    foreach (var writerId in coinKernel.FileWriterIds) {
                        if (ServerContext.FileWriterSet.TryGetFileWriter(writerId, out IFileWriter writer)) {
                            BuildFragment(parameters, fileWriters, fragments, writer);
                        }
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private class ParameterNames {
            // 根据这个判断是否换成过期
            internal string Body = string.Empty;
            internal readonly HashSet<string> Names = new HashSet<string>();
        }

        private static readonly Dictionary<Guid, ParameterNames> _parameterNameDic = new Dictionary<Guid, ParameterNames>();
        private static readonly object _locker = new object();
        private static ParameterNames GetParameterNames(IFragmentWriter writer) {
            if (string.IsNullOrEmpty(writer.Body)) {
                return new ParameterNames {
                    Body = writer.Body
                };
            }
            Guid writerId = writer.GetId();
            if (_parameterNameDic.TryGetValue(writerId, out ParameterNames parameterNames) && parameterNames.Body == writer.Body) {
                return parameterNames;
            }
            else {
                lock (_locker) {
                    if (_parameterNameDic.TryGetValue(writerId, out parameterNames) && parameterNames.Body == writer.Body) {
                        return parameterNames;
                    }
                    if (parameterNames != null) {
                        parameterNames.Body = writer.Body;
                    }
                    else {
                        parameterNames = new ParameterNames {
                            Body = writer.Body
                        };
                        _parameterNameDic.Add(writerId, parameterNames);
                    }
                    parameterNames.Names.Clear();
                    const string pattern = @"\{(\w+)\}";
                    var matches = Regex.Matches(writer.Body, pattern);
                    foreach (Match match in matches) {
                        parameterNames.Names.Add(match.Groups[1].Value);
                    }
                    return parameterNames;
                }
            }
        }

        private static bool IsMatch(IFragmentWriter writer, Dictionary<string, string> parameters, out ParameterNames parameterNames) {
            parameterNames = GetParameterNames(writer);
            if (string.IsNullOrEmpty(writer.Body)) {
                return false;
            }
            if (parameterNames.Names.Count == 0) {
                return true;
            }
            foreach (var name in parameterNames.Names) {
                if (!parameters.ContainsKey(name)) {
                    return false;
                }
            }
            return true;
        }

        private static void BuildFragment(Dictionary<string, string> parameters, Dictionary<Guid, string> fileWriters, Dictionary<Guid, string> fragments, IFragmentWriter writer) {
            try {
                if (!IsMatch(writer, parameters, out ParameterNames parameterNames)) {
                    return;
                }
                string content = writer.Body;
                foreach (var parameterName in parameterNames.Names) {
                    content = content.Replace($"{{{parameterName}}}", parameters[parameterName]);
                }
                if (writer is IFileWriter) {
                    if (fileWriters.ContainsKey(writer.GetId())) {
                        fileWriters[writer.GetId()] = content;
                    }
                    else {
                        fileWriters.Add(writer.GetId(), content);
                    }
                }
                else {
                    if (fragments.ContainsKey(writer.GetId())) {
                        fragments[writer.GetId()] = content;
                    }
                    else {
                        fragments.Add(writer.GetId(), content);
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
        #endregion

        private class MineContext : IMineContext {
            private event Action OnKill;
            private readonly List<IMessagePathId> _contextPathIds = new List<IMessagePathId>();
            public MineContext(
                string minerName,
                ICoin mainCoin,
                IPool mainCoinPool,
                IKernel kernel,
                IKernelInput kernelInput,
                IKernelOutput kernelOutput,
                ICoinKernel coinKernel,
                string mainCoinWallet,
                string commandLine,
                Dictionary<string, string> parameters,
                Dictionary<Guid, string> fragments,
                Dictionary<Guid, string> fileWriters,
                int[] useDevices) {
                this.Fragments = fragments;
                this.FileWriters = fileWriters;
                this.Id = Guid.NewGuid();
                this.MinerName = minerName;
                this.MainCoin = mainCoin;
                this.MainCoinPool = mainCoinPool;
                this.Kernel = kernel;
                this.CoinKernel = coinKernel;
                this.MainCoinWallet = mainCoinWallet;
                this.AutoRestartKernelCount = 0;
                this.KernelSelfRestartCount = 0;
                this.CommandLine = commandLine ?? string.Empty;
                this.MineStartedOn = DateTime.MinValue;
                this.Parameters = parameters;
                this.UseDevices = useDevices;
                this.KernelInput = kernelInput;
                this.KernelOutput = kernelOutput;

                this.NewLogFileName();
            }

            private void NewLogFileName() {
                string logFileName;
                if (this.CommandLine.Contains(NTKeyword.LogFileParameterName)) {
                    this.KernelProcessType = KernelProcessType.Logfile;
                    logFileName = $"{this.Kernel.Code}_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_fff")}.log";
                }
                else {
                    this.KernelProcessType = KernelProcessType.Pip;
                    logFileName = $"{this.Kernel.Code}_pip_{DateTime.Now.Ticks.ToString()}.log";
                }
                this.LogFileFullName = Path.Combine(SpecialPath.LogsDirFullName, logFileName);
            }

            public void Start(bool isRestart) {
                Kill();
                if (isRestart) {
                    NewLogFileName();
                    this.IsRestart = true;
                }
                else {
                    AddOnecePath<MineStopedEvent>("挖矿停止后关闭非托管的日志句柄", LogEnum.DevConsole,
                        action: message => {
                            message.MineContext?.Close();
                        }, location: this.GetType(), pathId: Guid.Empty);
                }
                this.MineStartedOn = DateTime.Now;
                CreateProcessAsync();
            }

            private bool _isClosed = false;
            public void Close() {
                if (!_isClosed) {
                    _isClosed = true;
                    foreach (var pathId in _contextPathIds) {
                        VirtualRoot.RemoveMessagePath(pathId);
                    }
                    _contextPathIds.Clear();
                    Kill();
                }
            }

            public bool IsClosed {
                get {
                    return _isClosed;
                }
            }

            public Guid Id { get; private set; }

            public bool IsRestart { get; set; }

            public string MinerName { get; private set; }

            public ICoin MainCoin { get; private set; }

            public IPool MainCoinPool { get; private set; }

            public IKernel Kernel { get; private set; }

            public ICoinKernel CoinKernel { get; private set; }

            public string MainCoinWallet { get; private set; }

            public int AutoRestartKernelCount { get; set; }

            public int KernelSelfRestartCount { get; set; }

            public string LogFileFullName { get; private set; }

            public KernelProcessType KernelProcessType { get; private set; }

            public string CommandLine { get; private set; }

            public DateTime MineStartedOn { get; private set; }

            public Dictionary<string, string> Parameters { get; private set; }

            public Dictionary<Guid, string> Fragments { get; private set; }

            public Dictionary<Guid, string> FileWriters { get; private set; }

            public int[] UseDevices { get; private set; }

            public IKernelInput KernelInput { get; private set; }

            public IKernelOutput KernelOutput { get; private set; }

            public Process KernelProcess { get; set; }

            /// <summary>
            /// 事件响应
            /// </summary>
            private void AddEventPath<TEvent>(string description, LogEnum logType, Action<TEvent> action, Type location)
                where TEvent : IEvent {
                var messagePathId = VirtualRoot.AddMessagePath(description, logType, action, location);
                _contextPathIds.Add(messagePathId);
            }

            private void AddOnecePath<TMessage>(string description, LogEnum logType, Action<TMessage> action, Guid pathId, Type location) {
                var messagePathId = VirtualRoot.AddOnecePath(description, logType, action, pathId, location);
                _contextPathIds.Add(messagePathId);
            }

            #region Kill
            private void Kill() {
                if (Kernel != null) {
                    try {
                        string processName = Kernel.GetProcessName();
                        Windows.TaskKill.Kill(processName, waitForExit: true);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                    finally {
                        KernelProcess?.Dispose();
                        KernelProcess = null;
                        OnKill?.Invoke();
                    }
                }
            }
            #endregion

            #region CreateProcessAsync
            private static readonly object _locker = new object();
            private void CreateProcessAsync() {
                Task.Factory.StartNew(() => {
                    lock (_locker) {
                        try {
#if DEBUG
                            NTStopwatch.Start();
#endif
                            // 清理除当前外的Temp/Kernel
                            Cleaner.Instance.Clear();
#if DEBUG
                            var elapsedMilliseconds = NTStopwatch.Stop();
                            if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                                Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.{nameof(CreateProcessAsync)}[{nameof(Cleaner)}.{nameof(Cleaner.Clear)}]");
                            }
#endif
                            Write.UserOk("场地打扫完毕");
                            // 应用超频
                            if (Instance.GpuProfileSet.IsOverClockEnabled(MainCoin.GetId())) {
                                Write.UserWarn("应用超频，如果CPU性能较差耗时可能超过1分钟，请耐心等待");
                                var cmd = new CoinOverClockCommand(coinId: MainCoin.GetId());
                                AddOnecePath<CoinOverClockDoneEvent>("超频完成后继续流程", LogEnum.DevConsole,
                                    message => {
                                        // pathId是唯一的，从而可以断定该消息一定是因为该命令而引发的
                                        ContinueCreateProcess();
                                    }, location: this.GetType(), pathId: cmd.Id);
                                // 超频是在另一个线程执行的，因为N卡超频当cpu性能非常差时较耗时
                                VirtualRoot.Execute(cmd);
                            }
                            else {
                                ContinueCreateProcess();
                            }
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e);
                            Write.UserFail("挖矿内核启动失败，请联系开发人员解决");
                        }
                    }
                });
            }
            #endregion

            #region ContinueCreateProcess
            private void ContinueCreateProcess() {
                Thread.Sleep(1000);
                if (this != Instance.LockedMineContext) {
                    Write.UserWarn("挖矿停止");
                    return;
                }
                // 解压内核包
                if (!this.Kernel.ExtractPackage()) {
                    VirtualRoot.RaiseEvent(new StartingMineFailedEvent("内核解压失败，请卸载内核重试。"));
                }
                else {
                    Write.UserOk("内核包解压成功");
                }

                // 执行文件书写器
                this.ExecuteFileWriters();

                // 分离命令名和参数
                GetCmdNameAndArguments(out string kernelExeFileFullName, out string arguments);
                // 这是不应该发生的，如果发生很可能是填写命令的时候拼写错误了
                if (!File.Exists(kernelExeFileFullName)) {
                    Write.UserError(kernelExeFileFullName + "文件不存在，可能是被杀软删除导致，请退出杀毒软件重试或者QQ群联系小编，解释：大部分挖矿内核会报毒，不是开源矿工的问题也不是杀软的问题，也不是挖矿内核的问题，是挖矿这件事情的问题，可能是挖矿符合了病毒的定义。");
                }
                if (this.KernelProcessType == KernelProcessType.Logfile) {
                    arguments = arguments.Replace(NTKeyword.LogFileParameterName, this.LogFileFullName);
                }
                Write.UserOk($"\"{kernelExeFileFullName}\" {arguments}");
                Write.UserInfo($"有请内核上场");
                if (this != Instance.LockedMineContext) {
                    Write.UserWarn("挖矿停止");
                    return;
                }
                switch (this.KernelProcessType) {
                    case KernelProcessType.Logfile:
                        CreateLogfileProcess(kernelExeFileFullName, arguments);
                        break;
                    case KernelProcessType.Pip:
                        CreatePipProcess(kernelExeFileFullName, arguments);
                        break;
                    default:
                        throw new InvalidProgramException();
                }
                KernelProcessDaemon();
                VirtualRoot.RaiseEvent(new MineStartedEvent(this));
            }
            #endregion

            #region GetCmdNameAndArguments
            private void GetCmdNameAndArguments(out string kernelExeFileFullName, out string arguments) {
                var kernel = this.Kernel;
                if (string.IsNullOrEmpty(kernel.Package)) {
                    throw new InvalidDataException();
                }
                string kernelDir = Path.Combine(SpecialPath.KernelsDirFullName, Path.GetFileNameWithoutExtension(kernel.Package));
                string kernelCommandName = kernel.GetCommandName();
                kernelExeFileFullName = Path.Combine(kernelDir, kernelCommandName);
                if (!kernelExeFileFullName.EndsWith(".exe")) {
                    kernelExeFileFullName += ".exe";
                }
                var args = this.CommandLine;
                arguments = args.Substring(kernelCommandName.Length).Trim();
            }
            #endregion

            #region KernelProcessDaemon
            private void KernelProcessDaemon() {
                if (this.IsRestart) {
                    return;
                }
                string processName = this.Kernel.GetProcessName();
                this.AddEventPath<Per1MinuteEvent>("周期性检查挖矿内核是否消失，如果消失尝试重启", LogEnum.DevConsole,
                    action: message => {
                        if (this == Instance.LockedMineContext) {
                            if (!string.IsNullOrEmpty(processName)) {
                                Process[] processes = Process.GetProcessesByName(processName);
                                if (processes.Length == 0) {
                                    this.AutoRestartKernelCount += 1;
                                    VirtualRoot.ThisLocalWarn(nameof(NTMinerRoot), processName + $"挖矿内核进程消失", toConsole: true);
                                    if (Instance.MinerProfile.IsAutoRestartKernel && this.AutoRestartKernelCount <= Instance.MinerProfile.AutoRestartKernelTimes) {
                                        VirtualRoot.ThisLocalInfo(nameof(NTMinerRoot), $"尝试第{this.AutoRestartKernelCount.ToString()}次重启，共{Instance.MinerProfile.AutoRestartKernelTimes.ToString()}次", toConsole: true);
                                        Instance.RestartMine();
                                    }
                                    else {
                                        Instance.StopMineAsync(StopMineReason.KernelProcessLost);
                                    }
                                }
                            }
                        }
                        else {
                            this.Close();
                        }
                    }, location: this.GetType());
            }
            #endregion

            #region CreateLogfileProcess
            private void CreateLogfileProcess(string kernelExeFileFullName, string arguments) {
                ProcessStartInfo startInfo = new ProcessStartInfo(kernelExeFileFullName, arguments) {
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    WorkingDirectory = EntryAssemblyInfo.TempDirFullName
                };
                // 追加环境变量
                foreach (var item in this.CoinKernel.EnvironmentVariables) {
                    startInfo.EnvironmentVariables.Add(item.Key, item.Value);
                }
                this.KernelProcess = new Process {
                    StartInfo = startInfo
                };
                this.KernelProcess.Start();
                ReadPrintLoopLogFileAsync(isWriteToConsole: false);
            }
            #endregion

            #region CreatePipProcess
            // 创建管道，将输出通过管道转送到日志文件，然后读取日志文件内容打印到控制台
            private void CreatePipProcess(string kernelExeFileFullName, string arguments) {
                SECURITY_ATTRIBUTES saAttr = new SECURITY_ATTRIBUTES {
                    bInheritHandle = true,
                    lpSecurityDescriptor = IntPtr.Zero,
                    length = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES))
                };

                //set the bInheritHandle flag so pipe handles are inherited

                saAttr.lpSecurityDescriptor = IntPtr.Zero;
                //get handle to current stdOut

                IntPtr mypointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(STARTUPINFO)));
                Marshal.StructureToPtr(saAttr, mypointer, true);
                var bret = CreatePipe(out IntPtr hReadOut, out IntPtr hWriteOut, mypointer, 0);
                if (!bret) {
                    int lasterr = Marshal.GetLastWin32Error();
                    VirtualRoot.RaiseEvent(new StartingMineFailedEvent($"管道型进程创建失败 lasterr:{lasterr.ToString()}"));
                    return;
                }
                this.OnKill += () => {
                    try {
                        CloseHandle(hWriteOut);
                    }
                    catch {
                    }
                };

                const uint STARTF_USESHOWWINDOW = 0x00000001;
                const uint STARTF_USESTDHANDLES = 0x00000100;
                const uint NORMAL_PRIORITY_CLASS = 0x00000020;
                //const short SW_SHOW = 5;
                const short SW_HIDE = 0;
                const int HANDLE_FLAG_INHERIT = 1;

                //ensure the read handle to pipe for stdout is not inherited
                SetHandleInformation(hReadOut, HANDLE_FLAG_INHERIT, 0);
                ////Create pipe for the child process's STDIN
                STARTUPINFO lpStartupInfo = new STARTUPINFO {
                    cb = (uint)Marshal.SizeOf(typeof(STARTUPINFO)),
                    dwFlags = STARTF_USESTDHANDLES | STARTF_USESHOWWINDOW,
                    wShowWindow = SW_HIDE, // SW_HIDE; //SW_SHOW
                    hStdOutput = hWriteOut,
                    hStdError = hWriteOut,
                    hStdInput = IntPtr.Zero
                };
                StringBuilder lpEnvironment = new StringBuilder();
                // 复制父进程的环境变量
                IDictionary dic = Environment.GetEnvironmentVariables();
                // 追加环境变量
                foreach (var item in this.CoinKernel.EnvironmentVariables) {
                    dic.Add(item.Key, item.Value);
                }
                foreach (var key in dic.Keys) {
                    if (key == null || key.ToString().Contains("\0")) {
                        continue;
                    }
                    var value = dic[key];
                    if (value == null || value.ToString().Contains("\0")) {
                        continue;
                    }
                    lpEnvironment.Append($"{key.ToString()}={value.ToString()}\0");
                }
                if (CreateProcess(
                        lpApplicationName: null,
                        lpCommandLine: new StringBuilder($"\"{kernelExeFileFullName}\" {arguments}"),
                        lpProcessAttributes: IntPtr.Zero,
                        lpThreadAttributes: IntPtr.Zero,
                        bInheritHandles: true,
                        dwCreationFlags: NORMAL_PRIORITY_CLASS,
                        lpEnvironment: lpEnvironment,
                        lpCurrentDirectory: Path.GetDirectoryName(kernelExeFileFullName),
                        lpStartupInfo: ref lpStartupInfo,
                        lpProcessInformation: out PROCESS_INFORMATION processInfo)) {

                    try {
                        this.KernelProcess = Process.GetProcessById((int)processInfo.dwProcessId);
                    }
                    catch {
                        this.Close();
                        VirtualRoot.RaiseEvent(new StartingMineFailedEvent($"内核已退出"));
                        return;
                    }

                    Task.Factory.StartNew(() => {
                        using (FileStream fs = new FileStream(this.LogFileFullName, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                            const byte r = (byte)'\r';
                            byte[] buffer = new byte[1024];
                            int ret;
                            // Read会阻塞，直到读取到字符或者hWriteOut被关闭
                            while ((ret = Read(buffer, 0, buffer.Length, hReadOut)) > 0) {
                                byte[] data = new byte[ret];
                                int n = 0;
                                for (int i = 0; i < ret; i++) {
                                    if (buffer[i] != r) {
                                        data[n] = buffer[i];
                                        n++;
                                    }
                                }
                                fs.Write(data, 0, n);
                                fs.Flush();
                            }
                        }
                        CloseHandle(hReadOut);
                    }, TaskCreationOptions.LongRunning);
                    ReadPrintLoopLogFileAsync(isWriteToConsole: true);
                }
                else {
                    VirtualRoot.RaiseEvent(new StartingMineFailedEvent($"内核启动失败，请重试"));
                }
            }

            private static unsafe int Read(byte[] buffer, int index, int count, IntPtr hStdOut) {
                int n = 0;
                fixed (byte* p = buffer) {
                    if (!ReadFile(hStdOut, p + index, count, &n, (NativeOverlapped*)0))
                        return 0;
                }
                return n;
            }
            #endregion

            #region ReadPrintLoopLogFile
            private void ReadPrintLoopLogFileAsync(bool isWriteToConsole) {
                Task.Factory.StartNew(() => {
                    bool isLogFileCreated = true;
                    int n = 0;
                    string logFileFullName = this.LogFileFullName;
                    while (!File.Exists(logFileFullName)) {
                        if (n >= 20) {
                            // 20秒钟都没有建立日志文件，不可能
                            isLogFileCreated = false;
                            Write.UserFail("呃！意外，竟然20秒钟未产生内核输出。常见原因：1.挖矿内核被杀毒软件删除; 2.没有磁盘空间了; 3.反馈给开发人员");
                            break;
                        }
                        Thread.Sleep(1000);
                        if (n == 0) {
                            Write.UserInfo("等待内核出场");
                        }
                        if (logFileFullName != Instance.LockedMineContext.LogFileFullName) {
                            Write.UserWarn("结束内核输出等待。");
                            isLogFileCreated = false;
                            break;
                        }
                        n++;
                    }
                    if (isLogFileCreated) {
                        StreamReader sreader = null;
                        try {
                            DateTime _kernelRestartKeywordOn = DateTime.MinValue;
                            sreader = new StreamReader(File.Open(logFileFullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), Encoding.Default);
                            while (Instance.LockedMineContext != null && logFileFullName == Instance.LockedMineContext.LogFileFullName) {
                                string outline = sreader.ReadLine();
                                if (string.IsNullOrEmpty(outline) && sreader.EndOfStream) {
                                    Thread.Sleep(1000);
                                }
                                else {
                                    string input = outline;
                                    Guid kernelOutputId = Guid.Empty;
                                    if (this.KernelOutput != null) {
                                        kernelOutputId = this.KernelOutput.GetId();
                                    }
                                    // 前译
                                    Instance.ServerContext.KernelOutputTranslaterSet.Translate(kernelOutputId, ref input, isPre: true);
                                    if (!string.IsNullOrEmpty(KernelOutput.KernelRestartKeyword) && input.Contains(KernelOutput.KernelRestartKeyword)) {
                                        if (_kernelRestartKeywordOn.AddSeconds(1) < DateTime.Now) {
                                            KernelSelfRestartCount += 1;
                                            _kernelRestartKeywordOn = DateTime.Now;
                                            VirtualRoot.RaiseEvent(new KernelSelfRestartedEvent());
                                        }
                                    }
                                    Instance.ServerContext.KernelOutputSet.Pick(ref input, this);
                                    var kernelOutputKeywords = Instance.KernelOutputKeywordSet.GetKeywords(this.KernelOutput.GetId());
                                    if (kernelOutputKeywords != null && kernelOutputKeywords.Count != 0) {
                                        foreach (var keyword in kernelOutputKeywords) {
                                            if (input.Contains(keyword.Keyword)) {
                                                if (keyword.MessageType.TryParse(out LocalMessageType messageType)) {
                                                    string content = input;
                                                    if (!string.IsNullOrEmpty(keyword.Description)) {
                                                        content = $" 大意：{keyword.Description} 详情：" + content;
                                                    }
                                                    VirtualRoot.LocalMessage(LocalMessageChannel.Kernel, this.GetType().Name, messageType, content, OutEnum.None, toConsole: false);
                                                }
                                            }
                                        }
                                    }
                                    if (isWriteToConsole) {
                                        if (!string.IsNullOrEmpty(input)) {
                                            Write.UserLine(input, ConsoleColor.White);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e);
                        }
                        finally {
                            sreader?.Close();
                            sreader?.Dispose();
                        }
                        Write.UserWarn("内核表演结束");
                    }
                }, TaskCreationOptions.LongRunning);
            }
            #endregion

            #region 互操作
            private struct STARTUPINFO {
                public uint cb;
                public string lpReserved;
                public string lpDesktop;
                public string lpTitle;
                public uint dwX;
                public uint dwY;
                public uint dwXSize;
                public uint dwYSize;
                public uint dwXCountChars;
                public uint dwYCountChars;
                public uint dwFillAttribute;
                public uint dwFlags;
                public short wShowWindow;
                public short cbReserved2;
                public IntPtr lpReserved2;
                public IntPtr hStdInput;
                public IntPtr hStdOutput;
                public IntPtr hStdError;
            }

            private struct PROCESS_INFORMATION {
                public IntPtr hProcess;
                public IntPtr hThread;
                public uint dwProcessId;
                public uint dwThreadId;
            }

            private struct SECURITY_ATTRIBUTES {
                public int length;
                public IntPtr lpSecurityDescriptor;
                public bool bInheritHandle;
            }

            [DllImport(DllName.Kernel32Dll)]
            private static extern int CloseHandle(IntPtr hObject);

            [DllImport(DllName.Kernel32Dll)]
            private static extern bool CreatePipe(out IntPtr phReadPipe, out IntPtr phWritePipe, IntPtr lpPipeAttributes, uint nSize);

            [DllImport(DllName.Kernel32Dll, SetLastError = true)]
            private static extern unsafe bool ReadFile(
                IntPtr hfile,
                void* pBuffer,
                int NumberOfBytesToRead,
                int* pNumberOfBytesRead,
                NativeOverlapped* lpOverlapped
            );

            /// <summary>
            /// 
            /// </summary>
            /// <param name="lpEnvironment">
            /// A pointer to the environment block for the new process. If this parameter is NULL, the new process uses the environment of the calling process.
            /// An environment block consists of a null-terminated block of null-terminated strings. Each string is in the following form:
            /// name=value\0
            /// Because the equal sign is used as a separator, it must not be used in the name of an environment variable.
            /// An environment block can contain either Unicode or ANSI characters. If the environment block pointed to by lpEnvironment contains Unicode characters, be sure that dwCreationFlags includes CREATE_UNICODE_ENVIRONMENT. If this parameter is NULL and the environment block of the parent process contains Unicode characters, you must also ensure that dwCreationFlags includes CREATE_UNICODE_ENVIRONMENT.
            /// The ANSI version of this function, CreateProcessA fails if the total size of the environment block for the process exceeds 32,767 characters.
            /// Note that an ANSI environment block is terminated by two zero bytes: one for the last string, one more to terminate the block. A Unicode environment block is terminated by four zero bytes: two for the last string, two more to terminate the block.
            /// A parent process can directly alter the environment variables of a child process during process creation. This is the only situation when a process can directly change the environment settings of another process. For more information, see Changing Environment Variables.
            /// 
            /// If an application provides an environment block, the current directory information of the system drives is not automatically propagated to the new process.
            /// For example, there is an environment variable named = C: whose value is the current directory on drive C.An application must manually pass the current directory 
            /// information to the new process.To do so, the application must explicitly create these environment variable strings, sort them alphabetically(because the system 
            /// uses a sorted environment), and put them into the environment block.Typically, they will go at the front of the environment block, due to the environment block sort order
            /// </param>
            /// <returns></returns>
            [DllImport(DllName.Kernel32Dll)]
            private static extern bool CreateProcess(
                string lpApplicationName,
                StringBuilder lpCommandLine,
                IntPtr lpProcessAttributes,
                IntPtr lpThreadAttributes,
                bool bInheritHandles,
                uint dwCreationFlags,
                StringBuilder lpEnvironment,
                string lpCurrentDirectory,
                ref STARTUPINFO lpStartupInfo,
                out PROCESS_INFORMATION lpProcessInformation);

            [DllImport(DllName.Kernel32Dll)]
            private static extern bool SetHandleInformation(IntPtr hObject, int dwMask, uint dwFlags);
            #endregion
        }

        #region DualMineContext
        private class DualMineContext : MineContext, IDualMineContext {
            public DualMineContext(
                IMineContext mineContext,
                ICoin dualCoin,
                IPool dualCoinPool,
                string dualCoinWallet,
                double dualCoinWeight,
                Dictionary<string, string> parameters,
                Dictionary<Guid, string> fragments,
                Dictionary<Guid, string> fileWriters,
                int[] useDevices) : base(
                    mineContext.MinerName,
                    mineContext.MainCoin,
                    mineContext.MainCoinPool,
                    mineContext.Kernel,
                    mineContext.KernelInput,
                    mineContext.KernelOutput,
                    mineContext.CoinKernel,
                    mineContext.MainCoinWallet,
                    mineContext.CommandLine,
                    parameters,
                    fragments,
                    fileWriters,
                    useDevices) {
                this.DualCoin = dualCoin;
                this.DualCoinPool = dualCoinPool;
                this.DualCoinWallet = dualCoinWallet;
                this.DualCoinWeight = dualCoinWeight;
            }


            public ICoin DualCoin { get; private set; }

            public IPool DualCoinPool { get; private set; }

            public string DualCoinWallet { get; private set; }

            public double DualCoinWeight { get; private set; }
        }
        #endregion
    }
}
