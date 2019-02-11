using System;

namespace NTMiner.Daemon {
    public class RestartNTMinerRequest {
        public RestartNTMinerRequest() { }
        public Guid WorkId { get; set; }
    }
}
