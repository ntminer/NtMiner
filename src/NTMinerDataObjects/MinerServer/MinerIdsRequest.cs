using System.Collections.Generic;
using System.Text;

namespace NTMiner.MinerServer {
    public class MinerIdsRequest : RequestBase, IGetSignData {
        public MinerIdsRequest() {
            this.ObjectIds = new List<string>();
        }
        public List<string> ObjectIds { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            sb.Append(nameof(ObjectIds));
            foreach (var clientId in ObjectIds) {
                sb.Append(clientId);
            }
            return sb;
        }
    }
}
