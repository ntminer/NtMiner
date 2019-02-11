using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public static partial class Server {
        public partial class ProfileServiceFace {
            public static readonly ProfileServiceFace Instance = new ProfileServiceFace();

            private ProfileServiceFace() {
            }

            public MineWorkData GetMineWork(Guid workId) {
                try {
                    MineWorkRequest request = new MineWorkRequest {
                        WorkId = workId
                    };
                    MineWorkData response = Request<MineWorkData>("Profile", "MineWork", request);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public List<MineWorkData> GetMineWorks() {
                try {
                    List<MineWorkData> response = Request<List<MineWorkData>>("Profile", "MineWorks", null);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <returns></returns>
            public MinerProfileData GetMinerProfile(Guid workId) {
                try {
                    MinerProfileRequest request = new MinerProfileRequest {
                        WorkId = workId
                    };
                    MinerProfileData response = Request<MinerProfileData>("Profile", "MinerProfile", request);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <param name="coinId"></param>
            /// <returns></returns>
            public CoinProfileData GetCoinProfile(Guid workId, Guid coinId) {
                try {
                    CoinProfileRequest request = new CoinProfileRequest {
                        WorkId = workId,
                        CoinId = coinId
                    };
                    CoinProfileData response = Request<CoinProfileData>("Profile", "CoinProfile", request);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <param name="poolId"></param>
            /// <returns></returns>
            public PoolProfileData GetPoolProfile(Guid workId, Guid poolId) {
                try {
                    PoolProfileRequest request = new PoolProfileRequest {
                        WorkId = workId,
                        PoolId = poolId
                    };
                    PoolProfileData response = Request<PoolProfileData>("Profile", "PoolProfile", request);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <param name="coinKernelId"></param>
            /// <returns></returns>
            public CoinKernelProfileData GetCoinKernelProfile(Guid workId, Guid coinKernelId) {
                try {
                    CoinKernelProfileRequest request = new CoinKernelProfileRequest {
                        WorkId = workId,
                        CoinKernelId = coinKernelId
                    };
                    CoinKernelProfileData response = Request<CoinKernelProfileData>("Profile", "CoinKernelProfile", request);
                    return response;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }
        }
    }
}