using System;
using System.Runtime.Serialization;
using System.Text;

namespace NTMiner {
    [DataContract]
    public class SetCoinKernelProfilePropertyRequest : RequestBase, ISignatureRequest {
        [DataMember]
        public string LoginName { get; set; }
        [DataMember]
        public Guid WorkId { get; set; }
        [DataMember]
        public Guid CoinKernelId { get; set; }
        [DataMember]
        public string PropertyName { get; set; }
        [DataMember]
        public object Value { get; set; }
        [DataMember]
        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        public string GetSign(string password) {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(MessageId)).Append(MessageId)
                .Append(nameof(LoginName)).Append(LoginName)
                .Append(nameof(WorkId)).Append(WorkId)
                .Append(nameof(CoinKernelId)).Append(CoinKernelId)
                .Append(nameof(PropertyName)).Append(PropertyName)
                .Append(nameof(Value)).Append(Value)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong())
                .Append(nameof(UserData.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }
    }
}
