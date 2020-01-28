using NTMiner.Controllers;
using NTMiner.Core.Daemon;
using NTMiner.JsonDb;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner.Services.Client {
    public class NTMinerDaemonService {
        public static readonly NTMinerDaemonService Instance = new NTMinerDaemonService();

        private readonly string _controllerName = RpcRoot.GetControllerName<INTMinerDaemonController>();
        private NTMinerDaemonService() { }

        #region Localhost
        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void CloseDaemon() {
            try {
                using (HttpClient client = RpcRoot.CreateHttpClient()) {
                    client.Timeout = TimeSpan.FromSeconds(2);
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://localhost:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{_controllerName}/{nameof(INTMinerDaemonController.CloseDaemon)}", null);
                    Write.DevDebug($"{nameof(CloseDaemon)} {getHttpResponse.Result.ReasonPhrase}");
                }
            }
            catch (Exception e) {
                Write.DevException(e);
            }
        }

        /// <summary>
        /// 本机网络调用
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void SetWalletAsync(SetWalletRequest request, Action<ResponseBase, Exception> callback) {
            RpcRoot.PostAsync("localhost", NTKeyword.NTMinerDaemonPort, _controllerName, nameof(INTMinerDaemonController.SetWallet), request, callback, timeountMilliseconds: 2000);
        }
        #endregion

        #region ClientIp
        public ResponseBase EnableWindowsRemoteDesktop(string clientIp, SignRequest request) {
            using (HttpClient client = RpcRoot.CreateHttpClient()) {
                Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{_controllerName}/{nameof(INTMinerDaemonController.EnableWindowsRemoteDesktop)}", request);
                ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                return response;
            }
        }

        public void GetGpuProfilesJsonAsync(string clientIp, Action<GpuProfilesJsonDb, Exception> callback) {
            RpcRoot.PostAsync(clientIp, NTKeyword.NTMinerDaemonPort, _controllerName, nameof(INTMinerDaemonController.GetGpuProfilesJson), null, (string json, Exception e) => {
                GpuProfilesJsonDb data = VirtualRoot.JsonSerializer.Deserialize<GpuProfilesJsonDb>(json);
                callback?.Invoke(data, null);
            }, timeountMilliseconds: 3000);
        }

        public void SaveGpuProfilesJsonAsync(string clientIp, string json) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = RpcRoot.CreateHttpClient()) {
                        client.Timeout = TimeSpan.FromSeconds(3);
                        HttpContent content = new StringContent(json);
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{_controllerName}/{nameof(INTMinerDaemonController.SaveGpuProfilesJson)}", content);
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
                    using (HttpClient client = RpcRoot.CreateHttpClient()) {
                        client.Timeout = TimeSpan.FromSeconds(3);
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{_controllerName}/{nameof(INTMinerDaemonController.SetAutoBootStart)}?autoBoot={autoBoot}&autoStart={autoStart}", null);
                        Write.DevDebug($"{nameof(SetAutoBootStartAsync)} {getHttpResponse.Result.ReasonPhrase}");
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
            });
        }

        public ResponseBase RestartWindows(string clientIp, SignRequest request) {
            using (HttpClient client = RpcRoot.CreateHttpClient()) {
                Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{_controllerName}/{nameof(INTMinerDaemonController.RestartWindows)}", request);
                ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                return response;
            }
        }

        public ResponseBase ShutdownWindows(string clientIp, SignRequest request) {
            using (HttpClient client = RpcRoot.CreateHttpClient()) {
                Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{_controllerName}/{nameof(INTMinerDaemonController.ShutdownWindows)}", request);
                ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                return response;
            }
        }

        public ResponseBase RestartNTMiner(string clientIp, WorkRequest request) {
            using (HttpClient client = RpcRoot.CreateHttpClient()) {
                Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{_controllerName}/{nameof(INTMinerDaemonController.RestartNTMiner)}", request);
                ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                return response;
            }
        }

        public ResponseBase UpgradeNTMiner(string clientIp, UpgradeNTMinerRequest request) {
            using (HttpClient client = RpcRoot.CreateHttpClient()) {
                Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{_controllerName}/{nameof(INTMinerDaemonController.UpgradeNTMiner)}", request);
                ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                return response;
            }
        }

        public ResponseBase StartMine(string clientIp, WorkRequest request) {
            using (HttpClient client = RpcRoot.CreateHttpClient()) {
                Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{_controllerName}/{nameof(INTMinerDaemonController.StartMine)}", request);
                ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                return response;
            }
        }

        public ResponseBase StopMine(string clientIp, SignRequest request) {
            using (HttpClient client = RpcRoot.CreateHttpClient()) {
                Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.NTMinerDaemonPort.ToString()}/api/{_controllerName}/{nameof(INTMinerDaemonController.StopMine)}", request);
                ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                return response;
            }
        }
        #endregion
    }
}
