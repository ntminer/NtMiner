using System;

namespace NTMiner.MinerClient {
    public class SpeedData {
        public SpeedData() {
            GpuTable = new GpuSpeedData[0];
        }

        public bool IsAutoBoot { get; set; }
        public bool IsAutoStart { get; set; }
        public bool IsAutoRestartKernel { get; set; }
        public bool IsNoShareRestartKernel { get; set; }
        public int NoShareRestartKernelMinutes { get; set; }
        public bool IsPeriodicRestartKernel { get; set; }
        public int PeriodicRestartKernelHours { get; set; }
        public bool IsPeriodicRestartComputer { get; set; }
        public int PeriodicRestartComputerHours { get; set; }
        public string GpuDriver { get; set; }
        public GpuType GpuType { get; set; }
        // ReSharper disable once InconsistentNaming
        public string OSName { get; set; }
        // ReSharper disable once InconsistentNaming
        public int OSVirtualMemoryMb { get; set; }
        public string DiskSpace { get; set; }

        public Guid ClientId { get; set; }

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

        public bool IsDualCoinEnabled { get; set; }

        public int DualCoinTotalShare { get; set; }

        public int DualCoinRejectShare { get; set; }

        public double MainCoinSpeed { get; set; }

        public double DualCoinSpeed { get; set; }

        public string KernelCommandLine { get; set; }

        public GpuSpeedData[] GpuTable { get; set; }
    }
}
