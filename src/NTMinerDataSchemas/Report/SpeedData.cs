using NTMiner.Core;
using NTMiner.Core.MinerClient;
using System;

namespace NTMiner.Report {
    public class SpeedData : ISpeedData {
        private readonly SpeedDto _speedDto;

        public SpeedData() {
            _speedDto = new SpeedDto();
        }

        public SpeedData(SpeedDto speedDto) {
            _speedDto = speedDto;
        }

        public DateTime SpeedOn { get; set; }

        public Guid MineContextId {
            get { return _speedDto.MineContextId; }
            set {
                _speedDto.MineContextId = value;
            }
        }

        public DateTime LocalServerMessageTimestamp {
            get { return _speedDto.LocalServerMessageTimestamp; }
            set {
                _speedDto.LocalServerMessageTimestamp = value;
            }
        }

        public int AutoRestartKernelTimes {
            get { return _speedDto.AutoRestartKernelTimes; }
            set {
                _speedDto.AutoRestartKernelTimes = value;
            }
        }

        public int AutoStartDelaySeconds {
            get { return _speedDto.AutoStartDelaySeconds; }
            set {
                _speedDto.AutoStartDelaySeconds = value;
            }
        }

        public DateTime BootOn {
            get { return _speedDto.BootOn; }
            set {
                _speedDto.BootOn = value;
            }
        }

        public Guid ClientId {
            get { return _speedDto.ClientId; }
            set {
                _speedDto.ClientId = value;
            }
        }

        public string MACAddress {
            get { return _speedDto.MACAddress; }
            set {
                _speedDto.MACAddress = value;
            }
        }

        public int CpuGETemperatureSeconds {
            get { return _speedDto.CpuGETemperatureSeconds; }
            set {
                _speedDto.CpuGETemperatureSeconds = value;
            }
        }

        public int CpuLETemperatureSeconds {
            get { return _speedDto.CpuLETemperatureSeconds; }
            set {
                _speedDto.CpuLETemperatureSeconds = value;
            }
        }

        public int CpuPerformance {
            get { return _speedDto.CpuPerformance; }
            set {
                _speedDto.CpuPerformance = value;
            }
        }

        public int CpuStartTemperature {
            get { return _speedDto.CpuStartTemperature; }
            set {
                _speedDto.CpuStartTemperature = value;
            }
        }

        public int CpuStopTemperature {
            get { return _speedDto.CpuStopTemperature; }
            set {
                _speedDto.CpuStopTemperature = value;
            }
        }

        public int CpuTemperature {
            get { return _speedDto.CpuTemperature; }
            set {
                _speedDto.CpuTemperature = value;
            }
        }

        public string DiskSpace {
            get { return _speedDto.DiskSpace; }
            set {
                _speedDto.DiskSpace = value;
            }
        }

        public string DualCoinCode {
            get { return _speedDto.DualCoinCode; }
            set {
                _speedDto.DualCoinCode = value;
            }
        }

        public string DualCoinPool {
            get { return _speedDto.DualCoinPool; }
            set {
                _speedDto.DualCoinPool = value;
            }
        }

        public string DualCoinPoolDelay {
            get { return _speedDto.DualCoinPoolDelay; }
            set {
                _speedDto.DualCoinPoolDelay = value;
            }
        }

        public int DualCoinRejectShare {
            get { return _speedDto.DualCoinRejectShare; }
            set {
                _speedDto.DualCoinRejectShare = value;
            }
        }

        public double DualCoinSpeed {
            get { return _speedDto.DualCoinSpeed; }
            set {
                _speedDto.DualCoinSpeed = value;
            }
        }

        public int DualCoinTotalShare {
            get { return _speedDto.DualCoinTotalShare; }
            set {
                _speedDto.DualCoinTotalShare = value;
            }
        }

        public string DualCoinWallet {
            get { return _speedDto.DualCoinWallet; }
            set {
                _speedDto.DualCoinWallet = value;
            }
        }

        public string GpuDriver {
            get { return _speedDto.GpuDriver; }
            set {
                _speedDto.GpuDriver = value;
            }
        }

        public string GpuInfo {
            get { return _speedDto.GpuInfo; }
            set {
                _speedDto.GpuInfo = value;
            }
        }

        public DateTime MainCoinSpeedOn {
            get { return _speedDto.MainCoinSpeedOn; }
            set {
                _speedDto.MainCoinSpeedOn = value;
            }
        }

