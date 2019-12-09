using NTMiner.Core.Gpus;
using NTMiner.Core.Profiles;
using NTMiner.MinerClient;
using NTMiner.Profile;
using System;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class Reporter : IReporter {
        public Reporter() {
            if (VirtualRoot.IsMinerClient) {
                VirtualRoot.AddOnecePath<HasBoot10SecondEvent>("登录服务器并报告一次0算力", LogEnum.DevConsole,
                action: message => {
                    // 报告0算力从而告知服务器该客户端当前在线的币种
                    ReportSpeed();
                }, location: this.GetType(), pathId: Guid.Empty);

                VirtualRoot.AddEventPath<Per2MinuteEvent>("每两分钟上报一次", LogEnum.DevConsole,
                    action: message => {
                        ReportSpeed();
                    }, location: this.GetType());

                VirtualRoot.AddEventPath<MineStartedEvent>("开始挖矿后报告状态", LogEnum.DevConsole,
                    action: message => {
                        ReportSpeed();
                    }, location: this.GetType());

                VirtualRoot.AddEventPath<MineStopedEvent>("停止挖矿后报告状态", LogEnum.DevConsole,
                    action: message => {
                        RpcRoot.Server.ReportService.ReportStateAsync(NTKeyword.OfficialServerHost, VirtualRoot.Id, isMining: false);
                    }, location: this.GetType());
            }
        }

        private ICoin _sLastSpeedMainCoin;
        private ICoin _sLastSpeedDualCoin;
        public SpeedData CreateSpeedData() {
            INTMinerRoot root = NTMinerRoot.Instance;
            IWorkProfile workProfile = root.MinerProfile;
            string macAddress = string.Empty;
            string localIp = string.Empty;
            foreach (var item in VirtualRoot.LocalIpSet.AsEnumerable()) {
                if (macAddress.Length != 0) {
                    macAddress += "," + item.MACAddress;
                    localIp += "," + item.IPAddress;
                }
                else {
                    macAddress = item.MACAddress;
                    localIp = item.IPAddress;
                }
            }
            SpeedData data = new SpeedData {
                LocalServerMessageTimestamp = VirtualRoot.LocalServerMessageSetTimestamp,
                KernelSelfRestartCount = 0,
                IsAutoBoot = workProfile.IsAutoBoot,
                IsAutoStart = workProfile.IsAutoStart,
                AutoStartDelaySeconds = workProfile.AutoStartDelaySeconds,
                Version = EntryAssemblyInfo.CurrentVersion.ToString(4),
                BootOn = root.CreatedOn,
                MineStartedOn = null,
                IsMining = root.IsMining,
                MineWorkId = Guid.Empty,
                MineWorkName = string.Empty,
                MinerName = workProfile.MinerName,
                GpuInfo = root.GpuSetInfo,
                ClientId = VirtualRoot.Id,
                MACAddress = macAddress,
                LocalIp = localIp,
                MainCoinCode = string.Empty,
                MainCoinWallet = string.Empty,
                MainCoinTotalShare = 0,
                MainCoinRejectShare = 0,
                MainCoinSpeed = 0,
                DualCoinCode = string.Empty,
                DualCoinTotalShare = 0,
                DualCoinRejectShare = 0,
                DualCoinSpeed = 0,
                DualCoinPool = string.Empty,
                DualCoinWallet = string.Empty,
                IsDualCoinEnabled = false,
                Kernel = string.Empty,
                MainCoinPool = string.Empty,
                OSName = Windows.OS.Instance.WindowsEdition,
                GpuDriver = root.GpuSet.DriverVersion.ToString(),
                GpuType = root.GpuSet.GpuType,
                OSVirtualMemoryMb = NTMinerRoot.OSVirtualMemoryMb,
                KernelCommandLine = string.Empty,
                DiskSpace = NTMinerRoot.DiskSpace,
                IsAutoRestartKernel = workProfile.IsAutoRestartKernel,
                AutoRestartKernelTimes = workProfile.AutoRestartKernelTimes,
                IsNoShareRestartKernel = workProfile.IsNoShareRestartKernel,
                NoShareRestartKernelMinutes = workProfile.NoShareRestartKernelMinutes,
                IsNoShareRestartComputer = workProfile.IsNoShareRestartComputer,
                NoShareRestartComputerMinutes = workProfile.NoShareRestartComputerMinutes,
                IsPeriodicRestartComputer = workProfile.IsPeriodicRestartComputer,
                PeriodicRestartComputerHours = workProfile.PeriodicRestartComputerHours,
                IsPeriodicRestartKernel = workProfile.IsPeriodicRestartKernel,
                PeriodicRestartKernelHours = workProfile.PeriodicRestartKernelHours,
                PeriodicRestartComputerMinutes = workProfile.PeriodicRestartComputerMinutes,
                PeriodicRestartKernelMinutes = workProfile.PeriodicRestartKernelMinutes,
                IsAutoStartByCpu = workProfile.IsAutoStartByCpu,
                IsAutoStopByCpu = workProfile.IsAutoStopByCpu,
                CpuGETemperatureSeconds = workProfile.CpuGETemperatureSeconds,
                CpuLETemperatureSeconds = workProfile.CpuLETemperatureSeconds,
                CpuStartTemperature = workProfile.CpuStartTemperature,
                CpuStopTemperature = workProfile.CpuStopTemperature,
                MainCoinPoolDelay = string.Empty,
                DualCoinPoolDelay = string.Empty,
                MinerIp = string.Empty,
                IsFoundOneGpuShare = false,
                IsGotOneIncorrectGpuShare = false,
                IsRejectOneGpuShare = false,
                CpuPerformance = root.CpuPackage.Performance,
                CpuTemperature = root.CpuPackage.Temperature,
                IsRaiseHighCpuEvent = workProfile.IsRaiseHighCpuEvent,
                HighCpuPercent = workProfile.HighCpuBaseline,
                HighCpuSeconds = workProfile.HighCpuSeconds,
                GpuTable = root.GpusSpeed.AsEnumerable().Where(a => a.Gpu.Index != NTMinerRoot.GpuAllId).Select(a => a.ToGpuSpeedData()).ToArray()
            };
            if (workProfile.MineWork != null) {
                data.MineWorkId = workProfile.MineWork.GetId();
                data.MineWorkName = workProfile.MineWork.Name;
            }
            #region 当前选中的币种是什么
            if (root.ServerContext.CoinSet.TryGetCoin(workProfile.CoinId, out ICoin mainCoin)) {
                data.MainCoinCode = mainCoin.Code;
                ICoinProfile coinProfile = workProfile.GetCoinProfile(mainCoin.GetId());
                data.MainCoinWallet = coinProfile.Wallet;
                if (root.ServerContext.PoolSet.TryGetPool(coinProfile.PoolId, out IPool mainCoinPool)) {
                    data.MainCoinPool = mainCoinPool.Server;
                    if (root.IsMining) {
                        data.MainCoinPoolDelay = root.ServerContext.PoolSet.GetPoolDelayText(mainCoinPool.GetId(), isDual: false);
                    }
                    if (mainCoinPool.IsUserMode) {
                        IPoolProfile mainCoinPoolProfile = workProfile.GetPoolProfile(coinProfile.PoolId);
                        data.MainCoinWallet = mainCoinPoolProfile.UserName;
                    }
                }
                else {
                    data.MainCoinPool = string.Empty;
                }
                if (root.ServerContext.CoinKernelSet.TryGetCoinKernel(coinProfile.CoinKernelId, out ICoinKernel coinKernel)) {
                    if (root.ServerContext.KernelSet.TryGetKernel(coinKernel.KernelId, out IKernel kernel)) {
                        data.Kernel = kernel.GetFullName();
                        if (root.ServerContext.KernelOutputSet.TryGetKernelOutput(kernel.KernelOutputId, out IKernelOutput kernelOutput)) {
                            data.IsFoundOneGpuShare = !string.IsNullOrEmpty(kernelOutput.FoundOneShare);
                            data.IsGotOneIncorrectGpuShare = !string.IsNullOrEmpty(kernelOutput.GpuGotOneIncorrectShare);
                            data.IsRejectOneGpuShare = !string.IsNullOrEmpty(kernelOutput.RejectOneShare);
                        }
                        ICoinKernelProfile coinKernelProfile = workProfile.GetCoinKernelProfile(coinProfile.CoinKernelId);
                        data.IsDualCoinEnabled = coinKernelProfile.IsDualCoinEnabled;
                        if (coinKernelProfile.IsDualCoinEnabled) {
                            if (root.ServerContext.CoinSet.TryGetCoin(coinKernelProfile.DualCoinId, out ICoin dualCoin)) {
                                data.DualCoinCode = dualCoin.Code;
                                ICoinProfile dualCoinProfile = workProfile.GetCoinProfile(dualCoin.GetId());
                                data.DualCoinWallet = dualCoinProfile.DualCoinWallet;
                                if (root.ServerContext.PoolSet.TryGetPool(dualCoinProfile.DualCoinPoolId, out IPool dualCoinPool)) {
                                    data.DualCoinPool = dualCoinPool.Server;
                                    if (root.IsMining) {
                                        data.DualCoinPoolDelay = root.ServerContext.PoolSet.GetPoolDelayText(dualCoinPool.GetId(), isDual: true);
                                    }
                                    if (dualCoinPool.IsUserMode) {
                                        IPoolProfile dualCoinPoolProfile = workProfile.GetPoolProfile(dualCoinProfile.DualCoinPoolId);
                                        data.DualCoinWallet = dualCoinPoolProfile.UserName;
                                    }
                                }
                                else {
                                    data.DualCoinPool = string.Empty;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            if (root.IsMining) {
                var mineContext = root.LockedMineContext;
                if (mineContext != null) {
                    data.KernelSelfRestartCount = mineContext.KernelSelfRestartCount;
                    data.MineStartedOn = mineContext.CreatedOn;
                    data.KernelCommandLine = mineContext.CommandLine;
                }
                // 判断上次报告的算力币种和本次报告的是否相同，否则说明刚刚切换了币种默认第一次报告0算力
                if (_sLastSpeedMainCoin == null || _sLastSpeedMainCoin == root.LockedMineContext.MainCoin) {
                    _sLastSpeedMainCoin = root.LockedMineContext.MainCoin;
                    Guid coinId = root.LockedMineContext.MainCoin.GetId();
                    IGpusSpeed gpuSpeeds = NTMinerRoot.Instance.GpusSpeed;
                    IGpuSpeed totalSpeed = gpuSpeeds.CurrentSpeed(NTMinerRoot.GpuAllId);
                    data.MainCoinSpeed = totalSpeed.MainCoinSpeed.Value;
                    ICoinShare share = root.CoinShareSet.GetOrCreate(coinId);
                    data.MainCoinTotalShare = share.TotalShareCount;
                    data.MainCoinRejectShare = share.RejectShareCount;
                }
                else {
                    _sLastSpeedMainCoin = root.LockedMineContext.MainCoin;
                }
                if (root.LockedMineContext is IDualMineContext dualMineContext) {
                    // 判断上次报告的算力币种和本次报告的是否相同，否则说明刚刚切换了币种默认第一次报告0算力
                    if (_sLastSpeedDualCoin == null || _sLastSpeedDualCoin == dualMineContext.DualCoin) {
                        _sLastSpeedDualCoin = dualMineContext.DualCoin;
                        Guid coinId = dualMineContext.DualCoin.GetId();
                        IGpusSpeed gpuSpeeds = NTMinerRoot.Instance.GpusSpeed;
                        IGpuSpeed totalSpeed = gpuSpeeds.CurrentSpeed(NTMinerRoot.GpuAllId);
                        data.DualCoinSpeed = totalSpeed.DualCoinSpeed.Value;
                        ICoinShare share = root.CoinShareSet.GetOrCreate(coinId);
                        data.DualCoinTotalShare = share.TotalShareCount;
                        data.DualCoinRejectShare = share.RejectShareCount;
                    }
                    else {
                        _sLastSpeedDualCoin = dualMineContext.DualCoin;
                    }
                }
            }
            return data;
        }

        private void ReportSpeed() {
            try {
                SpeedData data = CreateSpeedData();
                RpcRoot.Server.ReportService.ReportSpeedAsync(NTKeyword.OfficialServerHost, data, response => {
                    if (response.IsSuccess()) {
                        AppVersionChangedEvent.PublishIfNewVersion(response.ServerState.MinerClientVersion);
                        if (response.NewServerMessages.Count != 0) {
                            VirtualRoot.Execute(new ReceiveServerMessageCommand(response.NewServerMessages));
                        }
                        else {
                            VirtualRoot.Execute(new LoadNewServerMessageCommand(response.ServerState.MessageTimestamp));
                        }
                        VirtualRoot.Execute(new LoadKernelOutputKeywordCommand(response.ServerState.OutputKeywordTimestamp));
                    }
                });
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
