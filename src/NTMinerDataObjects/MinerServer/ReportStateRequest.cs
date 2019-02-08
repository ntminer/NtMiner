using System;

namespace NTMiner.MinerServer {
    public class ReportStateRequest {
        public Guid ClientId { get; set; }
        public bool IsMining { get; set; }
    }
}
