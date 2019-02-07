using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public static readonly ControlCenterServiceFace ControlCenterService = ControlCenterServiceFace.Instance;
        public static readonly AppSettingServiceFace AppSettingService = AppSettingServiceFace.Instance;
        public static readonly FileUrlServiceFace FileUrlService = FileUrlServiceFace.Instance;
        public static readonly TimeServiceFace TimeService = TimeServiceFace.Instance;
        public static readonly ProfileServiceFace ProfileService = ProfileServiceFace.Instance;
        public static readonly ReportServiceFace ReportService = ReportServiceFace.Instance;
        public static string LoginName { get; set; }

        private static string _passwordSha1;
        public static string PasswordSha1 {
            get { return _passwordSha1; }
            set {
                _passwordSha1 = value;
                PasswordSha1Sha1 = HashUtil.Sha1(value);
            }
        }

        public static string PasswordSha1Sha1 {
            get; private set;
        }

        public static string MinerServerHost {
            get { return NTMinerRegistry.GetMinerServerHost(); }
            set {
                NTMinerRegistry.SetMinerServerHost(value);
            }
        }

        public static int MinerServerPort {
            get {
                return 3339;
            }
        }

        private static readonly string baseUrl = $"http://{MinerServerHost}:{MinerServerPort}/api";
        public static T Request<T>(string controller, string action, object param) {
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/{controller}/{action}", param);
                    T response = message.Result.Content.ReadAsAsync<T>().Result;
                    return response;
                }
            }
            catch (Exception e) {
                Global.WriteDevLine(e.Message + e.StackTrace);
                return default(T);
            }
        }
    }
}
