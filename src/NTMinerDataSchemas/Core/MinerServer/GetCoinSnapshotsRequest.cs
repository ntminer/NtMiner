using System.Text;

namespace NTMiner.Core.MinerServer {
    public class GetCoinSnapshotsRequest : IRequest, ISignableData {
        public GetCoinSnapshotsRequest() {
        }

        public int Limit { get; set; }

        public StringBuilder GetSignData() {
            return this.GetActionIdSign("9EB5411A-E91C-4142-A5B2-049B04660B76");
        }
    }
}
