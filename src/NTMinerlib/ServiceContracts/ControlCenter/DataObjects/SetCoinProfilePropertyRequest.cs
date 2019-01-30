using System;
using System.Runtime.Serialization;
using System.Text;

namespace NTMiner.ServiceContracts.ControlCenter.DataObjects {
    [DataContract]
    public class SetCoinProfilePropertyRequest : RequestBase, ISignatureRequest {
        [DataMember]
        public string LoginName { get; set; }
        [DataMember]
        public Guid WorkId { get; set; }
        [DataMember]
        public Guid CoinId { get; set; }
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
                .Append(nameof(CoinId)).Append(CoinId)
                .Append(nameof(PropertyName)).Append(PropertyName)
                .Append(nameof(Value)).Append(Value)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong())
                .Append(nameof(IUser.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }
    }
}
