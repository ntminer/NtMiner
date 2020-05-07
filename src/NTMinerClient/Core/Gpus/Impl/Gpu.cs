namespace NTMiner.Core.Gpus.Impl {
    public class Gpu : IGpu {
        public static readonly Gpu GpuAll = new Gpu {
            Index = NTMinerContext.GpuAllId,
            BusId = "",
            Name = "全部显卡"
            // 因为其余字段全部是数值类型，留空默认值即可
        };

        public static Gpu Create(GpuType gpuType, int index, string busId, string name) {
            return new Gpu {
                GpuType = gpuType,
                Index = index,
                BusId = busId,
                Name = name
                // 因为其余字段全部是数值类型，留空默认值即可
            };
        }

        private Gpu() {
        }

        public GpuType GpuType { get; set; }
        public int Index { get; set; }
        public string BusId { get; set; }
        public string Name { get; set; }
        public ulong TotalMemory { get; set; }

        public int Temperature { get; set; }
        public uint FanSpeed { get; set; }
        public uint PowerUsage { get; set; }
        public int CoreClockDelta { get; set; }
        public int MemoryClockDelta { get; set; }
        public int CoreClockDeltaMin { get; set; }
        public int CoreClockDeltaMax { get; set; }
        public int MemoryClockDeltaMin { get; set; }
        public int MemoryClockDeltaMax { get; set; }
        public int VoltMin { get; set; }
        public int VoltMax { get; set; }
        public int VoltDefault { get; set; }
        public int Cool { get; set; }
        public int CoolMin { get; set; }
        public int CoolMax { get; set; }
        public double PowerMin { get; set; }
        public double PowerMax { get; set; }
        public double PowerDefault { get; set; }
        public int PowerCapacity { get; set; }
        public int TempLimitMin { get; set; }
        public int TempLimitDefault { get; set; }
        public int TempLimitMax { get; set; }
        public int TempLimit { get; set; }
        public int CoreVoltage { get; set; }
        public int MemoryVoltage { get; set; }

        bool IGpuName.IsValid() {
            return GpuName.IsValid(this.GpuType, this.Name, this.TotalMemory);
        }
    }
}
