using System.Text;

namespace NTMiner.Core.MinerServer {
    public class GetCoinSnapshotsRequest : IRequest, ISignableData {
        public GetCoinSnapshotsRequest() {
        }

        public int Limit { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
