using System;

namespace NTMiner.MinerServer {
    public class AppSettingRequest {
        public Guid MessageId { get; set; }
        public string Key { get; set; }
    }
}
