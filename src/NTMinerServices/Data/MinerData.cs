using System;
using LiteDB;
using NTMiner.MinerClient;
using NTMiner.MinerServer;

namespace NTMiner.Data {
    public class MinerData {
        public static ClientData CreateClientData(MinerData data) {
            return new ClientData() {
                ClientId = data.ClientId,
                MinerIp = data.MinerIp,
                MinerName = data.MinerName,
                ClientName = string.Empty,
                CreatedOn = data.CreatedOn,
                GroupId = data.GroupId,
                WorkId = data.WorkId,
                WindowsLoginName = data.WindowsLoginName,
                WindowsPassword = data.WindowsPassword,
                Id = data.Id,
                IsAutoBoot = false,
                IsAutoStart = false,
                IsAutoRestartKernel = false,
                IsNoShareRestartKernel = false,
                NoShareRestartKernelMinutes = 0,
                IsPeriodicRestartKernel = false,
                PeriodicRestartKernelHours = 0,
                IsPeriodicRestartComputer = false,
                PeriodicRestartComputerHours = 0,
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
                GpuTable = new GpuSpeedData[0]
            };
        }

        public string Id { get; set; }
        public Guid ClientId { get; set; }
        public string MinerName { get; set; }
        public Guid WorkId { get; set; }
        public string MinerIp { get; set; }
        public string WindowsLoginName { get; set; }

        public string WindowsPassword { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid GroupId { get; set; }
    }
}
