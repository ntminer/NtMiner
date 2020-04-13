using System;
using System.Text;

namespace NTMiner.Core.MinerServer {
    public class LoadClientRequest : IRequest, ISignableData {
        public LoadClientRequest() { }
        public Guid ClientId { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
