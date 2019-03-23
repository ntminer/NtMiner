using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public static readonly ControlCenterServiceFace ControlCenterService = ControlCenterServiceFace.Instance;
        public static readonly AppSettingServiceFace AppSettingService = AppSettingServiceFace.Instance;
        public static readonly ReportServiceFace ReportService = ReportServiceFace.Instance;
        public static readonly WrapperMinerClientServiceFace MinerClientService = WrapperMinerClientServiceFace.Instance;

        public static string ControlCenterHost {
            get { return NTMinerRegistry.GetControlCenterHost(); }
            set {
                NTMinerRegistry.SetControlCenterHost(value);
            }
        }

        private static void PostAsync<T>(string controller, string action, object param, Action<T, Exception> callback) where T : class {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message =
                            client.PostAsJsonAsync($"http://{ControlCenterHost}:{WebApiConst.ControlCenterPort}/api/{controller}/{action}", param);
                        T response = message.Result.Content.ReadAsAsync<T>().Result;
                        callback?.Invoke(response, null);
                    }
                }
                catch (Exception e) {
                    e = e.GetInnerException();
                    callback?.Invoke(null, e);
                }
            });
        }

        private static T Post<T>(string controller, string action, object param) where T : class {
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{ControlCenterHost}:{WebApiConst.ControlCenterPort}/api/{controller}/{action}", param);
                    T response = message.Result.Content.ReadAsAsync<T>().Result;
                    return response;
                }
            }
            catch {
                return null;
            }
        }

        private static void GetAsync<T>(string controller, string action, Dictionary<string, string> param, Action<T, Exception> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        string queryString = string.Empty;
                        if (param != null && param.Count != 0) {
                            queryString = "?" + string.Join("&", param.Select(a => a.Key + "=" + a.Value));
                        }

                        Task<HttpResponseMessage> message =
                            client.GetAsync($"http://{ControlCenterHost}:{WebApiConst.ControlCenterPort}/api/{controller}/{action}{queryString}");
                        T response = message.Result.Content.ReadAsAsync<T>().Result;
                        callback?.Invoke(response, null);
                    }
                }
                catch (Exception e) {
                    e = e.GetInnerException();
                    callback?.Invoke(default(T), e);
                }
            });
        }
    }
}
