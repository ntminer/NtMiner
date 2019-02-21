using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class ColumnsShowData : IDbEntity<Guid>, IColumnsShow {
        public static readonly Guid PleaseSelectId = Guid.Parse("197f19e8-0c1b-4018-875d-2f5e56a02491");

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string ColumnsShowName { get; set; }

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

        public bool RemoteUserNameAndPassword{ get; set; }

        public bool RemotePassword{ get; set; }

        public bool GpuInfo{ get; set; }

        public string GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Id)).Append(Id)
                .Append(nameof(Work)).Append(Work)
                .Append(nameof(MinerName)).Append(MinerName)
                .Append(nameof(MinerIp)).Append(MinerIp)
                .Append(nameof(MinerIp)).Append(MinerGroup)
                .Append(nameof(MinerIp)).Append(MainCoinCode)
                .Append(nameof(MinerIp)).Append(MainCoinSpeedText)
                .Append(nameof(MinerIp)).Append(MainCoinWallet)
                .Append(nameof(MinerIp)).Append(MainCoinPool)
                .Append(nameof(MinerIp)).Append(Kernel)
                .Append(nameof(MinerIp)).Append(DualCoinCode)
                .Append(nameof(MinerIp)).Append(DualCoinSpeedText)
                .Append(nameof(MinerIp)).Append(DualCoinWallet)
                .Append(nameof(MinerIp)).Append(DualCoinPool)
                .Append(nameof(MinerIp)).Append(LastActivedOnText)
                .Append(nameof(MinerIp)).Append(Version)
                .Append(nameof(MinerIp)).Append(RemoteUserNameAndPassword)
                .Append(nameof(MinerIp)).Append(RemotePassword)
                .Append(nameof(MinerIp)).Append(GpuInfo);
            return sb.ToString();
        }
    }
}
