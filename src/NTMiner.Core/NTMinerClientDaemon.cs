using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace NTMiner {
    public class NTMinerClientDaemon {
        public static readonly NTMinerClientDaemon Instance = new NTMinerClientDaemon();

        private NTMinerClientDaemon() { }

        private static INTMinerDaemonService CreateService(string clientHost, int clientPort) {
            ChannelFactory<INTMinerDaemonService> factory = null;
            INTMinerDaemonService channel = EmptyNTMinerDaemonService.Instance;
            try {
                var baseUri = new Uri($"http://{clientHost}:{clientPort}/Daemon/");
                factory = new ChannelFactory<INTMinerDaemonService>(ChannelFactory.BasicHttpBinding, new EndpointAddress(new Uri(baseUri, typeof(INTMinerDaemonService).Name)));
                //利用通道创建客户端代理
                channel = factory.CreateChannel();
            }
            catch (Exception e) {
                if (factory != null) {
                    factory.Abort();
                }
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
            return channel;
        }

        public void GetDaemonVersionAsync(string clientHost, int clientPort, Action<string> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (var client = CreateService(clientHost, clientPort)) {
                        callback?.Invoke(client.GetDaemonVersion());
                    }
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(string.Empty);
                }
            });
        }

        public void RestartWindowsAsync(string clientHost, int clientPort, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (var client = CreateService(clientHost, clientPort)) {
                        client.RestartWindows();
                    }
                    callback?.Invoke(true);
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void ShutdownWindowsAsync(string clientHost, int clientPort, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (var client = CreateService(clientHost, clientPort)) {
                        client.ShutdownWindows();
                    }
                    callback?.Invoke(true);
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void OpenNTMinerAsync(string clientHost, int clientPort, Guid workId, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (var client = CreateService(clientHost, clientPort)) {
                        client.OpenNTMiner(workId);
                    }
                    callback?.Invoke(true);
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void RestartNTMinerAsync(string clientHost, int clientPort, Guid workId, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (var client = CreateService(clientHost, clientPort)) {
                        client.RestartNTMiner(workId);
                    }
                    callback?.Invoke(true);
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void CloseNTMinerAsync(string clientHost, int clientPort, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (var client = CreateService(clientHost, clientPort)) {
                        client.CloseNTMiner();
                    }
                    callback?.Invoke(true);
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void IsNTMinerDaemonOnlineAsync(string clientHost, int clientPort, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (var client = CreateService(clientHost, clientPort)) {
                        callback?.Invoke(client.IsNTMinerDaemonOnline());
                    }
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void IsNTMinerOnlineAsync(string clientHost, int clientPort, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (var client = CreateService(clientHost, clientPort)) {
                        callback?.Invoke(client.IsNTMinerOnline());
                    }
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void StartAsync(Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    if (!NTMinerRoot.Current.IsMining) {
                        callback?.Invoke(false);
                        return;
                    }
                    var context = NTMinerRoot.Current.CurrentMineContext;
                    try {
                        using (var client = CreateService(Global.Localhost, Global.ClientPort)) {
                            client.StartNoDevFee(
                                context.Id.GetHashCode(), 
                                context.MinerName,
                                context.MainCoin.Code,
                                context.MainCoinWallet,
                                context.MainCoin.TestWallet,
                                context.Kernel.FullName);
                        }
                        callback?.Invoke(true);
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(false);
                    }
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void StopAsync(Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    if (NTMinerRoot.Current.IsMining) {
                        callback?.Invoke(false);
                        return;
                    }
                    using (var client = CreateService(Global.Localhost, Global.ClientPort)) {
                        client.StopNoDevFee();
                    }
                    callback?.Invoke(true);
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public class EmptyNTMinerDaemonService : INTMinerDaemonService {
            public static readonly INTMinerDaemonService Instance = new EmptyNTMinerDaemonService();

            public string GetDaemonVersion() {
                return string.Empty;
            }

            public void ShutdownWindows() {
                // noting need todo
            }

            public void RestartWindows() {
                // noting need todo
            }

            public void CloseNTMiner() {
                // noting need todo
            }

            public void OpenNTMiner(Guid workId) {
                // noting need todo
            }

            public void RestartNTMiner(Guid workId) {
                // noting need todo
            }

            public bool IsNTMinerDaemonOnline() {
                return false;
            }

            public bool IsNTMinerOnline() {
                return false;
            }

            public void UpgradeNTMiner(string ntminerUrl) {
                // noting need todo
            }

            public void StartNoDevFee(
                int contextId,
                string workerName,
                string coin,
                string ourWallet,
                string testWallet,
                string kernelName) {
                // noting need todo
            }

            public void StopNoDevFee() {
                // noting need todo
            }

            public void Dispose() {
                // noting need todo
            }
        }
    }
}
