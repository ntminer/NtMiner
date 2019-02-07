using System;
using System.Diagnostics;
using System.Threading;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace NTMiner {
    class Program {
        private static Mutex mutexApp;

        private static HttpSelfHostServer _httpServer;
        static void Main(string[] args) {
            try {
                bool mutexCreated;
                try {
                    mutexApp = new Mutex(true, "NTMinerDaemonAppMutex", out mutexCreated);
                }
                catch {
                    mutexCreated = false;
                }
                if (mutexCreated) {
                    NTMinerRegistry.SetAutoBoot("NTMinerDaemon", true);
                    bool isAutoBoot = NTMinerRegistry.GetIsAutoBoot();
                    if (isAutoBoot) {
                        string location = NTMinerRegistry.GetLocation();
                        if (!string.IsNullOrEmpty(location)) {
                            string arguments = NTMinerRegistry.GetArguments();
                            try {
                                Process.Start(location, arguments);
                            }
                            catch (Exception e) {
                                Global.Logger.ErrorDebugLine(e.Message, e);
                            }
                        }
                    }
                    Run();
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }
        private static void Run() {
            try {
                var config = new HttpSelfHostConfiguration("http://localhost:3337");
                config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
                config.Routes.MapHttpRoute(
                    "API Default", "api/{controller}/{action}");
                _httpServer = new HttpSelfHostServer(config);
                _httpServer.OpenAsync().Wait();
                while (true) {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
            finally {
                if (_httpServer != null) {
                    _httpServer.Dispose();
                }
            }
        }
    }
}
