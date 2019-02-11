using System;

namespace NTMiner.Profile {
    public class CoinProfileRequest {
        public CoinProfileRequest() { }
        public Guid WorkId { get; set; }
        public Guid CoinId { get; set; }
    }
}
