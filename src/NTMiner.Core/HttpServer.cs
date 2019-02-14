using System;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace NTMiner {
    public static class HttpServer {
        private static HttpSelfHostServer _httpServer;
        public static void Start(string baseAddress) {
            var config = new HttpSelfHostConfiguration(baseAddress);
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Routes.MapHttpRoute("API Default", "api/{controller}/{action}");
            _httpServer = new HttpSelfHostServer(config);
            _httpServer.OpenAsync().Wait();
        }

        public static void Stop() {
            var tmp = _httpServer;
            if (tmp != null) {
                _httpServer = null;
                tmp.Dispose();
            }
        }
    }
}
