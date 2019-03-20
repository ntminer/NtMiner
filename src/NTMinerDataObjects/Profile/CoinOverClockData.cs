using System;

namespace NTMiner.Profile {
    public class CoinOverClockData {
        public Guid CoinId { get; set; }
        public bool IsOverClockEnabled { get; set; }
        public bool IsOverClockGpuAll { get; set; }
    }
}
