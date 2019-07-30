using System.Collections.Generic;
using System.Text;

namespace NTMiner.MinerServer {
    public class AddClientRequest : RequestBase, IGetSignData {
        public AddClientRequest() {
            this.ClientIps = new List<string>();
        }
        [ManualSign]
        public List<string> ClientIps { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = this.BuildSign();
            sb.Append(nameof(ClientIps));
            foreach (var clientIp in ClientIps) {
                sb.Append(clientIp);
            }
            return sb;
        }
    }
}
