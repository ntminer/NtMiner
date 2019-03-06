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

            public void ShowMainWindowAsync(string clientHost, int clientPort, Action<bool, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:{clientPort}/api/MinerClient/ShowMainWindow", null);
                            bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(false, e);
                    }
                });
            }

            public void StartMineAsync(StartMineRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        var response = StartMine(request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }

            public ResponseBase StartMine(StartMineRequest request) {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{request.ClientIp}:3336/api/MinerClient/StartMine", request);
                    ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public void StopMineAsync(StopMineRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        ResponseBase response = StopMine(request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }

            public ResponseBase StopMine(StopMineRequest request) {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{request.ClientIp}:3336/api/MinerClient/StopMine", request);
                    ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public void SetMinerProfilePropertyAsync(Profile.SetClientMinerProfilePropertyRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        var response = SetMinerProfileProperty(request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }

            public ResponseBase SetMinerProfileProperty(Profile.SetClientMinerProfilePropertyRequest request) {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{request.ClientIp}:3336/api/MinerClient/SetMinerProfileProperty", request);
                    ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public Task<SpeedData> GetSpeedAsync(string clientHost, Action<SpeedData, Exception> callback) {
                return Task.Factory.StartNew<SpeedData>(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:3336/api/MinerClient/GetSpeed", null);
                            SpeedData data = message.Result.Content.ReadAsAsync<SpeedData>().Result;
                            callback?.Invoke(data, null);
                            return data;
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                        return null;
                    }
                });
            }
        }
    }
}
