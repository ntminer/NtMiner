using System.Text;

namespace NTMiner.Core.MinerServer {
    public class MinerJsonPutUrlRequest : IRequest, ISignableData {
        public MinerJsonPutUrlRequest() { }

        public string FileName { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
