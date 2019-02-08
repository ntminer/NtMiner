using System;

namespace NTMiner.MinerClient {
    public class SetMinerProfilePropertyRequest {
        public string PropertyName { get; set; }
        public object Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
