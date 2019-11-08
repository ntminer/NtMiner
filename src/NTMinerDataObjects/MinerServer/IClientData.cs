using NTMiner.MinerClient;
using System;

namespace NTMiner.MinerServer {
    public interface IClientData : IEntity<string> {
        Guid ClientId { get; }
        bool IsAutoBoot { get; }

        bool IsAutoStart { get; }

        int AutoStartDelaySeconds { get; }

        Guid WorkId { get; }

        Guid MineWorkId { get; }

        string MineWorkName { get; }

        string Version { get; }

        bool IsMining { get; }

        DateTime BootOn { get; }

        DateTime? MineStartedOn { get; }

        /// <summary>
        /// 群控名矿机名
        /// </summary>
        string MinerName { get; }

        /// <summary>
        /// 矿机名
        /// </summary>
        string ClientName { get; }

        Guid GroupId { get; }

        string MinerIp { get; }

        string WindowsLoginName { get; }

        string WindowsPassword { get; }

        string MainCoinCode { get; }

        int MainCoinTotalShare { get; }

        int MainCoinRejectShare { get; }

        double MainCoinSpeed { get; }

        string MainCoinPool { get; }

        string MainCoinWallet { get; }

        string MainCoinPoolDelay { get; }

        string Kernel { get; }

        bool IsDualCoinEnabled { get; }

        string DualCoinCode { get; }

        int DualCoinTotalShare { get; }

        int DualCoinRejectShare { get; }

        double DualCoinSpeed { get; }

        string DualCoinPool { get; }

        string DualCoinWallet { get; }

        string DualCoinPoolDelay { get; }

        string GpuInfo { get; }

        // ReSharper disable once InconsistentNaming
        string OSName { get; }

        // ReSharper disable once InconsistentNaming
        int OSVirtualMemoryMb { get; }

        string DiskSpace { get; }

        GpuType GpuType { get; }

        string GpuDriver { get; }

        string KernelCommandLine { get; }
        bool IsAutoRestartKernel { get; }
        int AutoRestartKernelTimes { get; }
        bool IsNoShareRestartKernel { get; }
        int NoShareRestartKernelMinutes { get; }
        bool IsPeriodicRestartKernel { get; }
        int PeriodicRestartKernelHours { get; }
        int PeriodicRestartKernelMinutes { get; }
        bool IsPeriodicRestartComputer { get; }
        int NoShareRestartComputerMinutes { get; }
        int PeriodicRestartComputerHours { get; }
        int PeriodicRestartComputerMinutes { get; }
        bool IsRejectOneGpuShare { get; }
        bool IsFoundOneGpuShare { get; }
        bool IsGotOneIncorrectGpuShare { get; }
        bool IsAutoStopByCpu { get; }
        int CpuGETemperatureSeconds { get; }
        int CpuStopTemperature { get; }
        bool IsAutoStartByCpu { get; }
        int CpuLETemperatureSeconds { get; }
        int CpuStartTemperature { get; }
        int CpuPerformance { get; }
        int CpuTemperature { get; }

        GpuSpeedData[] GpuTable { get; }
    }
}
