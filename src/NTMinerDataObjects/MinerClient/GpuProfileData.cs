using System;

namespace NTMiner.MinerClient {
    public class GpuProfileData : IGpuProfile {
        public GpuProfileData() {
        }

        public GpuProfileData(Guid coinId, int index) {
            this.CoinId = coinId;
            this.Index = index;
            this.CoreClockDelta = 0;
            this.MemoryClockDelta = 0;
            this.PowerCapacity = 100;
            this.Cool = 90;
        }

        public GpuProfileData(IGpuProfile data) {
            this.CoinId = data.CoinId;
            this.Index = data.Index;
            this.CoreClockDelta = data.CoreClockDelta;
            this.MemoryClockDelta = data.MemoryClockDelta;
            this.PowerCapacity = data.PowerCapacity;
            this.Cool = data.Cool;
        }

        public void Update(IGpuProfile data) {
            this.CoinId = data.CoinId;
            this.Index = data.Index;
            this.CoreClockDelta = data.CoreClockDelta;
            this.MemoryClockDelta = data.MemoryClockDelta;
            this.PowerCapacity = data.PowerCapacity;
            this.Cool = data.Cool;
        }

        public string GetId() {
            return $"{CoinId}_{Index}";
        }

        public Guid CoinId { get; set; }

        public int Index { get; set; }

        public int CoreClockDelta { get; set; }

        public int MemoryClockDelta { get; set; }

        public int PowerCapacity { get; set; }

        public int Cool { get; set; }

        public override string ToString() {
            return $"{CoinId}{Index}{CoreClockDelta}{MemoryClockDelta}{PowerCapacity}{Cool}";
        }
    }
}
