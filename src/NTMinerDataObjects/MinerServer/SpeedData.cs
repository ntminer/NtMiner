using System;

namespace NTMiner.MinerServer {
    public class SpeedData : RequestBase {
        public SpeedData() { }

        public Guid ClientId { get; set; }

        public Guid WorkId { get; set; }

        public string Version { get; set; }

        public string MinerIp { get; set; }

        public string GpuInfo { get; set; }

        public string MinerName { get; set; }

        public string MainCoinCode { get; set; }

        public string MainCoinPool { get; set; }

        public string MainCoinWallet { get; set; }

        public string Kernel { get; set; }

        public string DualCoinCode { get; set; }

        public string DualCoinPool { get; set; }

        public string DualCoinWallet { get; set; }

        public int MainCoinShareDelta { get; set; }

        public bool IsDualCoinEnabled { get; set; }

        public int DualCoinShareDelta { get; set; }

        public long MainCoinSpeed { get; set; }

        public long DualCoinSpeed { get; set; }

        public bool IsMining { get; set; }
    }
}
