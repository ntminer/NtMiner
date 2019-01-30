using NTMiner.ServiceContracts.DataObjects;
using NTMiner.ServiceContracts.MinerClient;
using System;
using System.ServiceModel;

namespace NTMiner.Core.Impl {
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class MinerClientService : IMinerClientService {
        public bool ShowMainWindow() {
            Global.Execute(new ShowMainWindowCommand());
            return true;
        }

        public ResponseBase StartMine(StartMineRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(NTMinerRoot.Current.UserSet, out response)) {
                    return response;
                }
                NTMinerRoot.Current.StartMine(request.WorkId);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        public void StopMine(DateTime timestamp) {
            if (timestamp.AddSeconds(Global.DesyncSeconds) < DateTime.Now) {
                return;
            }
            NTMinerRoot.Current.StopMineAsync();
        }

        public void SetMinerProfileProperty(string propertyName, object value, DateTime timestamp) {
            if (timestamp.AddSeconds(Global.DesyncSeconds) < DateTime.Now) {
                return;
            }
            if (string.IsNullOrEmpty(propertyName)) {
                return;
            }
            NTMinerRoot.Current.SetMinerProfileProperty(propertyName, value);
        }

        public void Dispose() {
            // noting need todo
        }
    }
}
