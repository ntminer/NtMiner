using System;
using System.Text;

namespace NTMiner.Core.MinerServer {
    public class LoadClientRequest : RequestBase, IGetSignData {
        public LoadClientRequest() { }
        public Guid ClientId { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
