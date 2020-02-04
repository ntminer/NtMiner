using LiteDB;
using NTMiner.Core.MinerClient;
using System;

namespace NTMiner.Core.MinerServer {
    public class ClientData : SpeedData, IMinerData, ISpeedData, IDbEntity<string>, ITimestampEntity<string> {
        public ClientData() : base() {
        }

        public string GetId() {
            return this.Id;
        }

        public static ClientData CreateClientData(IMinerData data) {
            return new ClientData() {
                Id = data.Id,
                ClientId = data.ClientId,
                MACAddress = string.Empty,
                LocalIp = string.Empty,
                MinerIp = data.MinerIp,
                MinerName = data.MinerName,
                ClientName = data.ClientName,
                CreatedOn = data.CreatedOn,
                GroupId = data.GroupId,
                WorkId = data.WorkId,
                WindowsLoginName = data.WindowsLoginName,
                WindowsPassword = data.WindowsPassword,
                MineWorkId = Guid.Empty,
                MineWorkName = string.Empty,
                IsAutoBoot = false,
                IsAutoStart = false,
                AutoStartDelaySeconds = 15,
                IsAutoRestartKernel = false,
                AutoRestartKernelTimes = 10,
                IsNoShareRestartKernel = false,
                NoShareRestartKernelMinutes = 0,
                IsNoShareRestartComputer = false,
                NoShareRestartComputerMinutes = 0,
                IsPeriodicRestartKernel = false,
                PeriodicRestartKernelHours = 0,
                IsPeriodicRestartComputer = false,
                PeriodicRestartComputerHours = 0,
                PeriodicRestartKernelMinutes = 10,
                PeriodicRestartComputerMinutes = 10,
                IsAutoStartByCpu = false,
                IsAutoStopByCpu = false,
                CpuGETemperatureSeconds = 60,
                CpuLETemperatureSeconds = 60,
                CpuStartTemperature = 40,
                CpuStopTemperature = 65,
                GpuDriver = String.Empty,
                GpuType = GpuType.Empty,
                OSName = String.Empty,
                OSVirtualMemoryMb = 0,
                GpuInfo = String.Empty,
                Version = String.Empty,
                IsMining = false,
                BootOn = DateTime.MinValue,
                MineStartedOn = DateTime.MinValue,
                ModifiedOn = DateTime.MinValue,
                MainCoinCode = String.Empty,
                MainCoinTotalShare = 0,
                MainCoinRejectShare = 0,
                MainCoinSpeed = 0,
                MainCoinPool = String.Empty,
                MainCoinWallet = String.Empty,
                Kernel = String.Empty,
                IsDualCoinEnabled = false,
                DualCoinPool = String.Empty,
                DualCoinWallet = String.Empty,
                DualCoinCode = String.Empty,
                DualCoinTotalShare = 0,
                DualCoinRejectShare = 0,
                DualCoinSpeed = 0,
                KernelCommandLine = String.Empty,
                MainCoinPoolDelay = string.Empty,
                DualCoinPoolDelay = string.Empty,
                DiskSpace = string.Empty,
                IsFoundOneGpuShare = false,
                IsRejectOneGpuShare = false,
                IsGotOneIncorrectGpuShare = false,
                CpuPerformance = 0,
                CpuTemperature = 0,
                KernelSelfRestartCount = 0,
                IsRaiseHighCpuEvent = false,
                HighCpuPercent = 80,
                HighCpuSeconds = 10,
                LocalServerMessageTimestamp = Timestamp.UnixBaseTime,
                GpuTable = new GpuSpeedData[0]
            };
        }

