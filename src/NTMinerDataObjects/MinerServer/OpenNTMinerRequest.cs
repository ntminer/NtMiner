using System;

namespace NTMiner.MinerServer {
    public class OpenNTMinerRequest : RequestBase {
        public string ClientIp { get; set; }
        public Guid WorkId { get; set; }
    }
}
