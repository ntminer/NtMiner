using NTMiner.ServiceContracts.MinerClient;
using System;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class MinerClientService {
        public static readonly MinerClientService Instance = new MinerClientService();

        private MinerClientService() {
        }

        private IMinerClientService CreateService(string host) {
            return new EmptyMinerClientService();
        }

        public bool ShowMainWindow(string host) {
            try {
                using (var service = CreateService(host)) {
                    return service.ShowMainWindow();
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return false;
            }
        }

        public void StartMineAsync(string host, Guid workId, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (var service = CreateService(host)) {
                        StartMineRequest request = new StartMineRequest() {
                            MessageId = Guid.NewGuid(),
                            LoginName = "admin",
                            WorkId = workId,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(Server.PasswordSha1Sha1);
                        service.StartMine(request);
                    }
                    callback?.Invoke(true);
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void StopMineAsync(string host, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (var service = CreateService(host)) {
                        service.StopMine(DateTime.Now);
                    }
                    callback?.Invoke(true);
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void SetMinerProfilePropertyAsync(string host, string propertyName, object value, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (var service = CreateService(host)) {
                        service.SetMinerProfileProperty(propertyName, value, DateTime.Now);
                    }
                    callback?.Invoke(true);
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }
    }

    public class EmptyMinerClientService : IMinerClientService {
        public void Dispose() {
            
        }

        public void SetMinerProfileProperty(string propertyName, object value, DateTime timestamp) {
            
        }

        public bool ShowMainWindow() {
            return true;
        }

        public ResponseBase StartMine(StartMineRequest request) {
            return ResponseBase.Ok(request.MessageId);
        }

        public void StopMine(DateTime timestamp) {
            
        }
    }
}
