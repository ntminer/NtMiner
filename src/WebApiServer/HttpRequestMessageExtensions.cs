using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;

namespace NTMiner {
    public static class HttpRequestMessageExtensions {
        public static string GetRemoteIp(this HttpRequestMessage request) {
            string ip;
            if (request.Properties.TryGetValue("MS_HttpContext", out object obj) && obj is HttpContextWrapper context) {
                ip = context.Request.UserHostAddress;
            }
            else if (request.Properties.TryGetValue(RemoteEndpointMessageProperty.Name, out obj) && obj is RemoteEndpointMessageProperty prop) {
                ip = prop.Address;
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