        public static ClientData Create(ISpeedData speedData, string minerIp) {
            return new ClientData() {
                Id = ObjectId.NewObjectId().ToString(),
                MinerName = string.Empty,
                MinerIp = minerIp,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                GroupId = Guid.Empty,
                WorkId = Guid.Empty,
                WindowsLoginName = string.Empty,
                WindowsPassword = string.Empty,
                MACAddress = speedData.MACAddress,
                LocalIp = speedData.LocalIp,
                ClientId = speedData.ClientId,
                IsAutoBoot = speedData.IsAutoBoot,
                IsAutoStart = speedData.IsAutoStart,
                AutoStartDelaySeconds = speedData.AutoStartDelaySeconds,
                IsAutoRestartKernel = speedData.IsAutoRestartKernel,
                AutoRestartKernelTimes = speedData.AutoRestartKernelTimes,
                IsNoShareRestartKernel = speedData.IsNoShareRestartKernel,
                NoShareRestartKernelMinutes = speedData.NoShareRestartKernelMinutes,
                IsNoShareRestartComputer = speedData.IsNoShareRestartComputer,
                NoShareRestartComputerMinutes = speedData.NoShareRestartComputerMinutes,
                IsPeriodicRestartKernel = speedData.IsPeriodicRestartKernel,
                PeriodicRestartKernelHours = speedData.PeriodicRestartKernelHours,
                IsPeriodicRestartComputer = speedData.IsPeriodicRestartComputer,
                PeriodicRestartComputerHours = speedData.PeriodicRestartComputerHours,
                PeriodicRestartComputerMinutes = speedData.PeriodicRestartComputerMinutes,
                PeriodicRestartKernelMinutes = speedData.PeriodicRestartKernelMinutes,
                IsAutoStopByCpu = speedData.IsAutoStopByCpu,
                IsAutoStartByCpu = speedData.IsAutoStartByCpu,
                CpuStopTemperature = speedData.CpuStopTemperature,
                CpuStartTemperature = speedData.CpuStartTemperature,
                CpuLETemperatureSeconds = speedData.CpuLETemperatureSeconds,
                CpuGETemperatureSeconds = speedData.CpuGETemperatureSeconds,
                GpuDriver = speedData.GpuDriver,
                GpuType = speedData.GpuType,
                OSName = speedData.OSName,
                OSVirtualMemoryMb = speedData.OSVirtualMemoryMb,
                GpuInfo = speedData.GpuInfo,
                Version = speedData.Version,
                IsMining = speedData.IsMining,
                BootOn = speedData.BootOn,
                MineStartedOn = speedData.MineStartedOn,
                MainCoinCode = speedData.MainCoinCode,
                MainCoinTotalShare = speedData.MainCoinTotalShare,
                MainCoinRejectShare = speedData.MainCoinRejectShare,
                MainCoinSpeed = speedData.MainCoinSpeed,
                MainCoinPool = speedData.MainCoinPool,
                MainCoinWallet = speedData.MainCoinWallet,
                Kernel = speedData.Kernel,
                IsDualCoinEnabled = speedData.IsDualCoinEnabled,
                DualCoinPool = speedData.DualCoinPool,
                DualCoinWallet = speedData.DualCoinWallet,
                DualCoinCode = speedData.DualCoinCode,
                DualCoinTotalShare = speedData.DualCoinTotalShare,
                DualCoinRejectShare = speedData.DualCoinRejectShare,
                DualCoinSpeed = speedData.DualCoinSpeed,
                KernelCommandLine = speedData.KernelCommandLine,
                GpuTable = speedData.GpuTable,
                MineWorkId = speedData.MineWorkId,
                MineWorkName = speedData.MineWorkName,
                ClientName = speedData.MinerName,
                DiskSpace = speedData.DiskSpace,
                MainCoinPoolDelay = speedData.MainCoinPoolDelay,
                DualCoinPoolDelay = speedData.DualCoinPoolDelay,
                IsFoundOneGpuShare = speedData.IsFoundOneGpuShare,
                IsRejectOneGpuShare = speedData.IsRejectOneGpuShare,
                IsGotOneIncorrectGpuShare = speedData.IsGotOneIncorrectGpuShare,
                CpuTemperature = speedData.CpuTemperature,
                CpuPerformance = speedData.CpuPerformance,
                KernelSelfRestartCount = speedData.KernelSelfRestartCount,
                LocalServerMessageTimestamp = speedData.LocalServerMessageTimestamp,
                IsRaiseHighCpuEvent = speedData.IsRaiseHighCpuEvent,
                HighCpuPercent = speedData.HighCpuPercent,
                HighCpuSeconds = speedData.HighCpuSeconds
            };
        }

        private DateTime _preUpdateOn = DateTime.Now;
        private int _preMainCoinShare = 0;
        private int _preDualCoinShare = 0;
        private int _preMainCoinRejectShare = 0;
        private int _preDualCoinRejectShare = 0;
        private string _preMainCoin;
        private string _preDualCoin;

        public void Update(ISpeedData speedData, string minerIp) {
            this.MinerIp = minerIp;
            Update(speedData);
        }

