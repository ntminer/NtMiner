using System;

namespace NTMiner.Core.MinerServer {
    public class ReportState {
        public ReportState() { }
        public Guid ClientId { get; set; }
        public bool IsMining { get; set; }
    }
}