        public DateTime DualCoinSpeedOn {
            get { return _speedDto.DualCoinSpeedOn; }
            set {
                _speedDto.DualCoinSpeedOn = value;
            }
        }

        public GpuSpeedData[] GpuTable {
            get { return _speedDto.GpuTable; }
            set {
                _speedDto.GpuTable = value;
            }
        }

        public GpuType GpuType {
            get { return _speedDto.GpuType; }
            set {
                _speedDto.GpuType = value;
            }
        }

        public bool IsAutoBoot {
            get { return _speedDto.IsAutoBoot; }
            set {
                _speedDto.IsAutoBoot = value;
            }
        }

        public bool IsAutoRestartKernel {
            get { return _speedDto.IsAutoRestartKernel; }
            set {
                _speedDto.IsAutoRestartKernel = value;
            }
        }

        public bool IsAutoStart {
            get { return _speedDto.IsAutoStart; }
            set {
                _speedDto.IsAutoStart = value;
            }
        }

        public bool IsAutoStartByCpu {
            get { return _speedDto.IsAutoStartByCpu; }
            set {
                _speedDto.IsAutoStartByCpu = value;
            }
        }

        public bool IsAutoStopByCpu {
            get { return _speedDto.IsAutoStopByCpu; }
            set {
                _speedDto.IsAutoStopByCpu = value;
            }
        }

        public bool IsDualCoinEnabled {
            get { return _speedDto.IsDualCoinEnabled; }
            set {
                _speedDto.IsDualCoinEnabled = value;
            }
        }

        public bool IsFoundOneGpuShare {
            get { return _speedDto.IsFoundOneGpuShare; }
            set {
                _speedDto.IsFoundOneGpuShare = value;
            }
        }

        public bool IsGotOneIncorrectGpuShare {
            get { return _speedDto.IsGotOneIncorrectGpuShare; }
            set {
                _speedDto.IsGotOneIncorrectGpuShare = value;
            }
        }

        public bool IsMining {
            get { return _speedDto.IsMining; }
            set {
                _speedDto.IsMining = value;
            }
        }

        public bool IsNoShareRestartComputer {
            get { return _speedDto.IsNoShareRestartComputer; }
            set {
                _speedDto.IsNoShareRestartComputer = value;
            }
        }

        public bool IsNoShareRestartKernel {
            get { return _speedDto.IsNoShareRestartKernel; }
            set {
                _speedDto.IsNoShareRestartKernel = value;
            }
        }

        public bool IsPeriodicRestartComputer {
            get { return _speedDto.IsPeriodicRestartComputer; }
            set {
                _speedDto.IsPeriodicRestartComputer = value;
            }
        }

        public bool IsPeriodicRestartKernel {
            get { return _speedDto.IsPeriodicRestartKernel; }
            set {
                _speedDto.IsPeriodicRestartKernel = value;
            }
        }

        public bool IsRejectOneGpuShare {
            get { return _speedDto.IsRejectOneGpuShare; }
            set {
                _speedDto.IsRejectOneGpuShare = value;
            }
        }

        public string Kernel {
            get { return _speedDto.Kernel; }
            set {
                _speedDto.Kernel = value;
            }
        }

        public string KernelCommandLine {
            get { return _speedDto.KernelCommandLine; }
            set {
                _speedDto.KernelCommandLine = value;
            }
        }

        public int KernelSelfRestartCount {
            get { return _speedDto.KernelSelfRestartCount; }
            set {
                _speedDto.KernelSelfRestartCount = value;
            }
        }

        public string MainCoinCode {
            get { return _speedDto.MainCoinCode; }
            set {
                _speedDto.MainCoinCode = value;
            }
        }

        public string MainCoinPool {
            get { return _speedDto.MainCoinPool; }
            set {
                _speedDto.MainCoinPool = value;
            }
        }

        public string MainCoinPoolDelay {
            get { return _speedDto.MainCoinPoolDelay; }
            set {
                _speedDto.MainCoinPoolDelay = value;
            }
        }

        public int MainCoinRejectShare {
            get { return _speedDto.MainCoinRejectShare; }
            set {
                _speedDto.MainCoinRejectShare = value;
            }
        }

        public double MainCoinSpeed {
            get { return _speedDto.MainCoinSpeed; }
            set {
                _speedDto.MainCoinSpeed = value;
            }
        }

        public int MainCoinTotalShare {
            get { return _speedDto.MainCoinTotalShare; }
            set {
                _speedDto.MainCoinTotalShare = value;
            }
        }

