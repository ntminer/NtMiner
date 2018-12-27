using System;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class MinerProfileData : IMinerProfile, IDbEntity<Guid>, ITimestampEntity<Guid> {
        public static MinerProfileData CreateDefaultData() {
            return new MinerProfileData {
                Id = Guid.Parse("7d9eec49-2d1f-44fa-881e-571a78661ca0"),
                MinerName = Environment.MachineName,
                IsShowInTaskbar = true,
                IsLocalizationEnabled = true,
                CoinId = Guid.Empty,
                IsAutoBoot = false,
                IsAutoStart = false,
                IsAutoRestartKernel = true,
                IsNoShareRestartKernel = false,
                IsPeriodicRestartComputer = false,
                IsPeriodicRestartKernel = false,
                NoShareRestartKernelMinutes = 30,
                PeriodicRestartComputerHours = 24,
                PeriodicRestartKernelHours = 12,
                IsAutoThisPCName = true,
                ServerHost = Server.MINER_SERVER_HOST,
                ServerPort = 3336,
                CreatedOn = DateTime.Now,
                ModifiedOn = Global.UnixBaseTime
            };
        }

        public MinerProfileData() { }

        public Guid GetId() {
            return this.Id;
        }
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string MinerName { get; set; }
        [DataMember]
        public bool IsAutoThisPCName { get; set; }
        [DataMember]
        public bool IsShowInTaskbar { get; set; }
        [DataMember]
        public bool IsLocalizationEnabled { get; set; }

        [DataMember]
        public bool IsAutoBoot { get; set; }
        [DataMember]
        public bool IsAutoStart { get; set; }
        [DataMember]
        public bool IsAutoRestartKernel { get; set; }
        [DataMember]
        public string ServerHost { get; set; }
        [DataMember]
        public int ServerPort { get; set; }
        [DataMember]
        public Guid CoinId { get; set; }
        [DataMember]
        public bool IsNoShareRestartKernel { get; set; }
        [DataMember]
        public int NoShareRestartKernelMinutes { get; set; }
        [DataMember]
        public bool IsPeriodicRestartKernel { get; set; }
        [DataMember]
        public int PeriodicRestartKernelHours { get; set; }
        [DataMember]
        public bool IsPeriodicRestartComputer { get; set; }
        [DataMember]
        public int PeriodicRestartComputerHours { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public DateTime ModifiedOn { get; set; }
    }
}
