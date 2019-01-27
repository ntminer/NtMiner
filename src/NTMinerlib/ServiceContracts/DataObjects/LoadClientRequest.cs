using System;
using System.Runtime.Serialization;
using System.Text;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class LoadClientRequest : RequestBase, ISignatureRequest {
        [DataMember]
        public string LoginName { get; set; }
        [DataMember]
        public Guid ClientId { get; set; }
        [DataMember]
        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        public string GetSign(string password) {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(MessageId)).Append(MessageId)
                .Append(nameof(LoginName)).Append(LoginName)
                .Append(nameof(ClientId)).Append(ClientId)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong())
                .Append(nameof(IUser.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }
    }
}
