using System;
using System.Text;

namespace NTMiner.Profile {
    public class MinerProfileData : IMinerProfile, IDbEntity<Guid>, IGetSignData {
        public static readonly Guid DefaultId = Guid.Parse("7d9eec49-2d1f-44fa-881e-571a78661ca0");
        public static MinerProfileData CreateDefaultData(Guid coinId) {
            return new MinerProfileData {
                Id = DefaultId,
                CoinId = coinId
            };
        }

        public MinerProfileData() {
            this.MinerName = string.Empty;
            this.IsAutoRestartKernel = true;
            this.AutoRestartKernelTimes = 10;
            this.IsNoShareRestartKernel = false;
            this.NoShareRestartKernelMinutes = 15;
            this.IsNoShareRestartComputer = false;
            this.NoShareRestartComputerMinutes = 30;
            this.IsPeriodicRestartKernel = false;
            this.PeriodicRestartKernelHours = 12;
            this.PeriodicRestartKernelMinutes = 0;
            this.IsPeriodicRestartComputer = false;
            this.PeriodicRestartComputerHours = 24;
            this.PeriodicRestartComputerMinutes = 0;
            this.IsSpeedDownRestartComputer = false;
            this.RestartComputerSpeedDownPercent = 0;
            this.IsEChargeEnabled = true;
            this.EPrice = 0.3;
            this.IsPowerAppend = false;
            this.PowerAppend = 0;
            this.MaxTemp = 80;
            this.AutoStartDelaySeconds = 15;
        }

        public MinerProfileData(IMinerProfile data) {
            this.Id = data.CoinId;
            this.MinerName = data.MinerName;
            this.CoinId = data.CoinId;
            this.IsAutoRestartKernel = data.IsAutoRestartKernel;
            this.AutoRestartKernelTimes = data.AutoRestartKernelTimes;
            this.IsNoShareRestartKernel = data.IsNoShareRestartKernel;
            this.NoShareRestartKernelMinutes = data.NoShareRestartKernelMinutes;
            this.IsNoShareRestartComputer = data.IsNoShareRestartComputer;
            this.NoShareRestartComputerMinutes = data.NoShareRestartComputerMinutes;
            this.IsPeriodicRestartKernel = data.IsPeriodicRestartKernel;
            this.PeriodicRestartKernelHours = data.PeriodicRestartKernelHours;
            this.PeriodicRestartKernelHours = data.PeriodicRestartKernelHours;
            this.IsPeriodicRestartComputer = data.IsPeriodicRestartComputer;
            this.PeriodicRestartComputerHours = data.PeriodicRestartComputerHours;
            this.PeriodicRestartComputerMinutes = data.PeriodicRestartComputerMinutes;
            this.IsSpeedDownRestartComputer = data.IsSpeedDownRestartComputer;
            this.RestartComputerSpeedDownPercent = data.RestartComputerSpeedDownPercent;
            this.IsEChargeEnabled = data.IsEChargeEnabled;
            this.EPrice = data.EPrice;
            this.IsPowerAppend = data.IsPowerAppend;
            this.PowerAppend = data.PowerAppend;
            this.MaxTemp = data.MaxTemp;
            this.AutoStartDelaySeconds = data.AutoStartDelaySeconds;
        }

        public Guid GetId() {
            return this.Id;
        }
        public Guid Id { get; set; }
        public string MinerName { get; set; }
        public bool IsAutoRestartKernel { get; set; }
        public int AutoRestartKernelTimes { get; set; }
        public Guid CoinId { get; set; }
        public bool IsNoShareRestartKernel { get; set; }
        public int NoShareRestartKernelMinutes { get; set; }
        public bool IsNoShareRestartComputer { get; set; }
        public int NoShareRestartComputerMinutes { get; set; }
        public bool IsPeriodicRestartKernel { get; set; }
        public int PeriodicRestartKernelHours { get; set; }
        public bool IsPeriodicRestartComputer { get; set; }
        public int PeriodicRestartComputerHours { get; set; }

        public bool IsSpeedDownRestartComputer { get; set; }

        public int RestartComputerSpeedDownPercent { get; set; }

        public bool IsEChargeEnabled { get; set; }

        public double EPrice { get; set; }

        public bool IsPowerAppend { get; set; }

        public int PowerAppend { get; set; }

        public int PeriodicRestartKernelMinutes { get; set; }

        public int PeriodicRestartComputerMinutes { get; set; }

        public int MaxTemp { get; set; }

        public int AutoStartDelaySeconds { get; set; }

        public override string ToString() {
            return this.BuildSign().ToString();
        }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
