using NTMiner.Controllers;
using NTMiner.Daemon;
using NTMiner.MinerClient;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Client {
        public partial class MinerClientServiceFace {
            public static readonly MinerClientServiceFace Instance = new MinerClientServiceFace();
            private static readonly string s_controllerName = ControllerUtil.GetControllerName<IMinerClientController>();

            private MinerClientServiceFace() {
            }

            #region Localhost
            /// <summary>
            /// 本机网络调用
            /// </summary>
            public void ShowMainWindowAsync(int clientPort, Action<bool, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = RpcRoot.Create()) {
                            Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://localhost:{clientPort.ToString()}/api/{s_controllerName}/{nameof(IMinerClientController.ShowMainWindow)}", null);
                            bool response = getHttpResponse.Result.Content.ReadAsAsync<bool>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(false, e);
                    }
                });
            }

            /// <summary>
            /// 本机同步网络调用
            /// </summary>
            public void CloseNTMiner() {
                string location = NTMinerRegistry.GetLocation();
                if (string.IsNullOrEmpty(location) || !File.Exists(location)) {
                    return;
                }
                string processName = Path.GetFileNameWithoutExtension(location);
                if (Process.GetProcessesByName(processName).Length == 0) {
                    return;
                }
                bool isClosed = false;
                try {
                    using (HttpClient client = RpcRoot.Create()) {
                        client.Timeout = TimeSpan.FromSeconds(2);
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://localhost:{NTKeyword.MinerClientPort.ToString()}/api/{s_controllerName}/{nameof(IMinerClientController.CloseNTMiner)}", new SignRequest { });
                        ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        isClosed = response.IsSuccess();
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                if (!isClosed) {
                    try {
                        Windows.TaskKill.Kill(processName, waitForExit: true);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                }
            }

            /// <summary>
            /// 本机网络调用
            /// </summary>
            public void RefreshAutoBootStartAsync() {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = RpcRoot.Create()) {
                            client.Timeout = TimeSpan.FromSeconds(3);
                            Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://localhost:{NTKeyword.MinerClientPort.ToString()}/api/{s_controllerName}/{nameof(IMinerClientController.RefreshAutoBootStart)}", null);
                            Write.DevDebug($"{nameof(RefreshAutoBootStartAsync)} {getHttpResponse.Result.ReasonPhrase}");
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                });
            }
            #endregion

            #region ClientIp
            public void StartMineAsync(string clientIp, WorkRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        var response = StartMine(clientIp, request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }

            public ResponseBase StartMine(string clientIp, WorkRequest request) {
                using (HttpClient client = RpcRoot.Create()) {
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.MinerClientPort.ToString()}/api/{s_controllerName}/{nameof(IMinerClientController.StartMine)}", request);
                    ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public void StopMineAsync(string clientIp, SignRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = RpcRoot.Create()) {
                            Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.MinerClientPort.ToString()}/api/{s_controllerName}/{nameof(IMinerClientController.StopMine)}", request);
                            ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }

            public void SetMinerProfilePropertyAsync(string clientIp, SetClientMinerProfilePropertyRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        var response = SetMinerProfileProperty(clientIp, request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }

            public ResponseBase SetMinerProfileProperty(string clientIp, SetClientMinerProfilePropertyRequest request) {
                using (HttpClient client = RpcRoot.Create()) {
                    client.Timeout = TimeSpan.FromSeconds(3);
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.MinerClientPort.ToString()}/api/{s_controllerName}/{nameof(IMinerClientController.SetMinerProfileProperty)}", request);
                    ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public Task<SpeedData> GetSpeedAsync(string clientHost, Action<SpeedData, Exception> callback) {
                return Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = RpcRoot.Create()) {
                            client.Timeout = TimeSpan.FromSeconds(3);
                            Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://{clientHost}:{NTKeyword.MinerClientPort.ToString()}/api/{s_controllerName}/{nameof(IMinerClientController.GetSpeed)}", null);
                            SpeedData data = getHttpResponse.Result.Content.ReadAsAsync<SpeedData>().Result;
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
            #endregion
        }
    }
}
