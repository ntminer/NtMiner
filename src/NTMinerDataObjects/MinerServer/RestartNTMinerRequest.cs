using System;

namespace NTMiner.MinerServer {
    public class RestartNTMinerRequest : RequestBase {
        public string ClientIp { get; set; }
        public Guid WorkId { get; set; }
    }
}
