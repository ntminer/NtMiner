using System;
using System.Collections.Generic;
using System.Text;

namespace NTMiner.MinerServer {
    public class UpdateClientPropertiesRequest : RequestBase, ISignatureRequest {
        public UpdateClientPropertiesRequest() {
            this.Values = new Dictionary<string, object>();
        }
        public string LoginName { get; set; }
        public Guid ClientId { get; set; }
        public Dictionary<string, object> Values { get; set; }
        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        public string GetSign(string password) {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(MessageId)).Append(MessageId)
                .Append(nameof(LoginName)).Append(LoginName)
                .Append(nameof(ClientId)).Append(ClientId)
                .Append(nameof(Values)).Append(GetValuesString())
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong())
                .Append(nameof(UserData.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }

        private string GetValuesString() {
            if (Values == null || Values.Count == 0) {
                return string.Empty;
            }
            return $"{string.Join(",", Values.Keys)}:{string.Join(",", Values.Values)}";
        }
    }
}
