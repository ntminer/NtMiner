using System;

namespace NTMiner {
    public class ReportStateRequest {
        public Guid ClientId { get; set; }
        public bool IsMining { get; set; }
    }
}
