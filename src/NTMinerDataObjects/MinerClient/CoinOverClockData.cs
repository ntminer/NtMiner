using System;

namespace NTMiner.MinerClient {
    public class CoinOverClockData : IEntity<Guid> {
        public Guid GetId() {
            return this.CoinId;
        }

        public Guid CoinId { get; set; }
        public bool IsOverClockEnabled { get; set; }
        public bool IsOverClockGpuAll { get; set; }
    }
}
