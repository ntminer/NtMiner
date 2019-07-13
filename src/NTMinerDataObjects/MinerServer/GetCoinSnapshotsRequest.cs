using System.Text;

namespace NTMiner.MinerServer {
    public class GetCoinSnapshotsRequest : RequestBase, ISignatureRequest {
        public GetCoinSnapshotsRequest() {
        }

        public int Limit { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Limit)).Append(Limit)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            return sb;
        }
    }
}
