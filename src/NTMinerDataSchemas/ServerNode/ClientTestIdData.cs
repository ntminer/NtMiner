using System;

namespace NTMiner.ServerNode {
    public class ClientTestIdData : IClientTestId {
        public ClientTestIdData() { }

        public Guid MinerClientTestId { get; set; }

        public Guid StudioClientTestId { get; set; }
    }
}
