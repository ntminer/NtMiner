namespace NTMiner.Core.Gpus.Impl {
    public class GpuOverClockData : IGpuOverClockData {
        public static readonly GpuOverClockData GpuAllData = new GpuOverClockData(NTMinerRoot.GpuAllId);

        public GpuOverClockData(int index) {
            this.Index = index;
            this.Cool = 0;
            this.CoreClockDelta = 0;
            this.MemoryClockDelta = 0;
            this.PowerCapacity = 0;
            this.Name = "Gpu" + index;
        }

        public GpuOverClockData(IGpuOverClockData data) {
            this.Index = data.Index;
            this.Name = data.Name;
            this.CoreClockDelta = data.CoreClockDelta;
            this.MemoryClockDelta = data.MemoryClockDelta;
            this.PowerCapacity = data.PowerCapacity;
            this.Cool = data.Cool;
        }

        public void Update(IGpuOverClockData data) {
            this.Name = data.Name;
            this.CoreClockDelta = data.CoreClockDelta;
            this.MemoryClockDelta = data.MemoryClockDelta;
            this.PowerCapacity = data.PowerCapacity;
            this.Cool = data.Cool;
        }

        [LiteDB.BsonId]
        public int Index { get; set; }

        public string Name { get; set; }

        public int CoreClockDelta { get; set; }

        public int MemoryClockDelta { get; set; }

        public int PowerCapacity { get; set; }

        public int Cool { get; set; }
    }
}
