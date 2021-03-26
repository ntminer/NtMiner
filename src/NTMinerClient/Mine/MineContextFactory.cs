using NTMiner.Core;
using NTMiner.Core.Kernels;
using NTMiner.Core.Profile;
using NTMiner.Gpus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NTMiner.Mine {
    public static class MineContextFactory {
        private class ParameterNames {
            // 根据这个判断是否换成过期
            internal string Body = string.Empty;
            internal readonly HashSet<string> Names = new HashSet<string>();
        }

        public static IMineContext CreateMineContext() {
            INTMinerContext ntminerContext = NTMinerContext.Instance;
            var minerProfile = ntminerContext.MinerProfile;
            var serverContext = ntminerContext.ServerContext;
            var gpuSet = ntminerContext.GpuSet;
            if (!ntminerContext.GetProfileData(out ICoin mainCoin, out ICoinProfile mainCoinProfile, out IPool mainCoinPool, out ICoinKernel mainCoinKernel, out IKernel kernel,
                out IKernelInput kernelInput, out IKernelOutput kernelOutput, out string _)) {
                return null;
            }
            if (!kernel.IsSupported(mainCoin)) {
                return null;
            }
            ICoinKernelProfile coinKernelProfile = minerProfile.GetCoinKernelProfile(mainCoinProfile.CoinKernelId);
            string poolKernelArgs = string.Empty;
            IPoolKernel poolKernel = serverContext.PoolKernelSet.AsEnumerable().FirstOrDefault(a => a.PoolId == mainCoinPool.GetId() && a.KernelId == kernel.GetId());
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
                IPoolProfile poolProfile = minerProfile.GetPoolProfile(mainCoinPool.GetId());
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
            string minerName = $"{mainCoinPool.MinerNamePrefix}{minerProfile.MinerName}{mainCoinPool.MinerNamePostfix}";
            parameters.Add(NTKeyword.WorkerParameterName, minerName);
            if (mainCoinKernel.IsSupportPool1 && !mainCoinPool.NoPool1) {
                parameters.Add(NTKeyword.Worker1ParameterName, minerName);
                if (serverContext.PoolSet.TryGetPool(mainCoinProfile.PoolId1, out IPool mainCoinPool1)) {
                    parameters.Add(NTKeyword.Host1ParameterName, mainCoinPool1.GetHost());
                    parameters.Add(NTKeyword.Port1ParameterName, mainCoinPool1.GetPort().ToString());
                    parameters.Add(NTKeyword.Pool1ParameterName, mainCoinPool1.Server);
                    if (mainCoinPool1.IsUserMode) {
                        IPoolProfile poolProfile1 = minerProfile.GetPoolProfile(mainCoinPool1.GetId());
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
                    if (serverContext.CoinSet.TryGetCoin(coinKernelProfile.DualCoinId, out ICoin dualCoin)) {
                        ICoinProfile dualCoinProfile = minerProfile.GetCoinProfile(dualCoin.GetId());
                        if (serverContext.PoolSet.TryGetPool(dualCoinProfile.DualCoinPoolId, out IPool dualCoinPool)) {
                            string dualUserName = string.Empty;
                            string dualPassword = NTKeyword.PasswordDefaultValue;
                            string dualWallet = dualCoinProfile.DualCoinWallet;
                            parameters.Add(NTKeyword.DualCoinParameterName, dualCoin.Code);
                            if (dualCoinPool.IsUserMode) {
                                IPoolProfile dualPoolProfile = minerProfile.GetPoolProfile(dualCoinPool.GetId());
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
                            BuildFragments(serverContext, mainCoinKernel, parameters, out fileWriters, out fragments);
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
                                    minerProfile.MinerName,
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
                                    gpuSet.GetUseDevices()),
                                dualCoin,
                                dualCoinPool,
                                dualWallet,
                                coinKernelProfile.DualCoinWeight,
                                parameters,
                                fragments,
                                fileWriters,
                                gpuSet.GetUseDevices());
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
            BuildFragments(serverContext, mainCoinKernel, parameters, out fileWriters, out fragments);
            foreach (var fragment in fragments.Values) {
                sb.Append(" ").Append(fragment);
            }
            if (!string.IsNullOrEmpty(customArgs)) {
                sb.Append(" ").Append(customArgs);
            }

            return new MineContext(
                minerProfile.MinerName,
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
                gpuSet.GetUseDevices());
        }

        #region 私有方法
        private static void BuildFragments(IServerContext serverContext, ICoinKernel coinKernel, Dictionary<string, string> parameters, out Dictionary<Guid, string> fileWriters, out Dictionary<Guid, string> fragments) {
            fileWriters = new Dictionary<Guid, string>();
            fragments = new Dictionary<Guid, string>();
            try {
                if (coinKernel.FragmentWriterIds != null && coinKernel.FragmentWriterIds.Count != 0) {
                    foreach (var writerId in coinKernel.FragmentWriterIds) {
                        if (serverContext.FragmentWriterSet.TryGetFragmentWriter(writerId, out IFragmentWriter writer)) {
                            BuildFragment(parameters, fileWriters, fragments, writer);
                        }
                    }
                }
                if (coinKernel.FileWriterIds != null && coinKernel.FileWriterIds.Count != 0) {
                    foreach (var writerId in coinKernel.FileWriterIds) {
                        if (serverContext.FileWriterSet.TryGetFileWriter(writerId, out IFileWriter writer)) {
                            BuildFragment(parameters, fileWriters, fragments, writer);
                        }
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static string GetDevicesArgs(IKernelInput kernelInput) {
            INTMinerContext ntminerContext = NTMinerContext.Instance;
            var gpuSet = ntminerContext.GpuSet;
            string devicesArgs = string.Empty;
            if (!string.IsNullOrWhiteSpace(kernelInput.DevicesArg)) {
                int[] useDevices = gpuSet.GetUseDevices();
                if ((useDevices.Length != 0 && useDevices.Length != gpuSet.Count) || kernelInput.IsDeviceAllNotEqualsNone) {
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
                    switch (gpuSet.GpuType) {
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

        private static readonly HashSet<string> _mainParameterNames = new HashSet<string> {
            NTKeyword.MainCoinParameterName,
            NTKeyword.WalletParameterName,
            NTKeyword.UserNameParameterName,
            NTKeyword.PasswordParameterName,
            NTKeyword.HostParameterName,
            NTKeyword.PortParameterName,
            NTKeyword.PoolParameterName,
            NTKeyword.Pool1ParameterName,
            NTKeyword.WorkerParameterName
        };
        private static readonly HashSet<string> _dualParameterNames = new HashSet<string> {
            NTKeyword.DualCoinParameterName,
            NTKeyword.DualWalletParameterName,
            NTKeyword.DualUserNameParameterName,
            NTKeyword.DualPasswordParameterName,
            NTKeyword.DualHostParameterName,
            NTKeyword.DualPortParameterName,
            NTKeyword.DualPoolParameterName
        };
        private static void AssembleArgs(Dictionary<string, string> prms, ref string args, bool isDual) {
            if (string.IsNullOrEmpty(args)) {
                args = string.Empty;
                return;
            }
            foreach (var parameterName in _mainParameterNames) {
                if (prms.ContainsKey(parameterName)) {
                    args = args.Replace("{" + parameterName + "}", prms[parameterName]);
                }
            }
            if (isDual) {
                foreach (var parameterName in _dualParameterNames) {
                    if (prms.ContainsKey(parameterName)) {
                        args = args.Replace("{" + parameterName + "}", prms[parameterName]);
                    }
                }
            }
            // 防止命令行上还有备用矿池，将备用矿池替换为主矿池
            args = args.Replace("{" + NTKeyword.Pool1ParameterName + "}", prms[NTKeyword.PoolParameterName]);
            // 这里不要考虑{logfile}，{logfile}往后推迟
        }

        private static readonly object _locker = new object();
        private static readonly Dictionary<Guid, ParameterNames> _parameterNameDic = new Dictionary<Guid, ParameterNames>();
        private static readonly string logfile = NTKeyword.LogFileParameterName.TrimStart('{').TrimEnd('}');
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
                        string v = match.Groups[1].Value;
                        // 因为logfile在开始挖矿后才有值
                        if (v != logfile) {
                            parameterNames.Names.Add(v);
                        }
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
                    if (parameters.ContainsKey(parameterName)) {
                        content = content.Replace($"{{{parameterName}}}", parameters[parameterName]);
                    }
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
    }
}
