using System;

namespace NTMiner.MinerClient {
    public class CoinOverClockData {
        public Guid CoinId { get; set; }
        public bool IsOverClockEnabled { get; set; }
        public bool IsOverClockGpuAll { get; set; }
    }
}
