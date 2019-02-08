using System;

namespace NTMiner.MinerServer {
    public class PoolProfileRequest {
        public Guid WorkId { get; set; }
        public Guid PoolId { get; set; }
    }
}
