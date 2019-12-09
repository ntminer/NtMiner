using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Server {
        public readonly ControlCenterServiceFace ControlCenterService = ControlCenterServiceFace.Instance;
        public readonly UserServiceFace UserService = UserServiceFace.Instance;
        public readonly MinerGroupServiceFace MinerGroupService = MinerGroupServiceFace.Instance;
        public readonly MineWorkServiceFace MineWorkService = MineWorkServiceFace.Instance;
        public readonly WalletServiceFace WalletService = WalletServiceFace.Instance;
        public readonly PoolServiceFace PoolService = PoolServiceFace.Instance;
        public readonly ColumnsShowServiceFace ColumnsShowService = ColumnsShowServiceFace.Instance;
        public readonly ClientServiceFace ClientService = ClientServiceFace.Instance;
        public readonly AppSettingServiceFace AppSettingService = AppSettingServiceFace.Instance;
        public readonly ReportServiceFace ReportService = ReportServiceFace.Instance;
        public readonly WrapperMinerClientServiceFace MinerClientService = WrapperMinerClientServiceFace.Instance;

        internal Server() { }

        private static void PostAsync<T>(string controller, string action, Dictionary<string, string> query, object param, Action<T, Exception> callback) where T : class {
            Task.Factory.StartNew(() => {
                try {
                    string queryString = string.Empty;
                    if (query != null && query.Count != 0) {
                        queryString = "?" + string.Join("&", query.Select(a => a.Key + "=" + a.Value));
                    }
                    string serverHost = NTMinerRegistry.GetControlCenterHost();
                    using (HttpClient client = RpcRoot.Create()) {
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{serverHost}:{NTKeyword.ControlCenterPort.ToString()}/api/{controller}/{action}{queryString}", param);
                        T response = getHttpResponse.Result.Content.ReadAsAsync<T>().Result;
                        callback?.Invoke(response, null);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(null, e);
                }
            });
        }

        private static T Post<T>(string controller, string action, Dictionary<string, string> query, object param, int? timeout = null) where T : class {
            try {
                string queryString = string.Empty;
                if (query != null && query.Count != 0) {
                    queryString = "?" + string.Join("&", query.Select(a => a.Key + "=" + a.Value));
                }
                string serverHost = NTMinerRegistry.GetControlCenterHost();
                using (HttpClient client = RpcRoot.Create()) {
                    if (timeout.HasValue) {
                        client.Timeout = TimeSpan.FromMilliseconds(timeout.Value);
                    }
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{serverHost}:{NTKeyword.ControlCenterPort.ToString()}/api/{controller}/{action}{queryString}", param);
                    T response = getHttpResponse.Result.Content.ReadAsAsync<T>().Result;
                    return response;
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return null;
            }
        }

        private static void GetAsync<T>(string controller, string action, Dictionary<string, string> param, Action<T, Exception> callback) {
            Task.Factory.StartNew(() => {
                try {
                    string serverHost = NTMinerRegistry.GetControlCenterHost();
                    using (HttpClient client = RpcRoot.Create()) {
                        string queryString = string.Empty;
                        if (param != null && param.Count != 0) {
                            queryString = "?" + string.Join("&", param.Select(a => a.Key + "=" + a.Value));
                        }

                        Task<HttpResponseMessage> message =
                            client.GetAsync($"http://{serverHost}:{NTKeyword.ControlCenterPort.ToString()}/api/{controller}/{action}{queryString}");
                        T response = message.Result.Content.ReadAsAsync<T>().Result;
                        callback?.Invoke(response, null);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(default, e);
                }
            });
        }
    }
}
