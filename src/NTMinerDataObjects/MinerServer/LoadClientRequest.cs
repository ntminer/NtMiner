using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class LoadClientRequest : RequestBase, IGetSignData {
        public LoadClientRequest() { }
        public Guid ClientId { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(ClientId)).Append(ClientId)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            return sb;
        }
    }
}
