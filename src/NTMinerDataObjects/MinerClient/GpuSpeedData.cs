namespace NTMiner.MinerClient {
    public class GpuSpeedData {
        public GpuSpeedData() {
        }

        public int Index { get; set; }

        public string Name { get; set; }

        public double MainCoinSpeed { get; set; }

        public double DualCoinSpeed { get; set; }

        public uint Temperature { get; set; }

        public uint FanSpeed { get; set; }

        public uint PowerUsage { get; set; }
    }
}
