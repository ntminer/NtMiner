using NTMiner.MinerClient;
using System;

namespace NTMiner.MinerServer {
    public interface IClientData : IEntity<string> {
        Guid ClientId { get; }
        bool IsAutoBoot { get; }

        bool IsAutoStart { get; }

        Guid WorkId { get; }

        string Version { get; }

        bool IsMining { get; }

        DateTime BootOn { get; }

        DateTime? MineStartedOn { get; }

        string MinerName { get; }

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

        string Kernel { get; }

        bool IsDualCoinEnabled { get; }

        string DualCoinCode { get; }

        int DualCoinTotalShare { get; }

        int DualCoinRejectShare { get; }

        double DualCoinSpeed { get; }

        string DualCoinPool { get; }

        string DualCoinWallet { get; }

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
        bool IsNoShareRestartKernel { get; }
        int NoShareRestartKernelMinutes { get; }
        bool IsPeriodicRestartKernel { get; }
        int PeriodicRestartKernelHours { get; }
        bool IsPeriodicRestartComputer { get; }
        int PeriodicRestartComputerHours { get; }
        GpuSpeedData[] GpuTable { get; }
    }
}
