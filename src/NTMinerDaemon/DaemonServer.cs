using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace NTMiner {
    public static class DaemonServer {
        private static List<ServiceHost> _serviceHosts = null;
        public static void Start() {
            try {
                string baseUrl = $"http://{Global.Localhost}:{Global.ClientPort}/Daemon/";
                ServiceHost minerServerServiceHost = new ServiceHost(typeof(NTMinerDaemonService));
                minerServerServiceHost.AddServiceEndpoint(typeof(INTMinerDaemonService), ChannelFactory.BasicHttpBinding, new Uri(new Uri(baseUrl), nameof(INTMinerDaemonService)));
                _serviceHosts = new List<ServiceHost>
                {
                    minerServerServiceHost
                };
                foreach (var serviceHost in _serviceHosts) {
                    ServiceMetadataBehavior serviceMetadata = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                    if (serviceMetadata == null) {
                        serviceMetadata = new ServiceMetadataBehavior();
                        serviceHost.Description.Behaviors.Add(serviceMetadata);
                    }
                    serviceMetadata.HttpGetEnabled = false;

                    serviceHost.Open();
                }

                Global.Logger.Info($"服务启动成功: {DateTime.Now}.");
                Global.Logger.Info("服务列表：");
                foreach (var serviceHost in _serviceHosts) {
                    foreach (var endpoint in serviceHost.Description.Endpoints) {
                        Global.Logger.Info(endpoint.Address.Uri.ToString());
                    }
                }
            }
            catch {
                Stop();
            }
        }

        public static void Stop() {
            if (_serviceHosts == null) {
                return;
            }
            foreach (var serviceHost in _serviceHosts) {
                serviceHost.Close();
            }
            _serviceHosts = null;
        }
    }
}
