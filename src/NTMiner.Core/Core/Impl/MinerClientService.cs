using System;
using System.ServiceModel;

namespace NTMiner.Core.Impl {
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class MinerClientService : IMinerClientService {
        public bool ShowMainWindow() {
            Global.Execute(new ShowMainWindowCommand());
            return true;
        }

        public void StartMine(Guid workId, DateTime timestamp) {
            if (timestamp.AddSeconds(Global.DesyncSeconds) < DateTime.Now) {
                return;
            }
            NTMinerRoot.Current.StartMine(workId);
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
