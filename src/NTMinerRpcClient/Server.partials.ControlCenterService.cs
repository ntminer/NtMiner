using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public class ControlCenterServiceFace {
            public static readonly ControlCenterServiceFace Instance = new ControlCenterServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IControlCenterController>();

            private ControlCenterServiceFace() {
            }

            #region GetServicesVersionAsync
            public void GetServicesVersionAsync(Action<string, Exception> callback) {
                Process[] processes = Process.GetProcessesByName("NTMinerServices");
                if (processes.Length == 0) {
                    callback?.Invoke(string.Empty, null);
                }
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
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
            public void CloseServices() {
                try {
                    Process[] processes = Process.GetProcessesByName("NTMinerServices");
                    if (processes.Length == 0) {
                        return;
                    }
                    using (HttpClient client = new HttpClient()) {
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
                PostAsync(SControllerName, nameof(IControlCenterController.ActiveControlCenterAdmin), null, password, callback);
            }
            #endregion

            #region LoginAsync
            public void LoginAsync(string loginName, string password, Action<ResponseBase, Exception> callback) {
                SignRequest request = new SignRequest() {
                };
                PostAsync(SControllerName, nameof(IControlCenterController.LoginControlCenter), request.ToQuery(loginName, password), request, callback);
            }
            #endregion

            #region GetLatestSnapshotsAsync
            public void GetLatestSnapshotsAsync(
                int limit,
                Action<GetCoinSnapshotsResponse, Exception> callback) {
                GetCoinSnapshotsRequest request = new GetCoinSnapshotsRequest {
                    Limit = limit
                };
                PostAsync(SControllerName, nameof(IControlCenterController.LatestSnapshots), request.ToQuery(SingleUser.LoginName, SingleUser.PasswordSha1), request, callback);
            }
            #endregion
        }
    }
}