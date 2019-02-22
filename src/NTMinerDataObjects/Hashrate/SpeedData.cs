using System;

namespace NTMiner.Hashrate {
    public class SpeedData {
        public SpeedData() {
            GpuSpeeds = new GpuSpeedData[0];
        }

        public Guid ClientId { get; set; }

        public Guid WorkId { get; set; }

        public string Version { get; set; }

        public DateTime BootOn { get; set; }

        public DateTime? MineStartedOn { get; set; }

        public bool IsMining { get; set; }

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

        public int MainCoinTotalShare { get; set; }

        public int MainCoinRejectShare { get; set; }

        public int MainCoinShareDelta { get; set; }

        public int MainCoinRejectShareDelta { get; set; }

        public bool IsDualCoinEnabled { get; set; }

        public int DualCoinTotalShare { get; set; }

        public int DualCoinRejectShare { get; set; }

        public int DualCoinShareDelta { get; set; }

        public int DualCoinRejectShareDelta { get; set; }

        public double MainCoinSpeed { get; set; }

        public double DualCoinSpeed { get; set; }

        public string GpuDriver { get; set; }

        public GpuType GpuType { get; set; }

        public string OSName { get; set; }

        /// <summary>
        /// Gb
        /// </summary>
        public double OSVirtualMemory { get; set; }

        public GpuSpeedData[] GpuSpeeds { get; set; }
    }
}
