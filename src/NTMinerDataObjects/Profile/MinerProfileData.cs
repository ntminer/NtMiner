using System;
using System.Text;

namespace NTMiner.Profile {
    public class MinerProfileData : IMinerProfile, IDbEntity<Guid>, IGetSignData {
        public static MinerProfileData CreateDefaultData(Guid coinId) {
            return new MinerProfileData {
                Id = Guid.Parse("7d9eec49-2d1f-44fa-881e-571a78661ca0"),
                MinerName = string.Empty,
                CoinId = coinId,
                IsAutoRestartKernel = true,
                IsNoShareRestartKernel = false,
                NoShareRestartKernelMinutes = 30,
                IsPeriodicRestartKernel = false,
                PeriodicRestartKernelHours = 12,
                IsPeriodicRestartComputer = false,
                PeriodicRestartComputerHours = 24
            };
        }

        public MinerProfileData() { }

        public MinerProfileData(IMinerProfile data) {
            this.Id = data.CoinId;
            this.MinerName = data.MinerName;
            this.CoinId = data.CoinId;
            this.IsAutoRestartKernel = data.IsAutoRestartKernel;
            this.IsNoShareRestartKernel = data.IsNoShareRestartKernel;
            this.NoShareRestartKernelMinutes = data.NoShareRestartKernelMinutes;
            this.IsPeriodicRestartKernel = data.IsPeriodicRestartKernel;
            this.PeriodicRestartKernelHours = data.PeriodicRestartKernelHours;
            this.IsPeriodicRestartComputer = data.IsPeriodicRestartComputer;
            this.PeriodicRestartComputerHours = data.PeriodicRestartComputerHours;
        }

        public Guid GetId() {
            return this.Id;
        }
        public Guid Id { get; set; }
        public string MinerName { get; set; }
        public bool IsAutoRestartKernel { get; set; }
        public Guid CoinId { get; set; }
        public bool IsNoShareRestartKernel { get; set; }
        public int NoShareRestartKernelMinutes { get; set; }
        public bool IsPeriodicRestartKernel { get; set; }
        public int PeriodicRestartKernelHours { get; set; }
        public bool IsPeriodicRestartComputer { get; set; }
        public int PeriodicRestartComputerHours { get; set; }

        public override string ToString() {
            return $"{Id}{MinerName}{IsAutoRestartKernel}{CoinId}{IsNoShareRestartKernel}{NoShareRestartKernelMinutes}{IsPeriodicRestartKernel}{PeriodicRestartKernelHours}{IsPeriodicRestartComputer}{PeriodicRestartComputerHours}";
        }

        public StringBuilder GetSignData() {
            return new StringBuilder(this.ToString());
        }
    }
}
