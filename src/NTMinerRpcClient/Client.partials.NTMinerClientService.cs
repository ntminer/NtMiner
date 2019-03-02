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

            public void CloseNTMinerAsync(string clientHost, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            RequestBase request = new RequestBase();
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:3336/api/MinerClient/CloseNTMiner", request);
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

            public void StartMineAsync(string clientHost, Guid workId, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            StartMineRequest request = new StartMineRequest() {
                                LoginName = SingleUser.LoginName,
                                WorkId = workId
                            };
                            request.SignIt(SingleUser.PasswordSha1Sha1);
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:3336/api/MinerClient/StartMine", request);
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

            public void StopMineAsync(string clientHost, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            StopMineRequest request = new StopMineRequest() {
                                LoginName = SingleUser.LoginName
                            };
                            request.SignIt(SingleUser.PasswordSha1Sha1);
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:3336/api/MinerClient/StopMine", request);
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

            public void SetMinerNameAsync(string clientHost, string minerName, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            SetMinerNameRequest request = new SetMinerNameRequest() {
                                LoginName = SingleUser.LoginName,
                                MinerName = minerName
                            };
                            request.SignIt(SingleUser.PasswordSha1Sha1);
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:3336/api/MinerClient/SetMinerName", request);
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

            public void SetMinerProfilePropertyAsync(string clientHost, string propertyName, object value, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            SetMinerProfilePropertyRequest request = new SetMinerProfilePropertyRequest {
                                LoginName = SingleUser.LoginName,
                                PropertyName = propertyName,
                                Value = value
                            };
                            request.SignIt(SingleUser.PasswordSha1Sha1);
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:3336/api/MinerClient/SetMinerProfileProperty", request);
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
