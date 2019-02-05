using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace NTMiner {
    [DataContract]
    public class SaveCalcConfigsRequest : RequestBase, ISignatureRequest {
        [DataMember]
        public string LoginName { get; set; }
        [DataMember]
        public List<CalcConfigData> Data { get; set; }
        [DataMember]
        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        public string GetSign(string password) {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(MessageId)).Append(MessageId)
                .Append(nameof(LoginName)).Append(LoginName)
                .Append(GetSignData(sb, this.Data))
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong())
                .Append(nameof(UserData.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }

        private static StringBuilder GetSignData(StringBuilder sb, List<CalcConfigData> list) {
            foreach (var item in list) {
                sb.Append(item.GetSignData());
            }
            return sb;
        }
    }
}