        public string MainCoinWallet {
            get { return _speedDto.MainCoinWallet; }
            set {
                _speedDto.MainCoinWallet = value;
            }
        }

        public string MinerName {
            get { return _speedDto.MinerName; }
            set {
                _speedDto.MinerName = value;
            }
        }

        public DateTime? MineStartedOn {
            get { return _speedDto.MineStartedOn; }
            set {
                _speedDto.MineStartedOn = value;
            }
        }

        public Guid MineWorkId {
            get { return _speedDto.MineWorkId; }
            set {
                _speedDto.MineWorkId = value;
            }
        }

        public string MineWorkName {
            get { return _speedDto.MineWorkName; }
            set {
                _speedDto.MineWorkName = value;
            }
        }

        public int NoShareRestartComputerMinutes {
            get { return _speedDto.NoShareRestartComputerMinutes; }
            set {
                _speedDto.NoShareRestartComputerMinutes = value;
            }
        }

        public int NoShareRestartKernelMinutes {
            get { return _speedDto.NoShareRestartKernelMinutes; }
            set {
                _speedDto.NoShareRestartKernelMinutes = value;
            }
        }

        public string OSName {
            get { return _speedDto.OSName; }
            set {
                _speedDto.OSName = value;
            }
        }

        public int OSVirtualMemoryMb {
            get { return _speedDto.OSVirtualMemoryMb; }
            set {
                _speedDto.OSVirtualMemoryMb = value;
            }
        }

        public int TotalPhysicalMemoryMb {
            get { return _speedDto.TotalPhysicalMemoryMb; }
            set {
                _speedDto.TotalPhysicalMemoryMb = value;
            }
        }

        public int PeriodicRestartComputerHours {
            get { return _speedDto.PeriodicRestartComputerHours; }
            set {
                _speedDto.PeriodicRestartComputerHours = value;
            }
        }

        public int PeriodicRestartComputerMinutes {
            get { return _speedDto.PeriodicRestartComputerMinutes; }
            set {
                _speedDto.PeriodicRestartComputerMinutes = value;
            }
        }

        public int PeriodicRestartKernelHours {
            get { return _speedDto.PeriodicRestartKernelHours; }
            set {
                _speedDto.PeriodicRestartKernelHours = value;
            }
        }

        public int PeriodicRestartKernelMinutes {
            get { return _speedDto.PeriodicRestartKernelMinutes; }
            set {
                _speedDto.PeriodicRestartKernelMinutes = value;
            }
        }

        public bool IsRaiseHighCpuEvent {
            get { return _speedDto.IsRaiseHighCpuEvent; }
            set {
                _speedDto.IsRaiseHighCpuEvent = value;
            }
        }

        public int HighCpuPercent {
            get { return _speedDto.HighCpuPercent; }
            set {
                _speedDto.HighCpuPercent = value;
            }
        }

        public int HighCpuSeconds {
            get { return _speedDto.HighCpuSeconds; }
            set {
                _speedDto.HighCpuSeconds = value;
            }
        }

        public string Version {
            get { return _speedDto.Version; }
            set {
                _speedDto.Version = value;
            }
        }

        public bool IsOuterUserEnabled {
            get { return _speedDto.IsOuterUserEnabled; }
            set {
                _speedDto.IsOuterUserEnabled = value;
            }
        }

        public bool IsAutoDisableWindowsFirewall {
            get { return _speedDto.IsAutoDisableWindowsFirewall; }
            set {
                _speedDto.IsAutoDisableWindowsFirewall = value;
            }
        }

        public bool IsDisableUAC {
            get { return _speedDto.IsDisableUAC; }
            set {
                _speedDto.IsDisableUAC = value;
            }
        }

        public bool IsDisableWAU {
            get { return _speedDto.IsDisableWAU; }
            set {
                _speedDto.IsDisableWAU = value;
            }
        }

        public bool IsDisableAntiSpyware {
            get { return _speedDto.IsDisableAntiSpyware; }
            set {
                _speedDto.IsDisableAntiSpyware = value;
            }
        }

        public string ReportOuterUserId {
            get { return _speedDto.ReportOuterUserId; }
            set {
                _speedDto.ReportOuterUserId = value;
            }
        }

        public string LocalIp {
            get { return _speedDto.LocalIp; }
            set {
                _speedDto.LocalIp = value;
            }
        }

        public string MinerIp {
            get { return _speedDto.MinerIp; }
            set {
                _speedDto.MinerIp = value;
            }
        }
    }
}
