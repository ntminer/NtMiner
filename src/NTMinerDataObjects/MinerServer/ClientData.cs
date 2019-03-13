using NTMiner.MinerClient;
using System;

namespace NTMiner.MinerServer {
    public class ClientData : IClientData, IDbEntity<Guid>, ITimestampEntity<Guid> {
        public ClientData() {
            this.GpuTable = new GpuSpeedData[0];
        }

        public Guid GetId() {
            return this.Id;
        }

        public static ClientData Create(SpeedData speedData, string minerIp) {
            return new ClientData() {
                Id = speedData.ClientId,
                IsAutoBoot = speedData.IsAutoBoot,
                IsAutoStart = speedData.IsAutoStart,
                IsAutoRestartKernel = speedData.IsAutoRestartKernel,
                IsNoShareRestartKernel = speedData.IsNoShareRestartKernel,
                NoShareRestartKernelMinutes = speedData.NoShareRestartKernelMinutes,
                IsPeriodicRestartKernel = speedData.IsPeriodicRestartKernel,
                PeriodicRestartKernelHours = speedData.PeriodicRestartKernelHours,
                IsPeriodicRestartComputer = speedData.IsPeriodicRestartComputer,
                PeriodicRestartComputerHours = speedData.PeriodicRestartComputerHours,
                GpuDriver = speedData.GpuDriver,
                GpuType = speedData.GpuType,
                OSName = speedData.OSName,
                OSVirtualMemoryMb = speedData.OSVirtualMemoryMb,
                GpuInfo = speedData.GpuInfo,
                Version = speedData.Version,
                IsMining = speedData.IsMining,
                BootOn = speedData.BootOn,
                MineStartedOn = speedData.MineStartedOn,
                MinerIp = minerIp,
                MinerName = speedData.MinerName,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
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
                GroupId = Guid.Empty,
                WorkId = Guid.Empty,
                WindowsLoginName = string.Empty,
                WindowsPassword = string.Empty
            };
        }

        public void Update(SpeedData speedData, string minerIp) {
            this.MinerIp = minerIp;
            Update(speedData);
        }

        public void Update(SpeedData speedData) {
            if (speedData == null) {
                return;
            }
            this.IsAutoBoot = speedData.IsAutoBoot;
            this.IsAutoStart = speedData.IsAutoStart;
            this.IsAutoRestartKernel = speedData.IsAutoRestartKernel;
            this.IsNoShareRestartKernel = speedData.IsNoShareRestartKernel;
            this.NoShareRestartKernelMinutes = speedData.NoShareRestartKernelMinutes;
            this.IsPeriodicRestartKernel = speedData.IsPeriodicRestartKernel;
            this.PeriodicRestartKernelHours = speedData.PeriodicRestartKernelHours;
            this.IsPeriodicRestartComputer = speedData.IsPeriodicRestartComputer;
            this.PeriodicRestartComputerHours = speedData.PeriodicRestartComputerHours;
            this.GpuDriver = speedData.GpuDriver;
            this.GpuType = speedData.GpuType;
            this.OSName = speedData.OSName;
            this.OSVirtualMemoryMb = speedData.OSVirtualMemoryMb;
            this.GpuInfo = speedData.GpuInfo;

            this.Version = speedData.Version;
            this.IsMining = speedData.IsMining;
            this.BootOn = speedData.BootOn;
            this.MineStartedOn = speedData.MineStartedOn;
            this.MinerName = speedData.MinerName;
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
        }

        private string _mainCoinCode;
        private string _dualCoinCode;
        public Guid Id { get; set; }

        public bool IsAutoBoot { get; set; }
        public bool IsAutoStart { get; set; }
        public bool IsAutoRestartKernel { get; set; }
        public bool IsNoShareRestartKernel { get; set; }
        public int NoShareRestartKernelMinutes { get; set; }
        public bool IsPeriodicRestartKernel { get; set; }
        public int PeriodicRestartKernelHours { get; set; }
        public bool IsPeriodicRestartComputer { get; set; }
        public int PeriodicRestartComputerHours { get; set; }
        public string OSName { get; set; }
        public int OSVirtualMemoryMb { get; set; }
        public GpuType GpuType { get; set; }
        public string GpuDriver { get; set; }
        public string GpuInfo { get; set; }

        public Guid WorkId { get; set; }

        public string Version { get; set; }

        public bool IsMining { get; set; }

        public string MinerName { get; set; }

        public string MinerIp { get; set; }

        public string WindowsLoginName { get; set; }

        public string WindowsPassword { get; set; }

        public DateTime BootOn { get; set; }

        public DateTime? MineStartedOn { get; set; }

        public string MainCoinCode {
            get => _mainCoinCode ?? string.Empty;
            set => _mainCoinCode = value;
        }

        public double MainCoinSpeed { get; set; }

        public int MainCoinRejectShare { get; set; }

        public int MainCoinTotalShare { get; set; }

        public string MainCoinPool { get; set; }

        public string MainCoinWallet { get; set; }

        public string Kernel { get; set; }

        public bool IsDualCoinEnabled { get; set; }

        public string DualCoinCode {
            get => _dualCoinCode ?? string.Empty;
            set => _dualCoinCode = value;
        }

        public double DualCoinSpeed { get; set; }

        public int DualCoinRejectShare { get; set; }

        public int DualCoinTotalShare { get; set; }

        public string DualCoinPool { get; set; }

        public string DualCoinWallet { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid GroupId { get; set; }

        public string KernelCommandLine { get; set; }

        public GpuSpeedData[] GpuTable { get; set; }
    }
}
