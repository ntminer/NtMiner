using System;

namespace NTMiner.Core.Gpus.Impl {
    public class GpuOverClockData : IGpuOverClockData {
        public GpuOverClockData(Guid id, Guid coinId, int index) {
            this.Id = id;
            this.CoinId = coinId;
            this.Index = index;
            this.Cool = 0;
            this.CoreClockDelta = 0;
            this.MemoryClockDelta = 0;
            this.PowerCapacity = 0;
            this.Name = "Gpu" + index;
        }

        public GpuOverClockData(IGpuOverClockData data) {
            this.Id = data.CoinId;
            this.CoinId = data.CoinId;
            this.Index = data.Index;
            this.Name = data.Name;
            this.CoreClockDelta = data.CoreClockDelta;
            this.MemoryClockDelta = data.MemoryClockDelta;
            this.PowerCapacity = data.PowerCapacity;
            this.Cool = data.Cool;
        }

        public void Update(IGpuOverClockData data) {
            this.CoinId = data.CoinId;
            this.Index = data.Index;
            this.Name = data.Name;
            this.CoreClockDelta = data.CoreClockDelta;
            this.MemoryClockDelta = data.MemoryClockDelta;
            this.PowerCapacity = data.PowerCapacity;
            this.Cool = data.Cool;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public Guid CoinId { get; set; }

        public int Index { get; set; }

        public string Name { get; set; }

        public int CoreClockDelta { get; set; }

        public int MemoryClockDelta { get; set; }

        public int PowerCapacity { get; set; }

        public int Cool { get; set; }
    }
}
