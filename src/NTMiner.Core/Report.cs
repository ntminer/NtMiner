using NTMiner.Core;
using NTMiner.Core.Gpus;
using NTMiner.MinerClient;
using NTMiner.Profile;
using System;
using System.Linq;

namespace NTMiner {
    public static class Report {
        public static void Init(INTMinerRoot root) {
            if (Design.IsInDesignMode) {
                return;
            }

            VirtualRoot.On<HasBoot5SecondEvent>("登录服务器并报告一次0算力", LogEnum.DevConsole,
                action: message => {
                    // 报告0算力从而告知服务器该客户端当前在线的币种
                    ReportSpeed();
                });

            VirtualRoot.On<Per2MinuteEvent>("每两分钟上报一次", LogEnum.DevConsole,
                action: message => {
                    ReportSpeed();
                });

            VirtualRoot.On<MineStartedEvent>("开始挖矿后报告状态", LogEnum.DevConsole,
                action: message => {
                    ReportSpeed();
                });

            VirtualRoot.On<MineStopedEvent>("停止挖矿后报告状态", LogEnum.DevConsole,
                action: message => {
                    try {
                        Server.ReportService.ReportStateAsync(AssemblyInfo.OfficialServerHost, ClientId.Id, isMining: false);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                });
        }

        private static ICoin _sLastSpeedMainCoin;
        private static ICoin _sLastSpeedDualCoin;
        public static SpeedData CreateSpeedData() {
            INTMinerRoot root = NTMinerRoot.Current;
            SpeedData data = new SpeedData {
                IsAutoBoot = NTMinerRegistry.GetIsAutoBoot(),
                IsAutoStart = NTMinerRegistry.GetIsAutoStart(),
                Version = NTMinerRoot.CurrentVersion.ToString(4),
                BootOn = root.CreatedOn,
                MineStartedOn = null,
                IsMining = root.IsMining,
                MinerName = root.MinerProfile.MinerName,
                GpuInfo = root.GpuSetInfo,
                ClientId = ClientId.Id,
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
                OSName = Windows.OS.Current.WindowsEdition,
                GpuDriver = root.GpuSet.GetProperty("DriverVersion"),
                GpuType = root.GpuSet.GpuType,
                OSVirtualMemoryMb = NTMinerRoot.OSVirtualMemoryMb,
                KernelCommandLine = NTMinerRoot.UserKernelCommandLine,
                DiskSpace = NTMinerRoot.DiskSpace,
                IsAutoRestartKernel = root.MinerProfile.IsAutoRestartKernel,
                IsNoShareRestartKernel = root.MinerProfile.IsNoShareRestartKernel,
                IsPeriodicRestartComputer = root.MinerProfile.IsPeriodicRestartComputer,
                IsPeriodicRestartKernel = root.MinerProfile.IsPeriodicRestartKernel,
                NoShareRestartKernelMinutes = root.MinerProfile.NoShareRestartKernelMinutes,
                PeriodicRestartComputerHours = root.MinerProfile.PeriodicRestartComputerHours,
                PeriodicRestartKernelHours = root.MinerProfile.PeriodicRestartKernelHours,
                GpuTable = root.GpusSpeed.Where(a => a.Gpu.Index != NTMinerRoot.GpuAllId).Select(a => new GpuSpeedData {
                    Index = a.Gpu.Index,
                    Name = a.Gpu.Name,
                    MainCoinSpeed = a.MainCoinSpeed.Value,
                    DualCoinSpeed = a.DualCoinSpeed.Value,
                    FanSpeed = a.Gpu.FanSpeed,
                    Temperature = a.Gpu.Temperature,
                    PowerUsage = a.Gpu.PowerUsage,
                    Cool = a.Gpu.Cool,
                    Power = a.Gpu.Power,
                    CoreClockDelta = a.Gpu.CoreClockDelta,
                    MemoryClockDelta = a.Gpu.MemoryClockDelta
                }).ToArray()
            };
            #region 当前选中的币种是什么
            if (root.CoinSet.TryGetCoin(root.MinerProfile.CoinId, out ICoin mainCoin)) {
                data.MainCoinCode = mainCoin.Code;
                ICoinProfile coinProfile = root.MinerProfile.GetCoinProfile(mainCoin.GetId());
                data.MainCoinWallet = coinProfile.Wallet;
                if (root.PoolSet.TryGetPool(coinProfile.PoolId, out IPool mainCoinPool)) {
                    data.MainCoinPool = mainCoinPool.Server;
                    if (mainCoinPool.IsUserMode) {
                        IPoolProfile mainCoinPoolProfile = root.MinerProfile.GetPoolProfile(coinProfile.PoolId);
                        data.MainCoinWallet = mainCoinPoolProfile.UserName;
                    }
                }
                else {
                    data.MainCoinPool = string.Empty;
                }
                if (root.CoinKernelSet.TryGetCoinKernel(coinProfile.CoinKernelId, out ICoinKernel coinKernel)) {
                    if (root.KernelSet.TryGetKernel(coinKernel.KernelId, out IKernel kernel)) {
                        data.Kernel = kernel.GetFullName();
                        ICoinKernelProfile coinKernelProfile = root.MinerProfile.GetCoinKernelProfile(coinProfile.CoinKernelId);
                        data.IsDualCoinEnabled = coinKernelProfile.IsDualCoinEnabled;
                        if (coinKernelProfile.IsDualCoinEnabled) {
                            if (root.CoinSet.TryGetCoin(coinKernelProfile.DualCoinId, out ICoin dualCoin)) {
                                data.DualCoinCode = dualCoin.Code;
                                ICoinProfile dualCoinProfile = root.MinerProfile.GetCoinProfile(dualCoin.GetId());
                                data.DualCoinWallet = dualCoinProfile.DualCoinWallet;
                                if (root.PoolSet.TryGetPool(dualCoinProfile.DualCoinPoolId, out IPool dualCoinPool)) {
                                    data.DualCoinPool = dualCoinPool.Server;
                                    if (dualCoinPool.IsUserMode) {
                                        IPoolProfile dualCoinPoolProfile = root.MinerProfile.GetPoolProfile(dualCoinProfile.DualCoinPoolId);
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
                var mineContext = root.CurrentMineContext;
                if (mineContext != null) {
                    data.MineStartedOn = mineContext.CreatedOn;
                    data.KernelCommandLine = mineContext.CommandLine;
                }
                // 判断上次报告的算力币种和本次报告的是否相同，否则说明刚刚切换了币种默认第一次报告0算力
                if (_sLastSpeedMainCoin == null || _sLastSpeedMainCoin == root.CurrentMineContext.MainCoin) {
                    _sLastSpeedMainCoin = root.CurrentMineContext.MainCoin;
                    Guid coinId = root.CurrentMineContext.MainCoin.GetId();
                    IGpusSpeed gpuSpeeds = NTMinerRoot.Current.GpusSpeed;
                    IGpuSpeed totalSpeed = gpuSpeeds.CurrentSpeed(NTMinerRoot.GpuAllId);
                    data.MainCoinSpeed = totalSpeed.MainCoinSpeed.Value;
                    ICoinShare share = root.CoinShareSet.GetOrCreate(coinId);
                    data.MainCoinTotalShare = share.TotalShareCount;
                    data.MainCoinRejectShare = share.RejectShareCount;
                }
                else {
                    _sLastSpeedMainCoin = root.CurrentMineContext.MainCoin;
                }
                if (root.CurrentMineContext is IDualMineContext dualMineContext) {
                    // 判断上次报告的算力币种和本次报告的是否相同，否则说明刚刚切换了币种默认第一次报告0算力
                    if (_sLastSpeedDualCoin == null || _sLastSpeedDualCoin == dualMineContext.DualCoin) {
                        _sLastSpeedDualCoin = dualMineContext.DualCoin;
                        Guid coinId = dualMineContext.DualCoin.GetId();
                        IGpusSpeed gpuSpeeds = NTMinerRoot.Current.GpusSpeed;
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

        private static void ReportSpeed() {
            try {
                SpeedData data = CreateSpeedData();
                Server.ReportService.ReportSpeedAsync(AssemblyInfo.OfficialServerHost, data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
