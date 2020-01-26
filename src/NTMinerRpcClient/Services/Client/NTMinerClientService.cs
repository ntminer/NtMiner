using NTMiner.Controllers;
using NTMiner.Daemon;
using NTMiner.MinerClient;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner.Services.Client {
    public class MinerClientService {
        public static readonly MinerClientService Instance = new MinerClientService();

        private readonly string _controllerName = RpcRoot.GetControllerName<IMinerClientController>();
        private MinerClientService() {
        }

        #region Localhost
        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void ShowMainWindowAsync(int clientPort, Action<bool, Exception> callback) {
            RpcRoot.PostAsync("localhost", clientPort, _controllerName, nameof(IMinerClientController.ShowMainWindow), callback);
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
            RpcRoot.PostAsync("localhost", NTKeyword.MinerClientPort, _controllerName, nameof(IMinerClientController.CloseNTMiner), new SignRequest { }, (ResponseBase response, Exception e) => {
                if (!response.IsSuccess()) {
                    try {
                        Windows.TaskKill.Kill(processName, waitForExit: true);
                    }
                    catch (Exception ex) {
                        Logger.ErrorDebugLine(ex);
                    }
                }
            }, timeountMilliseconds: 2000);
        }

        /// <summary>
        /// 本机网络调用
        /// </summary>
        public void RefreshAutoBootStartAsync() {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = RpcRoot.Create()) {
                        client.Timeout = TimeSpan.FromSeconds(3);
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://localhost:{NTKeyword.MinerClientPort.ToString()}/api/{_controllerName}/{nameof(IMinerClientController.RefreshAutoBootStart)}", null);
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
                Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.MinerClientPort.ToString()}/api/{_controllerName}/{nameof(IMinerClientController.StartMine)}", request);
                ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                return response;
            }
        }

        public void StopMineAsync(string clientIp, SignRequest request, Action<ResponseBase, Exception> callback) {
            RpcRoot.PostAsync(clientIp, NTKeyword.MinerClientPort, _controllerName, nameof(IMinerClientController.StopMine), request, callback);
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
                Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{clientIp}:{NTKeyword.MinerClientPort.ToString()}/api/{_controllerName}/{nameof(IMinerClientController.SetMinerProfileProperty)}", request);
                ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                return response;
            }
        }

        public Task<SpeedData> GetSpeedAsync(string clientIp, Action<SpeedData, Exception> callback) {
            return Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = RpcRoot.Create()) {
                        client.Timeout = TimeSpan.FromSeconds(3);
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://{clientIp}:{NTKeyword.MinerClientPort.ToString()}/api/{_controllerName}/{nameof(IMinerClientController.GetSpeed)}", null);
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
