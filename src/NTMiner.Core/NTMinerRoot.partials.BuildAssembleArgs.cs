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
        private static readonly string[] gpuIndexChars = new string[] { "a", "b", "c", "d", "e", "f", "g", "h" };
        public string BuildAssembleArgs(out Dictionary<string, string> parameters, out Dictionary<Guid, string> fileWriters, out Dictionary<Guid, string> fragments) {
            parameters = new Dictionary<string, string>();
            fileWriters = new Dictionary<Guid, string>();
            fragments = new Dictionary<Guid, string>();
            if (!CoinSet.TryGetCoin(this.MinerProfile.CoinId, out ICoin mainCoin)) {
                return string.Empty;
            }
            ICoinProfile coinProfile = this.MinerProfile.GetCoinProfile(mainCoin.GetId());
            if (!PoolSet.TryGetPool(coinProfile.PoolId, out IPool mainCoinPool)) {
                return string.Empty;
            }
            if (!CoinKernelSet.TryGetCoinKernel(coinProfile.CoinKernelId, out ICoinKernel coinKernel)) {
                return string.Empty;
            }
            if (!KernelSet.TryGetKernel(coinKernel.KernelId, out IKernel kernel)) {
                return string.Empty;
            }
            if (!kernel.IsSupported(mainCoin)) {
                return string.Empty;
            }
            if (!KernelInputSet.TryGetKernelInput(kernel.KernelInputId, out IKernelInput kernelInput)) {
                return string.Empty;
            }
            ICoinKernelProfile coinKernelProfile = this.MinerProfile.GetCoinKernelProfile(coinProfile.CoinKernelId);
            string poolKernelArgs = string.Empty;
            IPoolKernel poolKernel = PoolKernelSet.FirstOrDefault(a => a.PoolId == mainCoinPool.GetId() && a.KernelId == kernel.GetId());
            if (poolKernel != null) {
                poolKernelArgs = poolKernel.Args;
            }
            string kernelArgs = kernelInput.Args;
            string coinKernelArgs = coinKernel.Args;
            string customArgs = coinKernelProfile.CustomArgs;
            parameters.Add(Consts.MainCoinParameterName, mainCoin.Code);
            if (mainCoinPool.IsUserMode) {
                IPoolProfile poolProfile = MinerProfile.GetPoolProfile(mainCoinPool.GetId());
                string password = poolProfile.Password;
                if (string.IsNullOrEmpty(password)) {
                    password = Consts.PasswordDefaultValue;
                }
                parameters.Add(Consts.UserNameParameterName, poolProfile.UserName);
                parameters.Add(Consts.PasswordParameterName, password);
                parameters.Add(Consts.WalletParameterName, poolProfile.UserName);
            }
            else {
                parameters.Add(Consts.WalletParameterName, coinProfile.Wallet);
            }
            parameters.Add(Consts.HostParameterName, mainCoinPool.GetHost());
            parameters.Add(Consts.PortParameterName, mainCoinPool.GetPort().ToString());
            parameters.Add(Consts.PoolParameterName, mainCoinPool.Server);
            string minerName = $"{mainCoinPool.MinerNamePrefix}{this.MinerProfile.MinerName}{mainCoinPool.MinerNamePostfix}";
            parameters.Add(Consts.WorkerParameterName, minerName);
            if (coinKernel.IsSupportPool1 && !mainCoinPool.NoPool1) {
                parameters.Add(Consts.Worker1ParameterName, minerName);
                if (PoolSet.TryGetPool(coinProfile.PoolId1, out IPool mainCoinPool1)) {
                    parameters.Add(Consts.Host1ParameterName, mainCoinPool1.GetHost());
                    parameters.Add(Consts.Port1ParameterName, mainCoinPool1.GetPort().ToString());
                    parameters.Add(Consts.Pool1ParameterName, mainCoinPool1.Server);
                    if (mainCoinPool1.IsUserMode) {
                        IPoolProfile poolProfile1 = MinerProfile.GetPoolProfile(mainCoinPool1.GetId());
                        string password1 = poolProfile1.Password;
                        if (string.IsNullOrEmpty(password1)) {
                            password1 = Consts.PasswordDefaultValue;
                        }
                        parameters.Add(Consts.UserName1ParameterName, poolProfile1.UserName);
                        parameters.Add(Consts.Password1ParameterName, password1);
                    }
                    else {
                        parameters.Add(Consts.Wallet1ParameterName, coinProfile.Wallet);
                    }
                }
            }
            string devicesArgs = GetDevicesArgs(kernelInput);
            // 这里不要考虑{logfile}，{logfile}往后推迟
            if (coinKernelProfile.IsDualCoinEnabled && kernelInput.IsSupportDualMine) {
                Guid dualCoinGroupId = coinKernel.DualCoinGroupId;
                if (dualCoinGroupId == Guid.Empty) {
                    dualCoinGroupId = kernelInput.DualCoinGroupId;
                }
                if (dualCoinGroupId != Guid.Empty) {
                    if (this.CoinSet.TryGetCoin(coinKernelProfile.DualCoinId, out ICoin dualCoin)) {
                        ICoinProfile dualCoinProfile = this.MinerProfile.GetCoinProfile(dualCoin.GetId());
                        if (PoolSet.TryGetPool(dualCoinProfile.DualCoinPoolId, out IPool dualCoinPool)) {
                            IPoolProfile dualPoolProfile = MinerProfile.GetPoolProfile(dualCoinPool.GetId());
                            string dualPassword = dualPoolProfile.Password;
                            if (string.IsNullOrEmpty(dualPassword)) {
                                dualPassword = Consts.PasswordDefaultValue;
                            }
                            parameters.Add(Consts.DualCoinParameterName, dualCoin.Code);
                            parameters.Add(Consts.DualWalletParameterName, dualCoinProfile.DualCoinWallet);
                            parameters.Add(Consts.DualUserNameParameterName, dualPoolProfile.UserName);
                            parameters.Add(Consts.DualPasswordParameterName, dualPassword);
                            parameters.Add(Consts.DualHostParameterName, dualCoinPool.GetHost());
                            parameters.Add(Consts.DualPortParameterName, dualCoinPool.GetPort().ToString());
                            parameters.Add(Consts.DualPoolParameterName, dualCoinPool.Server);

                            kernelArgs = kernelInput.DualFullArgs;
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
                            BuildFragments(coinKernel, parameters, out fileWriters, out fragments);
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
                            return dualSb.ToString();
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
            BuildFragments(coinKernel, parameters, out fileWriters, out fragments);
            foreach (var fragment in fragments.Values) {
                sb.Append(" ").Append(fragment);
            }
            if (!string.IsNullOrEmpty(customArgs)) {
                sb.Append(" ").Append(customArgs);
            }

            return sb.ToString();
        }

        private string GetDevicesArgs(IKernelInput kernelInput) {
            string devicesArgs = string.Empty;
            if (!string.IsNullOrWhiteSpace(kernelInput.DevicesArg)) {
                List<int> useDevices = this.GpuSet.GetUseDevices();
                if ((useDevices.Count != 0 && useDevices.Count != GpuSet.Count) || kernelInput.IsDeviceAllNotEqualsNone) {
                    string separator = kernelInput.DevicesSeparator;
                    // 因为空格在界面上不易被人读取所以以关键字代替空格
                    if (kernelInput.DevicesSeparator == Consts.SpaceKeyword) {
                        separator = " ";
                    }
                    if (string.IsNullOrEmpty(separator)) {
                        List<string> gpuIndexes = new List<string>();
                        foreach (var index in useDevices) {
                            int i = index;
                            if (kernelInput.DeviceBaseIndex != 0) {
                                i = index + kernelInput.DeviceBaseIndex;
                            }
                            string nText = i.ToString();
                            if (i > 9) {
                                nText = gpuIndexChars[i - 10];
                            }
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
                    else {
                        switch (GpuSet.GpuType) {
                            case GpuType.Empty:
                                break;
                            case GpuType.NVIDIA:
                                devicesArgs = $"{kernelInput.DevicesArg} {string.Join(separator, useDevices.Select(a => $"{kernelInput.NDevicePrefix}{a}{kernelInput.NDevicePostfix}"))}";
                                break;
                            case GpuType.AMD:
                                devicesArgs = $"{kernelInput.DevicesArg} {string.Join(separator, useDevices.Select(a => $"{kernelInput.ADevicePrefix}{a}{kernelInput.ADevicePostfix}"))}";
                                break;
                            default:
                                break;
                        }
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
            args = args.Replace("{" + Consts.MainCoinParameterName + "}", prms[Consts.MainCoinParameterName]);
            if (prms.ContainsKey(Consts.WalletParameterName)) {
                args = args.Replace("{" + Consts.WalletParameterName + "}", prms[Consts.WalletParameterName]);
            }
            if (prms.ContainsKey(Consts.UserNameParameterName)) {
                args = args.Replace("{" + Consts.UserNameParameterName + "}", prms[Consts.UserNameParameterName]);
            }
            if (prms.ContainsKey(Consts.PasswordParameterName)) {
                args = args.Replace("{" + Consts.PasswordParameterName + "}", prms[Consts.PasswordParameterName]);
            }
            args = args.Replace("{" + Consts.HostParameterName + "}", prms[Consts.HostParameterName]);
            args = args.Replace("{" + Consts.PortParameterName + "}", prms[Consts.PortParameterName]);
            args = args.Replace("{" + Consts.PoolParameterName + "}", prms[Consts.PoolParameterName]);
            args = args.Replace("{" + Consts.WorkerParameterName + "}", prms[Consts.WorkerParameterName]);
            if (isDual) {
                args = args.Replace("{" + Consts.DualCoinParameterName + "}", prms[Consts.DualCoinParameterName]);
                args = args.Replace("{" + Consts.DualWalletParameterName + "}", prms[Consts.DualWalletParameterName]);
                args = args.Replace("{" + Consts.DualUserNameParameterName + "}", prms[Consts.DualUserNameParameterName]);
                args = args.Replace("{" + Consts.DualPasswordParameterName + "}", prms[Consts.DualPasswordParameterName]);
                args = args.Replace("{" + Consts.DualHostParameterName + "}", prms[Consts.DualHostParameterName]);
                args = args.Replace("{" + Consts.DualPortParameterName + "}", prms[Consts.DualPortParameterName]);
                args = args.Replace("{" + Consts.DualPoolParameterName + "}", prms[Consts.DualPoolParameterName]);
            }
            // 这里不要考虑{logfile}，{logfile}往后推迟
        }

        public void BuildFragments(ICoinKernel coinKernel, Dictionary<string, string> parameters, out Dictionary<Guid, string> fileWriters, out Dictionary<Guid, string> fragments) {
            fileWriters = new Dictionary<Guid, string>();
            fragments = new Dictionary<Guid, string>();
            try {
                if (coinKernel.FragmentWriterIds != null && coinKernel.FragmentWriterIds.Count != 0) {
                    foreach (var writerId in coinKernel.FragmentWriterIds) {
                        if (FragmentWriterSet.TryGetFragmentWriter(writerId, out IFragmentWriter writer)) {
                            BuildFragment(parameters, fileWriters, fragments, writer);
                        }
                    }
                }
                if (coinKernel.FileWriterIds != null && coinKernel.FileWriterIds.Count != 0) {
                    foreach (var writerId in coinKernel.FileWriterIds) {
                        if (FileWriterSet.TryGetFileWriter(writerId, out IFileWriter writer)) {
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
