namespace NTMiner.Hashrate {
    public class GpuSpeedData {
        public int Index { get; set; }

        public long MainCoinSpeed { get; set; }

        public long DualCoinSpeed { get; set; }

        public uint Temperature { get; set; }

        public uint FanSpeed { get; set; }

        public uint PowerUsage { get; set; }
    }
}
