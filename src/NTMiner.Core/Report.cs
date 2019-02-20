using NTMiner.Core;
using NTMiner.Core.Gpus;
using NTMiner.Core.Kernels;
using NTMiner.Hashrate;
using NTMiner.Profile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static class Report {
        private class CoinShareData {
            public int TotalShareCount { get; set; }
        }

        public static void Init(INTMinerRoot root) {
            if (Design.IsInDesignMode) {
                return;
            }

            VirtualRoot.Access<HasBoot5SecondEvent>(
                Guid.Parse("b4efba26-1f02-42f2-822a-dd38cffd466d"),
                "登录服务器并报告一次0算力",
                LogEnum.Console,
                action: message => {
                    // 报告0算力从而告知服务器该客户端当前在线的币种
                    ReportSpeed();
                });

            VirtualRoot.Access<Per2MinuteEvent>(
                Guid.Parse("0b16bbd3-329b-4b46-9ebe-c403cae26018"),
                "每两分钟上报一次",
                LogEnum.Console,
                action: message => {
                    ReportSpeed();
                });

            VirtualRoot.Access<MineStartedEvent>(
                Guid.Parse("6e8ab8a2-7d08-41bd-9410-107793639f7f"),
                "开始挖矿后报告状态",
                LogEnum.Console,
                action: message => {
                    ReportSpeed();
                });

            VirtualRoot.Access<MineStopedEvent>(
                Guid.Parse("5ff27468-2a01-4ce4-91a3-aa354791d0eb"),
                "停止挖矿后报告状态",
                LogEnum.Console,
                action: message => {
                    ReportState();
                });
        }

        private static readonly Dictionary<Guid, CoinShareData> _coinShareDic = new Dictionary<Guid, CoinShareData>();
        private static ICoin _lastSpeedMainCoin;
        private static ICoin _lastSpeedDualCoin;
        public static SpeedData CreateSpeedData(bool withGpuSpeeds = false) {
            INTMinerRoot root = NTMinerRoot.Current;
            SpeedData data = new SpeedData {
                WorkId = CommandLineArgs.WorkId,
                Version = NTMinerRoot.CurrentVersion.ToString(4),
                MinerName = root.MinerProfile.MinerName,
                GpuInfo = root.GpuSetInfo,
                ClientId = ClientId.Id,
                MainCoinCode = string.Empty,
                MainCoinWallet = string.Empty,
                MainCoinShareDelta = 0,
                MainCoinSpeed = 0,
                DualCoinCode = string.Empty,
                DualCoinShareDelta = 0,
                DualCoinSpeed = 0,
                IsMining = root.IsMining,
                DualCoinPool = string.Empty,
                DualCoinWallet = string.Empty,
                IsDualCoinEnabled = false,
                Kernel = string.Empty,
                MainCoinPool = string.Empty
            };
            if (withGpuSpeeds) {
                data.GpuSpeeds = root.GpusSpeed.Select(a => new GpuSpeedData {
                    Index = a.Gpu.Index,
                    MainCoinSpeed = a.MainCoinSpeed.Value,
                    DualCoinSpeed = a.DualCoinSpeed.Value,
                    FanSpeed = a.Gpu.FanSpeed,
                    Temperature = a.Gpu.Temperature,
                    PowerUsage = a.Gpu.PowerUsage
                }).ToArray();
            }
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
                        data.Kernel = kernel.FullName;
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
                // 判断上次报告的算力币种和本次报告的是否相同，否则说明刚刚切换了币种默认第一次报告0算力
                if (_lastSpeedMainCoin == null || _lastSpeedMainCoin == root.CurrentMineContext.MainCoin) {
                    _lastSpeedMainCoin = root.CurrentMineContext.MainCoin;
                    CoinShareData preCoinShare;
                    Guid coinId = root.CurrentMineContext.MainCoin.GetId();
                    IGpusSpeed gpuSpeeds = NTMinerRoot.Current.GpusSpeed;
                    IGpuSpeed totalSpeed = gpuSpeeds.CurrentSpeed(NTMinerRoot.GpuAllId);
                    data.MainCoinSpeed = totalSpeed.MainCoinSpeed.Value;
                    ICoinShare share = root.CoinShareSet.GetOrCreate(coinId);
                    if (!_coinShareDic.TryGetValue(coinId, out preCoinShare)) {
                        preCoinShare = new CoinShareData() {
                            TotalShareCount = share.TotalShareCount
                        };
                        _coinShareDic.Add(coinId, preCoinShare);
                        data.MainCoinShareDelta = share.TotalShareCount;
                    }
                    else {
                        data.MainCoinShareDelta = share.TotalShareCount - preCoinShare.TotalShareCount;
                    }
                }
                else {
                    _lastSpeedMainCoin = root.CurrentMineContext.MainCoin;
                }
                if (root.CurrentMineContext is IDualMineContext dualMineContext) {
                    // 判断上次报告的算力币种和本次报告的是否相同，否则说明刚刚切换了币种默认第一次报告0算力
                    if (_lastSpeedDualCoin == null || _lastSpeedDualCoin == dualMineContext.DualCoin) {
                        _lastSpeedDualCoin = dualMineContext.DualCoin;
                        CoinShareData preCoinShare;
                        Guid coinId = dualMineContext.DualCoin.GetId();
                        IGpusSpeed gpuSpeeds = NTMinerRoot.Current.GpusSpeed;
                        IGpuSpeed totalSpeed = gpuSpeeds.CurrentSpeed(NTMinerRoot.GpuAllId);
                        data.DualCoinSpeed = totalSpeed.DualCoinSpeed.Value;
                        ICoinShare share = root.CoinShareSet.GetOrCreate(coinId);
                        if (!_coinShareDic.TryGetValue(coinId, out preCoinShare)) {
                            preCoinShare = new CoinShareData() {
                                TotalShareCount = share.TotalShareCount
                            };
                            _coinShareDic.Add(coinId, preCoinShare);
                            data.DualCoinShareDelta = share.TotalShareCount;
                        }
                        else {
                            data.DualCoinShareDelta = share.TotalShareCount - preCoinShare.TotalShareCount;
                        }
                    }
                    else {
                        _lastSpeedDualCoin = dualMineContext.DualCoin;
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

        private static void ReportState() {
            try {
                Server.ReportService.ReportStateAsync(ClientId.Id, NTMinerRoot.Current.IsMining);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
