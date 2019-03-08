using System;

namespace NTMiner.Profile {
    public class MinerProfileData : IMinerProfile, IDbEntity<Guid>, ITimestampEntity<Guid> {
        public static MinerProfileData CreateDefaultData() {
            return new MinerProfileData {
                Id = Guid.Parse("7d9eec49-2d1f-44fa-881e-571a78661ca0"),
                CoinId = Guid.Empty,
                IsAutoBoot = false,
                IsAutoStart = false,
                IsAutoRestartKernel = true,
                IsNoShareRestartKernel = false,
                NoShareRestartKernelMinutes = 30,
                IsPeriodicRestartKernel = false,
                PeriodicRestartKernelHours = 12,
                IsPeriodicRestartComputer = false,
                PeriodicRestartComputerHours = 24,
                CreatedOn = DateTime.Now,
                ModifiedOn = Timestamp.UnixBaseTime
            };
        }

        public MinerProfileData() { }

        public MinerProfileData(IMinerProfile data) {
            this.Id = data.CoinId;
            this.CoinId = data.CoinId;
            this.IsAutoBoot = data.IsAutoBoot;
            this.IsAutoStart = data.IsAutoStart;
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
        public bool IsAutoBoot { get; set; }
        public bool IsAutoStart { get; set; }
        public bool IsAutoRestartKernel { get; set; }
        public Guid CoinId { get; set; }
        public bool IsNoShareRestartKernel { get; set; }
        public int NoShareRestartKernelMinutes { get; set; }
        public bool IsPeriodicRestartKernel { get; set; }
        public int PeriodicRestartKernelHours { get; set; }
        public bool IsPeriodicRestartComputer { get; set; }
        public int PeriodicRestartComputerHours { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
