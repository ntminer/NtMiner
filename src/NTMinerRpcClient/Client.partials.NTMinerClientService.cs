using NTMiner.Daemon;
using NTMiner.Hashrate;
using NTMiner.MinerClient;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Client {
        public partial class MinerClientServiceFace {
            public static readonly MinerClientServiceFace Instance = new MinerClientServiceFace();

            private MinerClientServiceFace() {
            }

            public void ShowMainWindowAsync(string clientHost, int clientPort, Action<bool> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:{clientPort}/api/MinerClient/ShowMainWindow", null);
                            bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(false);
                    }
                });
            }

            public void StartMineAsync(StartMineRequest request, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{request.ClientIp}:3336/api/MinerClient/StartMine", request);
                            ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public void StopMineAsync(StopMineRequest request, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{request.ClientIp}:3336/api/MinerClient/StopMine", request);
                            ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public void SetMinerProfilePropertyAsync(Profile.SetClientMinerProfilePropertyRequest request, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{request.ClientIp}:3336/api/MinerClient/SetMinerProfileProperty", request);
                            ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public void GetProfile(string clientHost, Action<ProfileData> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:3336/api/MinerClient/GetProfile", null);
                            ProfileData data = message.Result.Content.ReadAsAsync<ProfileData>().Result;
                            callback?.Invoke(data);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            public void GetSpeed(string clientHost, Action<SpeedData> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:3336/api/MinerClient/GetSpeed", null);
                            SpeedData data = message.Result.Content.ReadAsAsync<SpeedData>().Result;
                            callback?.Invoke(data);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }
        }
    }
}
