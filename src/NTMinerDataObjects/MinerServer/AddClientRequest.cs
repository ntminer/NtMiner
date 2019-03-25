using System.Collections.Generic;
using System.Text;

namespace NTMiner.MinerServer {
    public class AddClientRequest : RequestBase, ISignatureRequest {
        public AddClientRequest() {
            this.ClientIps = new List<string>();
        }
        public string LoginName { get; set; }
        public List<string> ClientIps { get; set; }
        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        public string GetSign(string password) {
            StringBuilder sb = GetSignData().Append(nameof(UserData.Password)).Append(password);
            sb.Append(nameof(ClientIps));
            foreach (var clientIp in ClientIps) {
                sb.Append(clientIp);
            }
            return HashUtil.Sha1(sb.ToString());
        }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(LoginName)).Append(LoginName)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            return sb;
        }
    }
}
