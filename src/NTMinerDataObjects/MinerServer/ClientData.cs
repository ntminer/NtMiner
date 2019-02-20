using System;

namespace NTMiner.MinerServer {
    public class ClientData : IClientData, IDbEntity<Guid>, ITimestampEntity<Guid> {
        public ClientData() {
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

        public int BootSeconds { get; set; }

        public int MineSeconds { get; set; }

        public string MainCoinCode {
            get => _mainCoinCode ?? string.Empty;
            set => _mainCoinCode = value;
        }

        public double MainCoinSpeed { get; set; }

        public int RejectShareCount { get; set; }

        public int TotalShareCount { get; set; }

        public string MainCoinPool { get; set; }

        public string MainCoinWallet { get; set; }

        public string Kernel { get; set; }

        public bool IsDualCoinEnabled { get; set; }

        public string DualCoinCode {
            get => _dualCoinCode ?? string.Empty;
            set => _dualCoinCode = value;
        }

        public double DualCoinSpeed { get; set; }

        public int DualRejectShareCount { get; set; }

        public int DualTotalShareCount { get; set; }

        public string DualCoinPool { get; set; }

        public string DualCoinWallet { get; set; }

        public string OSName { get; set; }

        public int OSVirtualMemory { get; set; }

        public GpuType GpuType { get; set; }

        public string GpuDriver { get; set; }

        public string GpuInfo { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid GroupId { get; set; }
    }
}
