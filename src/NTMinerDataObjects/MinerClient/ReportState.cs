using System;

namespace NTMiner.MinerClient {
    public class ReportState {
        public ReportState() { }
        public Guid ClientId { get; set; }
        public bool IsMining { get; set; }
    }
}
