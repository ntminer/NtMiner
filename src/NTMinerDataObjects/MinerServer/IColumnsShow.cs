using System;

namespace NTMiner.MinerServer {
    public interface IColumnsShow : IEntity<Guid> {
        string ColumnsShowName { get; }
        bool Work { get; }
        bool MinerName { get; }
        bool ClientName { get; }
        bool MinerIp  { get; }
        bool MinerGroup  { get; }
        bool MainCoinCode  { get; }
        bool MainCoinSpeedText  { get; }
        bool GpuTableVm { get; }
        bool MainCoinWallet  { get; }
        bool MainCoinPool  { get; }
        bool Kernel  { get; }
        bool DualCoinCode  { get; }
        bool DualCoinSpeedText  { get; }
        bool DualCoinWallet  { get; }
        bool DualCoinPool  { get; }
        bool LastActivedOnText  { get; }
        bool Version  { get; }
        bool WindowsLoginNameAndPassword  { get; }
        bool GpuInfo  { get; }
        bool MainCoinRejectPercentText { get; }
        bool DualCoinRejectPercentText { get; }
        bool BootTimeSpanText { get; }
        bool MineTimeSpanText { get; }
        bool IncomeMainCoinPerDayText { get; }
        bool IncomeDualCoinPerDayText { get; }
        bool IsAutoBoot { get; }
        bool IsAutoStart { get; }
        bool OSName { get; }
        bool OSVirtualMemoryGbText { get; }
        bool DiskSpace { get; }
        bool GpuType { get; }
        bool GpuDriver { get; }
        bool TotalPowerText { get; }
        bool MaxTempText { get; }
        bool KernelCommandLine { get; }
        bool IsAutoRestartKernel { get; }
        bool IsNoShareRestartKernel { get; }
        bool IsNoShareRestartComputer { get; }
        bool IsPeriodicRestartKernel { get; }
        bool IsPeriodicRestartComputer { get; }
    }
}
