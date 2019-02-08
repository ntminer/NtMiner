using System;

namespace NTMiner.MinerServer {
    public class CoinProfileRequest {
        public Guid WorkId { get; set; }
        public Guid CoinId { get; set; }
    }
}
