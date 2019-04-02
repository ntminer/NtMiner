namespace NTMiner.Core.Gpus.Impl {
    public class Gpu : IGpu {
        public static readonly Gpu GpuAll = new Gpu {
            Index = NTMinerRoot.GpuAllId,
            Name = "全部显卡",
            Temperature = 0,
            FanSpeed = 0,
            PowerUsage = 0,
            CoreClockDelta = 0,
            MemoryClockDelta = 0,
            Cool = 0,
            CoolMax = 0,
            CoolMin = 0,
            CoreClockDeltaMax = 0,
            CoreClockDeltaMin = 0,
            MemoryClockDeltaMax = 0,
            MemoryClockDeltaMin = 0,
            Power = 0,
            PowerMax = 0,
            PowerMin = 0,
            TempLimit = 0,
            TempLimitDefault = 0,
            TempLimitMax = 0,
            TempLimitMin = 0
        };

        public static Gpu Create(int index, string name) {
            return new Gpu {
                Index = index,
                Name = name,
                Temperature = 0,
                PowerUsage = 0,
                FanSpeed = 0
            };
        }

        private Gpu() {
        }

        public int Index { get; set; }

        public string Name { get; set; }

        public uint Temperature { get; set; }

        public uint FanSpeed { get; set; }

        public uint PowerUsage { get; set; }
        public int CoreClockDelta { get; set; }
        public int MemoryClockDelta { get; set; }
        public int CoreClockDeltaMin { get; set; }
        public int CoreClockDeltaMax { get; set; }
        public int MemoryClockDeltaMin { get; set; }
        public int MemoryClockDeltaMax { get; set; }
        public int Cool { get; set; }
        public int CoolMin { get; set; }
        public int CoolMax { get; set; }
        public double PowerMin { get; set; }
        public double PowerMax { get; set; }
        public double Power { get; set; }
        public int TempLimitMin { get; set; }
        public int TempLimitDefault { get; set; }
        public int TempLimitMax { get; set; }
        public int TempLimit { get; set; }
    }
}