        public void Update(ISpeedData speedData) {
            if (speedData == null) {
                return;
            }
            _preUpdateOn = DateTime.Now;
            if (_preMainCoin != this.MainCoinCode) {
                _preMainCoinShare = 0;
                _preMainCoinRejectShare = 0;
            }
            else {
                _preMainCoinShare = this.MainCoinTotalShare;
                _preMainCoinRejectShare = this.MainCoinRejectShare;
            }
            _preMainCoin = this.MainCoinCode;
            if (_preDualCoin != this.DualCoinCode) {
                _preDualCoinShare = 0;
                _preDualCoinRejectShare = 0;
            }
            else {
                _preDualCoinShare = this.DualCoinTotalShare;
                _preDualCoinRejectShare = this.DualCoinRejectShare;
            }
            _preDualCoin = this.DualCoinCode;

            this.ClientId = speedData.ClientId;
            if (!string.IsNullOrEmpty(speedData.MACAddress)) {
                this.MACAddress = speedData.MACAddress;
            }
            if (!string.IsNullOrEmpty(speedData.LocalIp)) {
                this.LocalIp = speedData.LocalIp;
            }
            this.IsAutoBoot = speedData.IsAutoBoot;
            this.IsAutoStart = speedData.IsAutoStart;
            this.AutoStartDelaySeconds = speedData.AutoStartDelaySeconds;
            this.IsAutoRestartKernel = speedData.IsAutoRestartKernel;
            this.AutoRestartKernelTimes = speedData.AutoRestartKernelTimes;
            this.IsNoShareRestartKernel = speedData.IsNoShareRestartKernel;
            this.NoShareRestartKernelMinutes = speedData.NoShareRestartKernelMinutes;
            this.IsNoShareRestartComputer = speedData.IsNoShareRestartComputer;
            this.NoShareRestartComputerMinutes = speedData.NoShareRestartComputerMinutes;
            this.IsPeriodicRestartKernel = speedData.IsPeriodicRestartKernel;
            this.PeriodicRestartKernelHours = speedData.PeriodicRestartKernelHours;
            this.IsPeriodicRestartComputer = speedData.IsPeriodicRestartComputer;
            this.PeriodicRestartComputerHours = speedData.PeriodicRestartComputerHours;
            this.PeriodicRestartComputerMinutes = speedData.PeriodicRestartComputerMinutes;
            this.PeriodicRestartKernelMinutes = speedData.PeriodicRestartKernelMinutes;
            this.IsAutoStopByCpu = speedData.IsAutoStopByCpu;
            this.IsAutoStartByCpu = speedData.IsAutoStartByCpu;
            this.CpuStopTemperature = speedData.CpuStopTemperature;
            this.CpuStartTemperature = speedData.CpuStartTemperature;
            this.CpuLETemperatureSeconds = speedData.CpuLETemperatureSeconds;
            this.CpuGETemperatureSeconds = speedData.CpuGETemperatureSeconds;
            this.GpuDriver = speedData.GpuDriver;
            this.GpuType = speedData.GpuType;
            this.OSName = speedData.OSName;
            this.OSVirtualMemoryMb = speedData.OSVirtualMemoryMb;
            this.GpuInfo = speedData.GpuInfo;

            this.Version = speedData.Version;
            this.IsMining = speedData.IsMining;
            this.BootOn = speedData.BootOn;
            this.MineStartedOn = speedData.MineStartedOn;
            this.ClientName = speedData.MinerName;
            this.DiskSpace = speedData.DiskSpace;
            this.ModifiedOn = DateTime.Now;
            this.MainCoinCode = speedData.MainCoinCode;
            this.MainCoinTotalShare = speedData.MainCoinTotalShare;
            this.MainCoinRejectShare = speedData.MainCoinRejectShare;
            this.MainCoinSpeed = speedData.MainCoinSpeed;
            this.MainCoinPool = speedData.MainCoinPool;
            this.MainCoinWallet = speedData.MainCoinWallet;
            this.Kernel = speedData.Kernel;
            this.IsDualCoinEnabled = speedData.IsDualCoinEnabled;
            this.DualCoinPool = speedData.DualCoinPool;
            this.DualCoinWallet = speedData.DualCoinWallet;
            this.DualCoinCode = speedData.DualCoinCode;
            this.DualCoinTotalShare = speedData.DualCoinTotalShare;
            this.DualCoinRejectShare = speedData.DualCoinRejectShare;
            this.DualCoinSpeed = speedData.DualCoinSpeed;
            this.KernelCommandLine = speedData.KernelCommandLine;
            this.GpuTable = speedData.GpuTable;
            this.MainCoinPoolDelay = speedData.MainCoinPoolDelay;
            this.DualCoinPoolDelay = speedData.DualCoinPoolDelay;
            this.IsFoundOneGpuShare = speedData.IsFoundOneGpuShare;
            this.IsRejectOneGpuShare = speedData.IsRejectOneGpuShare;
            this.IsGotOneIncorrectGpuShare = speedData.IsGotOneIncorrectGpuShare;
            this.CpuPerformance = speedData.CpuPerformance;
            this.CpuTemperature = speedData.CpuTemperature;
            this.MineWorkId = speedData.MineWorkId;
            this.MineWorkName = speedData.MineWorkName;
            this.KernelSelfRestartCount = speedData.KernelSelfRestartCount;
            this.LocalServerMessageTimestamp = speedData.LocalServerMessageTimestamp;
            this.IsRaiseHighCpuEvent = speedData.IsRaiseHighCpuEvent;
            this.HighCpuPercent = speedData.HighCpuPercent;
            this.HighCpuSeconds = speedData.HighCpuSeconds;
        }

