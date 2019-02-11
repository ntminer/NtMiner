using NTMiner.Core;
using NTMiner.Core.Gpus;
using NTMiner.Core.Kernels;
using NTMiner.MinerClient;
using NTMiner.Profile;
using NTMiner.Hashrate;
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

            VirtualRoot.Access<HasBoot2SecondEvent>(
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

        public static ProfileData CreateProfileData() {
            INTMinerRoot root = NTMinerRoot.Current;
            IMinerProfile minerProfile = root.MinerProfile;
            if (minerProfile == null) {
                return null;
            }
            ICoin mainCoin;
            if (!root.CoinSet.TryGetCoin(minerProfile.CoinId, out mainCoin)) {
                return null;
            }
            string dualCoinCode = null;
            string dualCoinPool = null;
            string dualCoinWallet = null;
            bool isDualCoinPoolIsUserMode = false;
            string dualCoinPoolUserName = null;
            double dualCoinWeight = 0;
            bool isAutoDualWeight = true;
            bool isDualCoinEnabled = false;
            string mainCoinPool = null;
            string mainCoinWallet = null;
            bool isMainCoinPoolIsUserMode = false;
            string mainCoinPoolUserName = null;
            ICoinProfile mainCoinProfile = root.CoinProfileSet.GetCoinProfile(mainCoin.GetId());
            IPool mainCoinPoolModel;
            if (!root.PoolSet.TryGetPool(mainCoinProfile.PoolId, out mainCoinPoolModel)) {
                return null;
            }
            mainCoinPool = mainCoinPoolModel.Server;
            mainCoinWallet = mainCoinProfile.Wallet;
            isMainCoinPoolIsUserMode = mainCoinPoolModel.IsUserMode;
            mainCoinPoolUserName = mainCoinPoolModel.UserName;
            ICoinKernelProfile coinKernelProfile = root.CoinKernelProfileSet.GetCoinKernelProfile(mainCoinProfile.CoinKernelId);
            isDualCoinEnabled = coinKernelProfile.IsDualCoinEnabled;
            if (isDualCoinEnabled) {
                ICoin dualCoin;
                if (!root.CoinSet.TryGetCoin(coinKernelProfile.DualCoinId, out dualCoin)) {
                    return null;
                }
                dualCoinCode = dualCoin.Code;
                isAutoDualWeight = coinKernelProfile.IsAutoDualWeight;
                dualCoinWeight = coinKernelProfile.DualCoinWeight;
                ICoinProfile dualCoinProfile = root.CoinProfileSet.GetCoinProfile(coinKernelProfile.DualCoinId);
                IPool dualCoinPoolModel;
                if (!root.PoolSet.TryGetPool(dualCoinProfile.PoolId, out dualCoinPoolModel)) {
                    return null;
                }
                dualCoinPool = dualCoinPoolModel.Server;
                dualCoinWallet = dualCoinProfile.Wallet;
                isDualCoinPoolIsUserMode = dualCoinPoolModel.IsUserMode;
                dualCoinPoolUserName = dualCoinPoolModel.UserName;
            }
            ProfileData data = new ProfileData {
                CoinCode = mainCoin.Code,
                DualCoinCode = dualCoinCode,
                DualCoinPool = dualCoinPool,
                DualCoinWallet = dualCoinWallet,
                IsDualCoinPoolIsUserMode = isDualCoinPoolIsUserMode,
                DualCoinPoolUserName = dualCoinPoolUserName,
                DualCoinWeight = dualCoinWeight,
                IsAutoBoot = minerProfile.IsAutoBoot,
                IsAutoDualWeight = isAutoDualWeight,
                IsAutoRestartKernel = minerProfile.IsAutoRestartKernel,
                IsAutoStart = minerProfile.IsAutoStart,
                IsAutoThisPCName = minerProfile.IsAutoThisPCName,
                IsDualCoinEnabled = isDualCoinEnabled,
                IsNoShareRestartKernel = minerProfile.IsNoShareRestartKernel,
                IsPeriodicRestartComputer = minerProfile.IsPeriodicRestartComputer,
                IsPeriodicRestartKernel = minerProfile.IsPeriodicRestartKernel,
                MainCoinPool = mainCoinPool,
                MainCoinWallet = mainCoinWallet,
                IsMainCoinPoolIsUserMode = isMainCoinPoolIsUserMode,
                MainCoinPoolUserName = mainCoinPoolUserName,
                MinerName = minerProfile.MinerName,
                NoShareRestartKernelMinutes = minerProfile.NoShareRestartKernelMinutes,
                PeriodicRestartComputerHours = minerProfile.PeriodicRestartComputerHours,
                PeriodicRestartKernelHours = minerProfile.PeriodicRestartKernelHours
            };
            return data;
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
                MainCoinPool = string.Empty,
                MainCoinWallet = string.Empty,
            };
            if (withGpuSpeeds) {
                data.GpuSpeeds = root.GpusSpeed.Select(a => new GpuSpeedData {
                    Index = a.Gpu.Index,
                    MainCoinSpeed = (long)a.MainCoinSpeed.Value,
                    DualCoinSpeed = (long)a.DualCoinSpeed.Value,
                    FanSpeed = a.Gpu.FanSpeed,
                    Temperature = a.Gpu.Temperature,
                    PowerUsage = a.Gpu.PowerUsage
                }).ToArray();
            }
            #region 当前选中的币种是什么
            ICoin mainCoin;
            if (root.CoinSet.TryGetCoin(root.MinerProfile.CoinId, out mainCoin)) {
                data.MainCoinCode = mainCoin.Code;
                ICoinProfile coinProfile = root.CoinProfileSet.GetCoinProfile(mainCoin.GetId());
                IPool mainCoinPool;
                if (root.PoolSet.TryGetPool(coinProfile.PoolId, out mainCoinPool)) {
                    data.MainCoinPool = mainCoinPool.Server;
                }
                else {
                    data.MainCoinPool = string.Empty;
                }
                data.MainCoinWallet = coinProfile.Wallet;
                ICoinKernel coinKernel;
                if (root.CoinKernelSet.TryGetCoinKernel(coinProfile.CoinKernelId, out coinKernel)) {
                    IKernel kernel;
                    if (root.KernelSet.TryGetKernel(coinKernel.KernelId, out kernel)) {
                        data.Kernel = kernel.FullName;
                        ICoinKernelProfile coinKernelProfile = root.CoinKernelProfileSet.GetCoinKernelProfile(coinProfile.CoinKernelId);
                        data.IsDualCoinEnabled = coinKernelProfile.IsDualCoinEnabled;
                        if (coinKernelProfile.IsDualCoinEnabled) {
                            ICoin dualCoin;
                            if (root.CoinSet.TryGetCoin(coinKernelProfile.DualCoinId, out dualCoin)) {
                                data.DualCoinCode = dualCoin.Code;
                                ICoinProfile dualCoinProfile = root.CoinProfileSet.GetCoinProfile(dualCoin.GetId());
                                data.DualCoinWallet = dualCoinProfile.DualCoinWallet;
                                IPool dualCoinPool;
                                if (root.PoolSet.TryGetPool(dualCoinProfile.DualCoinPoolId, out dualCoinPool)) {
                                    data.DualCoinPool = dualCoinPool.Server;
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
                // 判断上次报告的算力币种和本次报告的是否相同，否则说明刚刚切换了币种默认第一次报告0算力
                if (_lastSpeedMainCoin == null || _lastSpeedMainCoin == root.CurrentMineContext.MainCoin) {
                    _lastSpeedMainCoin = root.CurrentMineContext.MainCoin;
                    CoinShareData preCoinShare;
                    Guid coinId = root.CurrentMineContext.MainCoin.GetId();
                    IGpusSpeed gpuSpeeds = NTMinerRoot.Current.GpusSpeed;
                    IGpuSpeed totalSpeed = gpuSpeeds.CurrentSpeed(NTMinerRoot.GpuAllId);
                    data.MainCoinSpeed = (int)totalSpeed.MainCoinSpeed.Value;
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
                        data.DualCoinSpeed = (int)totalSpeed.DualCoinSpeed.Value;
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
