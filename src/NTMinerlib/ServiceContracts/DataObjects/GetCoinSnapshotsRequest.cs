using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class GetCoinSnapshotsRequest : RequestBase {
        public GetCoinSnapshotsRequest() {
            this.CoinCodes = new List<string>();
        }

        [DataMember]
        public string LoginName { get; set; }
        [DataMember]
        public int Limit { get; set; }
        [DataMember]
        public List<string> CoinCodes { get; set; }
        [DataMember]
        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        public string GetSign(string password) {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(MessageId)).Append(MessageId)
                .Append(nameof(LoginName)).Append(LoginName)
                .Append(nameof(Limit)).Append(Limit)
                .Append(nameof(CoinCodes)).Append(string.Join(",", CoinCodes))
                .Append(nameof(Timestamp)).Append(Timestamp)
                .Append(nameof(IUser.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }
    }
}
