using System.Web.Http;
using System.Web.Http.SelfHost;

namespace NTMiner {
    public static class HttpServer {
        private static HttpSelfHostServer s_httpServer;
        public static void Start(string baseAddress) {
            if (s_httpServer != null) {
                return;
            }
            var config = new HttpSelfHostConfiguration(baseAddress);
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Routes.MapHttpRoute("API Default", "api/{controller}/{action}");
            s_httpServer = new HttpSelfHostServer(config);
            s_httpServer.OpenAsync().Wait();
            VirtualRoot.AddEventPath<AppExitEvent>("退出HttpServer", LogEnum.None, action: message => {
                var tmp = s_httpServer;
                if (tmp != null) {
                    s_httpServer = null;
                    tmp.Dispose();
                }
            }, typeof(HttpServer));
        }
    }
}
