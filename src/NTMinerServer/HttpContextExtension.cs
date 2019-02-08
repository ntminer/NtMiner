using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;

namespace NTMiner {
    public static class HttpContextExtension {
        /// <summary>
        /// 获取web客户端ip
        /// </summary>
        /// <returns></returns>
        public static string GetWebClientIp(this HttpRequestMessage request) {
            string ip;
            if (request.Properties.ContainsKey("MS_HttpContext")) {
                ip = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name)) {
                RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                ip = prop.Address;
            }
            else if (HttpContext.Current != null) {
                ip = HttpContext.Current.Request.UserHostAddress;
            }
            else {
                ip = string.Empty;
            }
            return ip;
        }
    }
}