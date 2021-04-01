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

        public void Update(IMinerSign minerSign) {
            this.ClientId = minerSign.ClientId;
            this.LoginName = minerSign.LoginName;
            this.OuterUserId = minerSign.OuterUserId;
            this.AESPassword = minerSign.AESPassword;
            this.AESPasswordOn = minerSign.AESPasswordOn;
        }

        public string Id { get; set; }

        public Guid ClientId { get; set; }

        public string LoginName { get; set; }

        public string OuterUserId { get; set; }

        public string AESPassword { get; set; }

        public DateTime AESPasswordOn { get; set; }
    }
}
