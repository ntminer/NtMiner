using System;
using NTMiner.Hashrate;

namespace NTMiner.MinerServer {
    public class ClientData : IClientData, IDbEntity<Guid>, ITimestampEntity<Guid> {
        public ClientData() {
            this.GpuSpeeds = new GpuSpeedData[0];
        }

        public Guid GetId() {
            return this.Id;
        }

        private string _mainCoinCode;
        private string _dualCoinCode;
        public Guid Id { get; set; }

        public Guid WorkId { get; set; }

        public string Version { get; set; }

        public bool IsMining { get; set; }

        public string MinerName { get; set; }

        public string MinerIp { get; set; }

        public string RemoteUserName { get; set; }

        public string RemotePassword { get; set; }

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

        public string OSName { get; set; }

        public double OSVirtualMemory { get; set; }

        public GpuType GpuType { get; set; }

        public string GpuDriver { get; set; }

        public string GpuInfo { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid GroupId { get; set; }

        public GpuSpeedData[] GpuSpeeds { get; set; }
    }
}
