namespace NTMiner.Core.Gpus.Impl {
    public class Gpu : IGpu {
        public static readonly Gpu GpuAll = new Gpu {
            Index = NTMinerRoot.GpuAllId,
            BusId = "",
            Name = "全部显卡"
            // 因为其余字段全部是数值类型，留空默认值即可
        };

        public static Gpu Create(int index, string busId, string name) {
            return new Gpu {
                Index = index,
                BusId = busId,
                Name = name
                // 因为其余字段全部是数值类型，留空默认值即可
            };
        }

        private Gpu() {
        }

        public int Index { get; set; }

        public string BusId { get; set; }

        public string Name { get; set; }
        public ulong TotalMemory { get; set; }

        public int Temperature { get; set; }

        public uint FanSpeed { get; set; }

        public GpuStatus State { get; set; }

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
        public double PowerDefault { get; set; }
        public int PowerCapacity { get; set; }
        public int TempLimitMin { get; set; }
        public int TempLimitDefault { get; set; }
        public int TempLimitMax { get; set; }
        public int TempLimit { get; set; }
    }
}
