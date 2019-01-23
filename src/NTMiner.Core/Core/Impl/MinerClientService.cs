using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace NTMiner.Core.Impl {
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class MinerClientService : IMinerClientService {
        private static T DecryptDeserialize<T>(string desKey, string data) {
            if (string.IsNullOrEmpty(desKey) || string.IsNullOrEmpty(data)) {
                return default(T);
            }
            desKey = Security.RSAHelper.DecryptString(desKey, Server.MinerServerPubKey);
            string json = Security.AESHelper.Decrypt(data, desKey);
            if (string.IsNullOrEmpty(json)) {
                return default(T);
            }
            return Global.JsonSerializer.Deserialize<T>(json);
        }

        public bool ShowMainWindow() {
            Global.Execute(new ShowMainWindowCommand());
            return true;
        }

        public void StartMine(string desKey, string data) {
            if (string.IsNullOrEmpty(desKey) || string.IsNullOrEmpty(data)) {
                return;
            }
            Dictionary<string, object> input = DecryptDeserialize<Dictionary<string, object>>(desKey, data);
            if (!input.ContainsKey("workId") || !input.ContainsKey("time")) {
                return;
            }
            Guid workId = input.GetGuidValue("workId");
            DateTime time = (DateTime)input["time"];
            if (time.AddSeconds(Global.DesyncSeconds) < DateTime.Now) {
                return;
            }
            NTMinerRoot.Current.StartMine(workId);
        }

        public void StopMine(string desKey, string data) {
            if (string.IsNullOrEmpty(desKey) || string.IsNullOrEmpty(data)) {
                return;
            }
            Dictionary<string, object> input = DecryptDeserialize<Dictionary<string, object>>(desKey, data);
            if (!input.ContainsKey("time")) {
                return;
            }
            DateTime time = (DateTime)input["time"];
            if (time.AddSeconds(Global.DesyncSeconds) < DateTime.Now) {
                return;
            }
            NTMinerRoot.Current.StopMineAsync();
        }

        public void SetMinerProfileProperty(string desKey, string data) {
            if (string.IsNullOrEmpty(desKey) || string.IsNullOrEmpty(data)) {
                return;
            }
            Dictionary<string, object> input = DecryptDeserialize<Dictionary<string, object>>(desKey, data);
            if (!input.ContainsKey("propertyName")
                || !input.ContainsKey("value")
                || !input.ContainsKey("time")) {
                return;
            }
            string propertyName = (string)input["propertyName"];
            object value = (object)input["value"];
            DateTime time = (DateTime)input["time"];
            if (time.AddSeconds(Global.DesyncSeconds) < DateTime.Now) {
                return;
            }
            if (string.IsNullOrEmpty(propertyName)) {
                return;
            }
            NTMinerRoot.Current.SetMinerProfileProperty(propertyName, value);
        }

        public void Dispose() {
            
        }
    }
}
