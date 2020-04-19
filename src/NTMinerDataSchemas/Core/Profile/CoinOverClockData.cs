using System;

namespace NTMiner.Core.Profile {
    public class CoinOverClockData : IEntity<Guid> {
        public CoinOverClockData() { }

        public Guid GetId() {
            return this.CoinId;
        }

        public Guid CoinId { get; set; }
        public bool IsOverClockEnabled { get; set; }
        public bool IsOverClockGpuAll { get; set; }
    }
}
