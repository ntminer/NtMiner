using NTMiner.MinerClient;
using System;
using LiteDB;

namespace NTMiner.MinerServer {
    public class ClientData : IClientData, IDbEntity<string>, ITimestampEntity<string> {
        public ClientData() {
            this.GpuTable = new GpuSpeedData[0];
        }

        public string GetId() {
            return this.Id;
        }

        public static ClientData Create(SpeedData speedData, string minerIp) {
            return new ClientData() {
                Id = ObjectId.NewObjectId().ToString(),
                ClientId = speedData.ClientId,
                IsAutoBoot = speedData.IsAutoBoot,
                IsAutoStart = speedData.IsAutoStart,
                IsAutoRestartKernel = speedData.IsAutoRestartKernel,
                IsNoShareRestartKernel = speedData.IsNoShareRestartKernel,
                NoShareRestartKernelMinutes = speedData.NoShareRestartKernelMinutes,
                IsPeriodicRestartKernel = speedData.IsPeriodicRestartKernel,
                PeriodicRestartKernelHours = speedData.PeriodicRestartKernelHours,
                IsPeriodicRestartComputer = speedData.IsPeriodicRestartComputer,
                PeriodicRestartComputerHours = speedData.PeriodicRestartComputerHours,
                MinerName = string.Empty,
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
                WindowsPassword = string.Empty,
                ClientName = speedData.MinerName,
                DiskSpace = speedData.DiskSpace
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

            this.ClientId = speedData.ClientId;
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
        }

        private int _preMainCoinShare = 0;
        private int _preDualCoinShare = 0;
        private int _preMainCoinRejectShare = 0;
        private int _preDualCoinRejectShare = 0;
        private string _preMainCoin;
        private string _preDualCoin;
        private DateTime _preMainCoinDeltaOn;
        private DateTime _preDualCoinDeltaOn;

        public int GetMainCoinShareDelta() {
            if (this.IsMining == false || string.IsNullOrEmpty(this.MainCoinCode) || this.ModifiedOn.AddSeconds(20) < DateTime.Now) {
                return 0;
            }

            int delta = 0;

            if (_preMainCoin == this.MainCoinCode) {
                if (_preMainCoinShare != 0 && _preMainCoinDeltaOn.AddSeconds(20) > DateTime.Now) {
                    delta = this.MainCoinTotalShare - _preMainCoinShare;
                    if (delta < 0) {
                        delta = 0;
                    }
                }
                _preMainCoinShare = this.MainCoinTotalShare;
            }
            else {
                _preMainCoinShare = 0;
                _preMainCoin = this.MainCoinCode;
            }

            _preMainCoinDeltaOn = DateTime.Now;

            return delta;
        }

        public int GetDualCoinShareDelta() {
            if (this.IsMining == false || string.IsNullOrEmpty(this.DualCoinCode) || this.ModifiedOn.AddSeconds(20) < DateTime.Now) {
                return 0;
            }

            int delta = 0;

            if (_preDualCoin == this.DualCoinCode) {
                if (_preDualCoinShare != 0 && _preDualCoinDeltaOn.AddSeconds(20) > DateTime.Now) {
                    delta = this.DualCoinTotalShare - _preDualCoinShare;
                    if (delta < 0) {
                        delta = 0;
                    }
                }
                _preDualCoinShare = this.DualCoinTotalShare;
            }
            else {
                _preDualCoinShare = 0;
                _preDualCoin = this.DualCoinCode;
            }

            _preDualCoinDeltaOn = DateTime.Now;

            return delta;
        }

        public int GetMainCoinRejectShareDelta() {
            if (this.IsMining == false || string.IsNullOrEmpty(this.MainCoinCode) || this.ModifiedOn.AddSeconds(20) < DateTime.Now) {
                return 0;
            }

            int delta = 0;

            if (_preMainCoin == this.MainCoinCode) {
                if (_preMainCoinRejectShare != 0) {
                    delta = this.MainCoinRejectShare - _preMainCoinRejectShare;
                    if (delta < 0) {
                        delta = 0;
                    }
                }
                _preMainCoinRejectShare = this.MainCoinRejectShare;
            }
            else {
                _preMainCoinRejectShare = 0;
                _preMainCoin = this.MainCoinCode;
            }

            return delta;
        }

        public int GetDualCoinRejectShareDelta() {
            if (this.IsMining == false || string.IsNullOrEmpty(this.DualCoinCode) || this.ModifiedOn.AddSeconds(20) < DateTime.Now) {
                return 0;
            }

            int delta = 0;

            if (_preDualCoin == this.DualCoinCode) {
                if (_preDualCoinRejectShare != 0) {
                    delta = this.DualCoinRejectShare - _preDualCoinRejectShare;
                    if (delta < 0) {
                        delta = 0;
                    }
                }
                _preDualCoinRejectShare = this.DualCoinRejectShare;
            }
            else {
                _preDualCoinRejectShare = 0;
                _preDualCoin = this.DualCoinCode;
            }

            return delta;
        }

        private string _mainCoinCode;
        private string _dualCoinCode;
        public string Id { get; set; }

        public Guid ClientId { get; set; }
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
        public string DiskSpace { get; set; }
        public GpuType GpuType { get; set; }
        public string GpuDriver { get; set; }
        public string GpuInfo { get; set; }

        public Guid WorkId { get; set; }

        public string Version { get; set; }

        public bool IsMining { get; set; }

        public string MinerName { get; set; }

        public string ClientName { get; set; }

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
