using NTMiner.Core;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class MinerClientService {
        public static readonly MinerClientService Instance = new MinerClientService();

        private MinerClientService() {
        }

        private IMinerClientService CreateService(string host) {
            return ChannelFactory.CreateChannel<IMinerClientService>(host, Global.ClientPort);
        }

        public bool ShowMainWindow(string host) {
            try {
                using (var service = CreateService(host)) {
                    return service.ShowMainWindow();
                }
            }
            catch (CommunicationException e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return false;
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return false;
            }
        }

        public void StartMineAsync(string host, string pubKey, Guid workId, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (var service = CreateService(host)) {
                        service.StartMine(workId, DateTime.Now);
                    }
                    callback?.Invoke(true);
                }
                catch (CommunicationException e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void StopMineAsync(string host, string pubKey, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (var service = CreateService(host)) {
                        service.StopMine(DateTime.Now);
                    }
                    callback?.Invoke(true);
                }
                catch (CommunicationException e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void SetMinerProfilePropertyAsync(string host, string pubKey, string propertyName, object value, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (var service = CreateService(host)) {
                        service.SetMinerProfileProperty(propertyName, value, DateTime.Now);
                    }
                    callback?.Invoke(true);
                }
                catch (CommunicationException e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void Dispose() {
            // nothing need todo
        }
    }
}
