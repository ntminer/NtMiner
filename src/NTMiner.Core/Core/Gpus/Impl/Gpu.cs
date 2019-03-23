using NTMiner.MinerClient;

namespace NTMiner.Core.Gpus.Impl {
    public class Gpu : IGpu {
        public Gpu() {
        }

        public IOverClock OverClock { get; set; }

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
    }
}
