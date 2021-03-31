using System;

namespace NTMiner.Core.Mq {
    public class ClientIdIp {
        public ClientIdIp(Guid clientId, string minerIp) {
            this.ClientId = clientId;
            this.MinerIp = minerIp;
        }

        public Guid ClientId { get; private set; }
        public string MinerIp { get; private set; }
    }
}
