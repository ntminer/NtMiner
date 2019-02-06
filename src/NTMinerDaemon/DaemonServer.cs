using System.Web.Http;
using System.Web.Http.SelfHost;

namespace NTMiner {
    public static class DaemonServer {
        private static HttpSelfHostServer _httpServer;
        public static void StartAsync() {
            var config = new HttpSelfHostConfiguration("http://localhost:3337");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });
            _httpServer = new HttpSelfHostServer(config);
            _httpServer.OpenAsync().Wait();
        }

        public static void Stop() {
            if (_httpServer != null) {
                _httpServer.Dispose();
            }
        }
    }
}
