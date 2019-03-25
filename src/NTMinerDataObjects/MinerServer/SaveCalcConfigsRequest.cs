using System.Collections.Generic;
using System.Text;

namespace NTMiner.MinerServer {
    public class SaveCalcConfigsRequest : RequestBase, ISignatureRequest {
        public SaveCalcConfigsRequest() { }
        public string LoginName { get; set; }
        public List<CalcConfigData> Data { get; set; }
        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        public string GetSign(string password) {
            StringBuilder sb = GetSignData().Append(nameof(UserData.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }

        private static StringBuilder GetSignData(StringBuilder sb, List<CalcConfigData> list) {
            foreach (var item in list) {
                sb.Append(item.GetSignData());
            }
            return sb;
        }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(LoginName)).Append(LoginName)
                .Append(GetSignData(sb, this.Data))
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            return sb;
        }
    }
}
