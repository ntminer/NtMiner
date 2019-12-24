using NTMiner.Controllers;
using NTMiner.Daemon;
using NTMiner.JsonDb;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Client {
        public class NTMinerDaemonServiceFace {
            public static readonly NTMinerDaemonServiceFace Instance = new NTMinerDaemonServiceFace();
            private static readonly string s_controllerName = ControllerUtil.GetControllerName<INTMinerDaemonController>();

            private NTMinerDaemonServiceFace() { }

            #region Localhost
            public void CloseDaemon() {
                try {
                    using (HttpClient client = RpcRoot.Create()) {
                        client.Timeout = TimeSpan.FromSeconds(2);
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://localhost:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{s_controllerName}/{nameof(INTMinerDaemonController.CloseDaemon)}", null);
                        Write.DevDebug($"{nameof(CloseDaemon)} {getHttpResponse.Result.ReasonPhrase}");
                    }
                }
                catch(Exception e) {
                    Write.DevException(e);
                }
            }

            public void SetWalletAsync(SetWalletRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = RpcRoot.Create()) {
                            client.Timeout = TimeSpan.FromSeconds(2);
                            Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://localhost:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{s_controllerName}/{nameof(INTMinerDaemonController.SetWallet)}", request);
                            ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region ClientIp
            public ResponseBase EnableWindowsRemoteDesktop(string clientIp, SignRequest request) {
                using (HttpClient client = RpcRoot.Create()) {
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{s_controllerName}/{nameof(INTMinerDaemonController.EnableWindowsRemoteDesktop)}", request);
                    ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public void GetGpuProfilesJsonAsync(string clientIp, Action<GpuProfilesJsonDb, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = RpcRoot.Create()) {
                            client.Timeout = TimeSpan.FromSeconds(3);
                            Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{s_controllerName}/{nameof(INTMinerDaemonController.GetGpuProfilesJson)}", null);
                            string json = getHttpResponse.Result.Content.ReadAsAsync<string>().Result;
                            GpuProfilesJsonDb data = VirtualRoot.JsonSerializer.Deserialize<GpuProfilesJsonDb>(json);
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

            public void SaveGpuProfilesJsonAsync(string clientIp, string json) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = RpcRoot.Create()) {
                            client.Timeout = TimeSpan.FromSeconds(3);
                            HttpContent content = new StringContent(json);
                            Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{s_controllerName}/{nameof(INTMinerDaemonController.SaveGpuProfilesJson)}", content);
                            Write.DevDebug($"{nameof(SaveGpuProfilesJsonAsync)} {getHttpResponse.Result.ReasonPhrase}");
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                });
            }

            public void SetAutoBootStartAsync(string clientIp, bool autoBoot, bool autoStart) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = RpcRoot.Create()) {
                            client.Timeout = TimeSpan.FromSeconds(3);
                            Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{s_controllerName}/{nameof(INTMinerDaemonController.SetAutoBootStart)}?autoBoot={autoBoot}&autoStart={autoStart}", null);
                            Write.DevDebug($"{nameof(SetAutoBootStartAsync)} {getHttpResponse.Result.ReasonPhrase}");
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                });
            }

            public ResponseBase RestartWindows(string clientIp, SignRequest request) {
                using (HttpClient client = RpcRoot.Create()) {
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{s_controllerName}/{nameof(INTMinerDaemonController.RestartWindows)}", request);
                    ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public ResponseBase ShutdownWindows(string clientIp, SignRequest request) {
                using (HttpClient client = RpcRoot.Create()) {
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{s_controllerName}/{nameof(INTMinerDaemonController.ShutdownWindows)}", request);
                    ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public ResponseBase RestartNTMiner(string clientIp, WorkRequest request) {
                using (HttpClient client = RpcRoot.Create()) {
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{s_controllerName}/{nameof(INTMinerDaemonController.RestartNTMiner)}", request);
                    ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public ResponseBase UpgradeNTMiner(string clientIp, UpgradeNTMinerRequest request) {
                using (HttpClient client = RpcRoot.Create()) {
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{s_controllerName}/{nameof(INTMinerDaemonController.UpgradeNTMiner)}", request);
                    ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public ResponseBase StartMine(string clientIp, WorkRequest request) {
                using (HttpClient client = RpcRoot.Create()) {
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{s_controllerName}/{nameof(INTMinerDaemonController.StartMine)}", request);
                    ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }

            public ResponseBase StopMine(string clientIp, SignRequest request) {
                using (HttpClient client = RpcRoot.Create()) {
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{s_controllerName}/{nameof(INTMinerDaemonController.StopMine)}", request);
                    ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                    return response;
                }
            }
            #endregion
        }
    }
}
