namespace NTMiner.Core.Gpus.Impl {
    public class Gpu : IGpu {
        public static readonly Gpu GpuAll = new Gpu {
            Index = NTMinerRoot.GpuAllId,
            Name = "全部显卡"
            // 因为其余字段全部是数值类型，留空默认值即可
        };

        public static Gpu Create(int index, string name) {
            return new Gpu {
                Index = index,
                Name = name
                // 因为其余字段全部是数值类型，留空默认值即可
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
        public int PowerCapacity { get; set; }
        public int TempLimitMin { get; set; }
        public int TempLimitDefault { get; set; }
        public int TempLimitMax { get; set; }
        public int TempLimit { get; set; }
        public bool IsGuardTemp { get; set; }
        public int TempGuard { get; set; }
    }
}
