using NTMiner.Controllers;
using NTMiner.Daemon;
using NTMiner.MinerClient;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Client {
        public partial class MinerClientServiceFace {
            public static readonly MinerClientServiceFace Instance = new MinerClientServiceFace();
            private static readonly string s_controllerName = ControllerUtil.GetControllerName<IMinerClientController>();

            private MinerClientServiceFace() {
            }

            public void ShowMainWindowAsync(int clientPort, Action<bool, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:{clientPort}/api/{s_controllerName}/{nameof(IMinerClientController.ShowMainWindow)}", null);
                            bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        e = e.GetInnerException();
                        callback?.Invoke(false, e);
                    }
                });
            }

            public void CloseNTMiner() {
                bool isClosed = false;
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:{WebApiConst.MinerClientPort}/api/MinerClient/CloseNTMiner", new SignatureRequest {});
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        isClosed = response.IsSuccess();
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                }
                if (!isClosed) {
                    try {
                        string location = NTMinerRegistry.GetLocation();
                        if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                            string processName = Path.GetFileNameWithoutExtension(location);
                            Windows.TaskKill.Kill(processName);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                }
            }

            public void StartMineAsync(string clientIp, WorkRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        var response = StartMine(clientIp, request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        e = e.GetInnerException();
                        callback?.Invoke(null, e);
                    }
                });
            }

            public ResponseBase StartMine(string clientIp, WorkRequest request) {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientIp}:{WebApiConst.MinerClientPort}/api/{s_controllerName}/{nameof(IMinerClientController.StartMine)}", request);
                    ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public void StopMineAsync(string clientIp, SignatureRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        ResponseBase response = StopMine(clientIp, request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        e = e.GetInnerException();
                        callback?.Invoke(null, e);
                    }
                });
            }

            public ResponseBase StopMine(string clientIp, SignatureRequest request) {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientIp}:{WebApiConst.MinerClientPort}/api/{s_controllerName}/{nameof(IMinerClientController.StopMine)}", request);
                    ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public void SetMinerProfilePropertyAsync(string clientIp, SetClientMinerProfilePropertyRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        var response = SetMinerProfileProperty(clientIp, request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        e = e.GetInnerException();
                        callback?.Invoke(null, e);
                    }
                });
            }

            public ResponseBase SetMinerProfileProperty(string clientIp, SetClientMinerProfilePropertyRequest request) {
                using (HttpClient client = new HttpClient()) {
                    client.Timeout = TimeSpan.FromMilliseconds(3000);
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientIp}:{WebApiConst.MinerClientPort}/api/{s_controllerName}/{nameof(IMinerClientController.SetMinerProfileProperty)}", request);
                    ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public Task<SpeedData> GetSpeedAsync(string clientHost, Action<SpeedData, Exception> callback) {
                return Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            client.Timeout = TimeSpan.FromMilliseconds(3000);
                            Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:{WebApiConst.MinerClientPort}/api/{s_controllerName}/{nameof(IMinerClientController.GetSpeed)}", null);
                            SpeedData data = message.Result.Content.ReadAsAsync<SpeedData>().Result;
                            callback?.Invoke(data, null);
                            return data;
                        }
                    }
                    catch (Exception e) {
                        e = e.GetInnerException();
                        callback?.Invoke(null, e);
                        return null;
                    }
                });
            }

            public void RefreshAutoBootStartAsync() {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            client.Timeout = TimeSpan.FromMilliseconds(3000);
                            Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:{WebApiConst.MinerClientPort}/api/{s_controllerName}/{nameof(IMinerClientController.RefreshAutoBootStart)}", null);
                            Write.DevLine(message.Result.ReasonPhrase);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                });
            }

            public void OverClockAsync() {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            client.Timeout = TimeSpan.FromMilliseconds(3000);
                            Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:{WebApiConst.MinerClientPort}/api/{s_controllerName}/{nameof(IMinerClientController.OverClock)}", null);
                            Write.DevLine(message.Result.ReasonPhrase);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                });
            }
        }
    }
}
