using System;

namespace NTMiner.Core.MinerServer {
    public class MinerSign : IMinerSign {
        public static MinerSign Create(MinerData minerData) {
            return new MinerSign {
                Id = minerData.Id,
                ClientId = minerData.ClientId,
                LoginName = minerData.LoginName,
                OuterUserId = minerData.OuterUserId,
                AESPassword = minerData.AESPassword,
                AESPasswordOn = minerData.AESPasswordOn
            };
        }

        public MinerSign() { }

        public void Update(MinerData minerData) {
            this.ClientId = minerData.ClientId;
            this.LoginName = minerData.LoginName;
            this.OuterUserId = minerData.OuterUserId;
            this.AESPassword = minerData.AESPassword;
            this.AESPasswordOn = minerData.AESPasswordOn;
        }

        public string Id { get; set; }

        public Guid ClientId { get; set; }

        public string LoginName { get; set; }

        public string OuterUserId { get; set; }

        public string AESPassword { get; set; }

        public DateTime AESPasswordOn { get; set; }
    }
}
