using NTMiner.Core;
using NTMiner.Core.Gpus;
using NTMiner.Core.Kernels;
using NTMiner.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NTMiner {
    public partial class NTMinerRoot : INTMinerRoot {
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
            IPoolKernel poolKernel = ServerContext.PoolKernelSet.FirstOrDefault(a => a.PoolId == mainCoinPool.GetId() && a.KernelId == kernel.GetId());
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
                                    dualWeightArg = $"{kernelInput.DualWeightArg} {Convert.ToInt32(coinKernelProfile.DualCoinWeight)}";
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
                    if (kernelInput.DevicesSeparator == VirtualRoot.SpaceKeyword) {
                        separator = " ";
                    }
                    List<string> gpuIndexes = new List<string>();
                    foreach (var index in useDevices) {
                        int i = index;
                        if (kernelInput.DeviceBaseIndex != 0) {
                            i = index + kernelInput.DeviceBaseIndex;
                        }
                        string nText = VirtualRoot.GetIndexChar(i, separator);
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

        public void BuildFragments(ICoinKernel coinKernel, Dictionary<string, string> parameters, out Dictionary<Guid, string> fileWriters, out Dictionary<Guid, string> fragments) {
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

        public class ParameterNames {
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
    }
}
