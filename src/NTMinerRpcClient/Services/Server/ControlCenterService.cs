using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner.Services.Server {
    public class ControlCenterService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IControlCenterController>();

        public ControlCenterService() {
        }

        #region GetServicesVersionAsync
        /// <summary>
        /// 本机网络调用
        /// </summary>
        /// <param name="callback"></param>
        public void GetServicesVersionAsync(Action<string, Exception> callback) {
            Process[] processes = Process.GetProcessesByName(NTKeyword.NTMinerServicesProcessName);
            if (processes.Length == 0) {
                callback?.Invoke(string.Empty, null);
            }
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = RpcRoot.Create()) {
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://localhost:{NTKeyword.ControlCenterPort.ToString()}/api/{_controllerName}/{nameof(IControlCenterController.GetServicesVersion)}", null);
                        string response = getHttpResponse.Result.Content.ReadAsAsync<string>().Result;
                        callback?.Invoke(response, null);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(string.Empty, e);
                }
            });
        }
        #endregion

        #region CloseServices
        /// <summary>
        /// 本机同步网络调用
        /// </summary>
        public void CloseServices() {
            try {
                Process[] processes = Process.GetProcessesByName(NTKeyword.NTMinerServicesProcessName);
                if (processes.Length == 0) {
                    return;
                }
                using (HttpClient client = RpcRoot.Create()) {
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://localhost:{NTKeyword.ControlCenterPort.ToString()}/api/{_controllerName}/{nameof(IControlCenterController.CloseServices)}", null);
                    Write.DevDebug($"{nameof(CloseServices)} {getHttpResponse.Result.ReasonPhrase}");
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
        #endregion

        #region ActiveControlCenterAdminAsync
        public void ActiveControlCenterAdminAsync(string password, Action<ResponseBase, Exception> callback) {
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IControlCenterController.ActiveControlCenterAdmin), password, callback);
        }
        #endregion

        #region LoginAsync
        public void LoginAsync(string loginName, string password, Action<ResponseBase, Exception> callback) {
            VirtualRoot.SetRpcUser(new User.RpcUser(loginName, password));
            SignRequest request = new SignRequest() {
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IControlCenterController.LoginControlCenter), request, request, callback);
        }
        #endregion

        #region GetLatestSnapshotsAsync
        public void GetLatestSnapshotsAsync(
            int limit,
            Action<GetCoinSnapshotsResponse, Exception> callback) {
            GetCoinSnapshotsRequest request = new GetCoinSnapshotsRequest {
                Limit = limit
            };
            RpcRoot.PostAsync(NTMinerRegistry.GetControlCenterHost(), NTKeyword.ControlCenterPort, _controllerName, nameof(IControlCenterController.LatestSnapshots), request, request, callback);
        }
        #endregion
    }
}