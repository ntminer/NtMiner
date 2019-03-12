using NTMiner.Core;
using NTMiner.Core.Gpus;
using NTMiner.MinerClient;
using NTMiner.Profile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static class Report {
        // 用于存储和计算份额delta
        private class CoinShareData {
            public CoinShareData(ICoinShare data) {
                this.AccptedShareCount = data.AcceptShareCount;
                this.RejectedShareCount = data.RejectShareCount;
            }

            public void Update(ICoinShare data) {
                this.AccptedShareCount = data.AcceptShareCount;
                this.RejectedShareCount = data.RejectShareCount;
            }

            public int AccptedShareCount { get; private set; }
            public int RejectedShareCount { get; private set; }
        }

        public static void Init(INTMinerRoot root) {
            if (Design.IsInDesignMode) {
                return;
            }

            VirtualRoot.On<HasBoot5SecondEvent>(
                "登录服务器并报告一次0算力",
                LogEnum.Console,
                action: message => {
                    // 报告0算力从而告知服务器该客户端当前在线的币种
                    ReportSpeed();
                });

            VirtualRoot.On<Per2MinuteEvent>(
                "每两分钟上报一次",
                LogEnum.Console,
                action: message => {
                    ReportSpeed();
                });

            VirtualRoot.On<MineStartedEvent>(
                "开始挖矿后报告状态",
                LogEnum.Console,
                action: message => {
                    ReportSpeed();
                });

            VirtualRoot.On<MineStopedEvent>(
                "停止挖矿后报告状态",
                LogEnum.Console,
                action: message => {
                    try {
                        Server.ReportService.ReportStateAsync(ClientId.Id, isMining: false);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                });
        }

        private static readonly Dictionary<Guid, CoinShareData> SCoinShareDic = new Dictionary<Guid, CoinShareData>();
        private static ICoin _sLastSpeedMainCoin;
        private static ICoin _sLastSpeedDualCoin;
        public static SpeedData CreateSpeedData() {
            INTMinerRoot root = NTMinerRoot.Current;
            SpeedData data = new SpeedData {
                IsAutoBoot = root.MinerProfile.IsAutoBoot,
                IsAutoStart = root.MinerProfile.IsAutoStart,
                Version = NTMinerRoot.CurrentVersion.ToString(4),
                BootOn = root.CreatedOn,
                MineStartedOn = null,
                IsMining = root.IsMining,
                MinerName = NTMinerRoot.GetMinerName(),
                GpuInfo = root.GpuSetInfo,
                ClientId = ClientId.Id,
                MainCoinCode = string.Empty,
                MainCoinWallet = string.Empty,
                MainCoinTotalShare = 0,
                MainCoinRejectShare = 0,
                MainCoinShareDelta = 0,
                MainCoinRejectShareDelta = 0,
                MainCoinSpeed = 0,
                DualCoinCode = string.Empty,
                DualCoinTotalShare = 0,
                DualCoinRejectShare = 0,
                DualCoinShareDelta = 0,
                DualCoinRejectShareDelta = 0,
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
                GpuTable = root.GpusSpeed.Where(a => a.Gpu.Index != NTMinerRoot.GpuAllId).Select(a => new GpuSpeedData {
                    Index = a.Gpu.Index,
                    MainCoinSpeed = a.MainCoinSpeed.Value,
                    DualCoinSpeed = a.DualCoinSpeed.Value,
                    FanSpeed = a.Gpu.FanSpeed,
                    Temperature = a.Gpu.Temperature,
                    PowerUsage = a.Gpu.PowerUsage
                }).ToArray()
            };
            #region 当前选中的币种是什么
            ICoin mainCoin;
            if (root.CoinSet.TryGetCoin(root.MinerProfile.CoinId, out mainCoin)) {
                data.MainCoinCode = mainCoin.Code;
                ICoinProfile coinProfile = root.MinerProfile.GetCoinProfile(mainCoin.GetId());
                data.MainCoinWallet = coinProfile.Wallet;
                IPool mainCoinPool;
                if (root.PoolSet.TryGetPool(coinProfile.PoolId, out mainCoinPool)) {
                    data.MainCoinPool = mainCoinPool.Server;
                }
                else {
                    data.MainCoinPool = string.Empty;
                }
                if (mainCoinPool.IsUserMode) {
                    IPoolProfile mainCoinPoolProfile = root.MinerProfile.GetPoolProfile(coinProfile.PoolId);
                    data.MainCoinWallet = mainCoinPoolProfile.UserName;
                }
                ICoinKernel coinKernel;
                if (root.CoinKernelSet.TryGetCoinKernel(coinProfile.CoinKernelId, out coinKernel)) {
                    IKernel kernel;
                    if (root.KernelSet.TryGetKernel(coinKernel.KernelId, out kernel)) {
                        data.Kernel = kernel.GetFullName();
                        ICoinKernelProfile coinKernelProfile = root.MinerProfile.GetCoinKernelProfile(coinProfile.CoinKernelId);
                        data.IsDualCoinEnabled = coinKernelProfile.IsDualCoinEnabled;
                        if (coinKernelProfile.IsDualCoinEnabled) {
                            ICoin dualCoin;
                            if (root.CoinSet.TryGetCoin(coinKernelProfile.DualCoinId, out dualCoin)) {
                                data.DualCoinCode = dualCoin.Code;
                                ICoinProfile dualCoinProfile = root.MinerProfile.GetCoinProfile(dualCoin.GetId());
                                data.DualCoinWallet = dualCoinProfile.DualCoinWallet;
                                IPool dualCoinPool;
                                if (root.PoolSet.TryGetPool(dualCoinProfile.DualCoinPoolId, out dualCoinPool)) {
                                    data.DualCoinPool = dualCoinPool.Server;
                                }
                                else {
                                    data.DualCoinPool = string.Empty;
                                }
                                if (dualCoinPool.IsUserMode) {
                                    IPoolProfile dualCoinPoolProfile = root.MinerProfile.GetPoolProfile(dualCoinProfile.DualCoinPoolId);
                                    data.DualCoinWallet = dualCoinPoolProfile.UserName;
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
                    if (!SCoinShareDic.TryGetValue(coinId, out var preCoinShare)) {
                        preCoinShare = new CoinShareData(share);
                        SCoinShareDic.Add(coinId, preCoinShare);
                        data.MainCoinShareDelta = share.AcceptShareCount;
                        data.MainCoinRejectShareDelta = share.RejectShareCount;
                    }
                    else {
                        data.MainCoinShareDelta = share.AcceptShareCount - preCoinShare.AccptedShareCount;
                        data.MainCoinRejectShareDelta = share.RejectShareCount - preCoinShare.RejectedShareCount;
                        preCoinShare.Update(share);
                    }
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
                        if (!SCoinShareDic.TryGetValue(coinId, out var preCoinShare)) {
                            preCoinShare = new CoinShareData(share);
                            SCoinShareDic.Add(coinId, preCoinShare);
                            data.DualCoinShareDelta = share.AcceptShareCount;
                            data.DualCoinRejectShareDelta = share.RejectShareCount;
                        }
                        else {
                            data.DualCoinShareDelta = share.AcceptShareCount - preCoinShare.AccptedShareCount;
                            data.DualCoinRejectShareDelta = share.RejectShareCount - preCoinShare.RejectedShareCount;
                            preCoinShare.Update(share);
                        }
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
                Server.ReportService.ReportSpeedAsync(data);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
