using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public partial class ProfileServiceFace {
            public static readonly ProfileServiceFace Instance = new ProfileServiceFace();
            private readonly string baseUrl = $"http://{MinerServerHost}:{MinerServerPort}/api/Profile";

            private ProfileServiceFace() {
            }

            public void GetMineWorkAsync(Guid workId, Action<MineWorkData> callback) {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/MineWork", new { workId });
                        MineWorkData response = message.Result.Content.ReadAsAsync<MineWorkData>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch {
                    callback?.Invoke(null);
                }
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public List<MineWorkData> GetMineWorks() {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"{baseUrl}/MineWorks", null);
                        List<MineWorkData> response = message.Result.Content.ReadAsAsync<List<MineWorkData>>().Result;
                        return response;
                    }
                }
                catch {
                    return new List<MineWorkData>();
                }
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <returns></returns>
            public MinerProfileData GetMinerProfile(Guid workId) {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/MinerProfile", new { workId });
                        MinerProfileData response = message.Result.Content.ReadAsAsync<MinerProfileData>().Result;
                        return response;
                    }
                }
                catch {
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
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/CoinProfile", new { workId, coinId });
                        CoinProfileData response = message.Result.Content.ReadAsAsync<CoinProfileData>().Result;
                        return response;
                    }
                }
                catch {
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
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/PoolProfile", new { workId, poolId });
                        PoolProfileData response = message.Result.Content.ReadAsAsync<PoolProfileData>().Result;
                        return response;
                    }
                }
                catch {
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
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/CoinKernelProfile", new { workId, coinKernelId });
                        CoinKernelProfileData response = message.Result.Content.ReadAsAsync<CoinKernelProfileData>().Result;
                        return response;
                    }
                }
                catch {
                    return null;
                }
            }
        }
    }
}