using System;

namespace NTMiner.MinerClient {
    public class SpeedData : ISpeedData {
        public SpeedData() {
            GpuTable = new GpuSpeedData[0];
        }

        public DateTime LocalServerMessageTimestamp { get; set; }
        public int KernelSelfRestartCount { get; set; }
        public bool IsAutoBoot { get; set; }
        public bool IsAutoStart { get; set; }
        public int AutoStartDelaySeconds { get; set; }
        public bool IsAutoRestartKernel { get; set; }
        public int AutoRestartKernelTimes { get; set; }
        public bool IsNoShareRestartKernel { get; set; }
        public int NoShareRestartKernelMinutes { get; set; }
        public bool IsNoShareRestartComputer { get; set; }
        public int NoShareRestartComputerMinutes { get; set; }
        public bool IsPeriodicRestartKernel { get; set; }
        public int PeriodicRestartKernelHours { get; set; }
        public bool IsPeriodicRestartComputer { get; set; }
        public int PeriodicRestartComputerHours { get; set; }
        public int PeriodicRestartKernelMinutes { get; set; }
        public int PeriodicRestartComputerMinutes { get; set; }
        public bool IsAutoStopByCpu { get; set; }
        public int CpuGETemperatureSeconds { get; set; }
        public int CpuStopTemperature { get; set; }
        public bool IsAutoStartByCpu { get; set; }
        public int CpuLETemperatureSeconds { get; set; }
        public int CpuStartTemperature { get; set; }
        public string GpuDriver { get; set; }
        public GpuType GpuType { get; set; }
        // ReSharper disable once InconsistentNaming
        public string OSName { get; set; }
        // ReSharper disable once InconsistentNaming
        public int OSVirtualMemoryMb { get; set; }
        public string DiskSpace { get; set; }

        public Guid ClientId { get; set; }

        public string MACAddress { get; set; }

        public string Version { get; set; }

        public DateTime BootOn { get; set; }

        public DateTime? MineStartedOn { get; set; }

        public bool IsMining { get; set; }

        public string MinerIp { get; set; }

        public string GpuInfo { get; set; }

        public Guid MineWorkId { get; set; }

        public string MineWorkName { get; set; }

        /// <summary>
        /// 注意：该属性对应服务端的ClientName，而服务端的MinerName是群控矿工名。
        /// </summary>
        public string MinerName { get; set; }

        private string _mainCoinCode;
        public string MainCoinCode {
            get => _mainCoinCode ?? string.Empty;
            set => _mainCoinCode = value;
        }

        public string MainCoinPool { get; set; }

        public string MainCoinWallet { get; set; }

        public string MainCoinPoolDelay { get; set; }

        public string Kernel { get; set; }

        private string _dualCoinCode;
        public string DualCoinCode {
            get => _dualCoinCode ?? string.Empty;
            set => _dualCoinCode = value;
        }

        public string DualCoinPool { get; set; }

        public string DualCoinWallet { get; set; }

        public string DualCoinPoolDelay { get; set; }

        public int MainCoinTotalShare { get; set; }

        public int MainCoinRejectShare { get; set; }

        public bool IsDualCoinEnabled { get; set; }

        public int DualCoinTotalShare { get; set; }

        public int DualCoinRejectShare { get; set; }

        public double MainCoinSpeed { get; set; }

        public double DualCoinSpeed { get; set; }

        public string KernelCommandLine { get; set; }

        public bool IsRejectOneGpuShare { get; set; }

        public bool IsFoundOneGpuShare { get; set; }

        public bool IsGotOneIncorrectGpuShare { get; set; }

        public int CpuPerformance { get; set; }

        public int CpuTemperature { get; set; }

        public bool IsRaiseHighCpuEvent { get; set; }

        public int HighCpuPercent { get; set; }

        public int HighCpuSeconds { get; set; }

        public GpuSpeedData[] GpuTable { get; set; }
    }
}
