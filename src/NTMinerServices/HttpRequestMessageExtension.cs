using System.Net.Http;
using System.Web;

namespace NTMiner {
    public static class HttpRequestMessageExtension {
        public static string GetWebClientIp(this HttpRequestMessage request) {
            string ip;
            if (request.Properties.ContainsKey("MS_HttpContext")) {
                ip = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (HttpContext.Current != null) {
                ip = HttpContext.Current.Request.UserHostAddress;
            }
            else {
                ip = string.Empty;
            }
            if (ip == "::1") {
                ip = "127.0.0.1";
            }
            return ip;
        }
    }
}