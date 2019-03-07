using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public static readonly ControlCenterServiceFace ControlCenterService = ControlCenterServiceFace.Instance;
        public static readonly AppSettingServiceFace AppSettingService = AppSettingServiceFace.Instance;
        public static readonly FileUrlServiceFace FileUrlService = FileUrlServiceFace.Instance;
        public static readonly ProfileServiceFace ProfileService = ProfileServiceFace.Instance;
        public static readonly ReportServiceFace ReportService = ReportServiceFace.Instance;
        public static readonly WrapperMinerClientServiceFace MinerClientService = WrapperMinerClientServiceFace.Instance;
        public static readonly OverClockDataServiceFace OverClockDataService = OverClockDataServiceFace.Instance;

        public static string MinerServerHost {
            get { return NTMinerRegistry.GetMinerServerHost(); }
            set {
                NTMinerRegistry.SetMinerServerHost(value);
            }
        }

        private static readonly string s_baseUrl = $"http://{MinerServerHost}:{{WebApiConst.MinerServerPort}}/api";
        public static void RequestAsync<T>(string controller, string action, object param, Action<T, Exception> callback) where T : class {
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{s_baseUrl}/{controller}/{action}", param);
                    T response = message.Result.Content.ReadAsAsync<T>().Result;
                    callback?.Invoke(response, null);
                }
            }
            catch (Exception e) {
                callback?.Invoke(null, e);
            }
        }

        public static T Request<T>(string controller, string action, object param) where T : class {
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{s_baseUrl}/{controller}/{action}", param);
                    T response = message.Result.Content.ReadAsAsync<T>().Result;
                    return response;
                }
            }
            catch {
                return null;
            }
        }
    }
}
