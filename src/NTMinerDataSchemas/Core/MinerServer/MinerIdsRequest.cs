using System.Collections.Generic;
using System.Text;

namespace NTMiner.Core.MinerServer {
    public class MinerIdsRequest : RequestBase, IGetSignData {
        public MinerIdsRequest() {
            this.ObjectIds = new List<string>();
        }

        [ManualSign]
        public List<string> ObjectIds { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = this.BuildSign();
            sb.Append(nameof(ObjectIds));
            foreach (var clientId in ObjectIds) {
                sb.Append(clientId);
            }
            return sb;
        }
    }
}
