using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace NTMiner {
    [DataContract]
    public class LoadClientsRequest : RequestBase, ISignatureRequest {
        public LoadClientsRequest() {
            ClientIds = new List<Guid>();
        }

        [DataMember]
        public string LoginName { get; set; }
        [DataMember]
        public List<Guid> ClientIds { get; set; }
        [DataMember]
        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        public string GetSign(string password) {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(MessageId)).Append(MessageId)
                .Append(nameof(LoginName)).Append(LoginName)
                .Append(nameof(ClientIds)).Append(string.Join(",", ClientIds))
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong())
                .Append(nameof(UserData.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }
    }
}
