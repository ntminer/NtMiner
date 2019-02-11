using System;

namespace NTMiner.Profile {
    public class PoolProfileRequest {
        public PoolProfileRequest() { }
        public Guid WorkId { get; set; }
        public Guid PoolId { get; set; }
    }
}
