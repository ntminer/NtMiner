using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Server {
        public class ControlCenterServiceFace {
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IControlCenterController>();

            private readonly string _host;
            private readonly int _port;
            public ControlCenterServiceFace(string host, int port) {
                _host = host;
                _port = port;
            }

            #region GetServicesVersionAsync
            /// <summary>
            /// 本机网络调用
            /// </summary>
            /// <param name="callback"></param>
            public void GetServicesVersionAsync(Action<string, Exception> callback) {
                Process[] processes = Process.GetProcessesByName("NTMinerServices");
                if (processes.Length == 0) {
                    callback?.Invoke(string.Empty, null);
                }
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = RpcRoot.Create()) {
                            Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://localhost:{NTKeyword.ControlCenterPort.ToString()}/api/{SControllerName}/{nameof(IControlCenterController.GetServicesVersion)}", null);
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
                    Process[] processes = Process.GetProcessesByName("NTMinerServices");
                    if (processes.Length == 0) {
                        return;
                    }
                    using (HttpClient client = RpcRoot.Create()) {
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://localhost:{NTKeyword.ControlCenterPort.ToString()}/api/{SControllerName}/{nameof(IControlCenterController.CloseServices)}", null);
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
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IControlCenterController.ActiveControlCenterAdmin), password, callback);
            }
            #endregion

            #region LoginAsync
            public void LoginAsync(string loginName, string password, Action<ResponseBase, Exception> callback) {
                VirtualRoot.SetRpcUser(new User.RpcUser(loginName, password));
                SignRequest request = new SignRequest() {
                };
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IControlCenterController.LoginControlCenter), request, request, callback);
            }
            #endregion

            #region GetLatestSnapshotsAsync
            public void GetLatestSnapshotsAsync(
                int limit,
                Action<GetCoinSnapshotsResponse, Exception> callback) {
                GetCoinSnapshotsRequest request = new GetCoinSnapshotsRequest {
                    Limit = limit
                };
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IControlCenterController.LatestSnapshots), request, request, callback);
            }
            #endregion
        }
    }
}