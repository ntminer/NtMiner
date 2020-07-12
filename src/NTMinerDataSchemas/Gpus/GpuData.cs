namespace NTMiner.Gpus {
    public class GpuData : IGpuStaticData {
        public GpuData() {
            this.PowerDefault = 100;
        }

        public GpuType GpuType { get; set; }
        public int Index { get; set; }
        public string BusId { get; set; }
        public string Name { get; set; }
        public ulong TotalMemory { get; set; }

        public int CoreClockDeltaMin { get; set; }
        public int CoreClockDeltaMax { get; set; }
        public int MemoryClockDeltaMin { get; set; }
        public int MemoryClockDeltaMax { get; set; }
        public int VoltMin { get; set; }
        public int VoltMax { get; set; }
        public int VoltDefault { get; set; }
        public int CoolMin { get; set; }
        public int CoolMax { get; set; }
        public double PowerMin { get; set; }
        public double PowerMax { get; set; }
        public double PowerDefault { get; set; }
        public int TempLimitMin { get; set; }
        public int TempLimitDefault { get; set; }
        public int TempLimitMax { get; set; }

        bool IGpuName.IsValid() {
            return GpuName.IsValid(this.GpuType, this.Name, this.TotalMemory);
        }
    }
}
