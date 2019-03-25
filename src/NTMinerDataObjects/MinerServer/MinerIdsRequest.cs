using System.Collections.Generic;
using System.Text;

namespace NTMiner.MinerServer {
    public class MinerIdsRequest : RequestBase, ISignatureRequest {
        public MinerIdsRequest() {
            this.ObjectIds = new List<string>();
        }
        public string LoginName { get; set; }
        public List<string> ObjectIds { get; set; }
        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        public string GetSign(string password) {
            StringBuilder sb = GetSignData().Append(nameof(UserData.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(LoginName)).Append(LoginName)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            sb.Append(nameof(ObjectIds));
            foreach (var clientId in ObjectIds) {
                sb.Append(clientId);
            }
            return sb;
        }
    }
}
