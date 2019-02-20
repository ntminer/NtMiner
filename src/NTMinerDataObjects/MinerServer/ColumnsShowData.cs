using System;

namespace NTMiner.MinerServer {
    public class ColumnsShowData : IDbEntity<Guid>, IColumnsShow {
        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public bool Work { get; set; }

        public bool MinerName{ get; set; }

        public bool MinerIp{ get; set; }

        public bool MinerGroup{ get; set; }

        public bool MainCoinCode{ get; set; }

        public bool MainCoinSpeedText{ get; set; }

        public bool MainCoinWallet{ get; set; }

        public bool MainCoinPool{ get; set; }

        public bool Kernel{ get; set; }

        public bool DualCoinCode{ get; set; }

        public bool DualCoinSpeedText{ get; set; }

        public bool DualCoinWallet{ get; set; }

        public bool DualCoinPool{ get; set; }

        public bool LastActivedOnText{ get; set; }

        public bool Version{ get; set; }

        public bool RemoteDesktop{ get; set; }

        public bool RemotePassword{ get; set; }

        public bool GpuInfo{ get; set; }
    }
}
