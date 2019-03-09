using System;

namespace NTMiner {
    public class GpuProfileData : IGpuProfile {
        public GpuProfileData() {
        }

        public GpuProfileData(Guid id, Guid coinId, int index) {
            this.Id = id;
            this.CoinId = coinId;
            this.Index = index;
            this.CoreClockDelta = 0;
            this.MemoryClockDelta = 0;
            this.PowerCapacity = 0;
            this.Cool = 0;
            this.IsEnabled = true;
        }

        public GpuProfileData(IGpuProfile data) {
            this.Id = data.GetId();
            this.CoinId = data.CoinId;
            this.Index = data.Index;
            this.CoreClockDelta = data.CoreClockDelta;
            this.MemoryClockDelta = data.MemoryClockDelta;
            this.PowerCapacity = data.PowerCapacity;
            this.Cool = data.Cool;
            this.IsEnabled = data.IsEnabled;
        }

        public void Update(IGpuProfile data) {
            this.CoinId = data.CoinId;
            this.Index = data.Index;
            this.CoreClockDelta = data.CoreClockDelta;
            this.MemoryClockDelta = data.MemoryClockDelta;
            this.PowerCapacity = data.PowerCapacity;
            this.Cool = data.Cool;
            this.IsEnabled = data.IsEnabled;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public Guid CoinId { get; set; }

        public int Index { get; set; }

        public int CoreClockDelta { get; set; }

        public int MemoryClockDelta { get; set; }

        public int PowerCapacity { get; set; }

        public int Cool { get; set; }

        public bool IsEnabled { get; set; }

        public override string ToString() {
            return $"{CoinId}{Index}{CoreClockDelta}{MemoryClockDelta}{PowerCapacity}{Cool}{IsEnabled}";
        }
    }
}
