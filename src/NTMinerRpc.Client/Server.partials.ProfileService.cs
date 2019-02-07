using System;
using System.Collections.Generic;

namespace NTMiner {
    public static partial class Server {
        public partial class ProfileServiceFace {
            public static readonly ProfileServiceFace Instance = new ProfileServiceFace();

            private ProfileServiceFace() {
            }

            public void GetMineWorkAsync(Guid workId, Action<MineWorkData> callback) {
                MineWorkData response = Request<MineWorkData>("Profile", "MineWork", new { workId });
                callback?.Invoke(response);
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public List<MineWorkData> GetMineWorks() {
                List<MineWorkData> response = Request<List<MineWorkData>>("Profile", "MineWorks", null);
                return response;
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <returns></returns>
            public MinerProfileData GetMinerProfile(Guid workId) {
                MinerProfileData response = Request<MinerProfileData>("Profile", "MinerProfile", new { workId });
                return response;
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <param name="coinId"></param>
            /// <returns></returns>
            public CoinProfileData GetCoinProfile(Guid workId, Guid coinId) {
                CoinProfileData response = Request<CoinProfileData>("Profile", "CoinProfile", new { workId, coinId });
                return response;
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <param name="poolId"></param>
            /// <returns></returns>
            public PoolProfileData GetPoolProfile(Guid workId, Guid poolId) {
                PoolProfileData response = Request<PoolProfileData>("Profile", "PoolProfile", new { workId, poolId });
                return response;
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <param name="coinKernelId"></param>
            /// <returns></returns>
            public CoinKernelProfileData GetCoinKernelProfile(Guid workId, Guid coinKernelId) {
                CoinKernelProfileData response = Request<CoinKernelProfileData>("Profile", "CoinKernelProfile", new { workId, coinKernelId });
                return response;
            }
        }
    }
}