        public int GetMainCoinShareDelta(bool isPull) {
            if (_preMainCoinShare == 0) {
                return 0;
            }
            if (this.IsMining == false || string.IsNullOrEmpty(this.MainCoinCode)) {
                return 0;
            }
            if (isPull) {
                if (this._preUpdateOn.AddSeconds(5) > DateTime.Now) {
                    return 0;
                }
            }
            else if (this._preUpdateOn.AddSeconds(115) > DateTime.Now) {
                return 0;
            }

            int delta = this.MainCoinTotalShare - _preMainCoinShare;
            if (delta < 0) {
                delta = 0;
            }
            return delta;
        }

        public int GetDualCoinShareDelta(bool isPull) {
            if (_preDualCoinShare == 0) {
                return 0;
            }
            if (this.IsMining == false || string.IsNullOrEmpty(this.DualCoinCode)) {
                return 0;
            }
            if (isPull) {
                if (this._preUpdateOn.AddSeconds(5) > DateTime.Now) {
                    return 0;
                }
            }
            else if (this._preUpdateOn.AddSeconds(115) > DateTime.Now) {
                return 0;
            }

            int delta = this.DualCoinTotalShare - _preDualCoinShare;
            if (delta < 0) {
                delta = 0;
            }
            return delta;
        }

        public int GetMainCoinRejectShareDelta(bool isPull) {
            if (_preMainCoinRejectShare == 0) {
                return 0;
            }
            if (this.IsMining == false || string.IsNullOrEmpty(this.MainCoinCode)) {
                return 0;
            }
            if (isPull && this._preUpdateOn.AddSeconds(5) > DateTime.Now) {
                return 0;
            }
            else if (this._preUpdateOn.AddSeconds(115) > DateTime.Now) {
                return 0;
            }

            int delta = this.MainCoinRejectShare - _preMainCoinRejectShare;
            if (delta < 0) {
                delta = 0;
            }
            return delta;
        }

        public int GetDualCoinRejectShareDelta(bool isPull) {
            if (_preDualCoinRejectShare == 0) {
                return 0;
            }
            if (this.IsMining == false || string.IsNullOrEmpty(this.DualCoinCode)) {
                return 0;
            }
            if (isPull) {
                if (this._preUpdateOn.AddSeconds(5) > DateTime.Now) {
                    return 0;
                }
            }
            else if (this._preUpdateOn.AddSeconds(115) > DateTime.Now) {
                return 0;
            }

            int delta = this.DualCoinRejectShare - _preDualCoinRejectShare;
            if (delta < 0) {
                delta = 0;
            }
            return delta;
        }

        public string Id { get; set; }

        /// <summary>
        /// 服务的指定的作业
        /// </summary>
        public Guid WorkId { get; set; }

        public string ClientName { get; set; }

        public string WindowsLoginName { get; set; }

        public string WindowsPassword { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid GroupId { get; set; }
    }
}
