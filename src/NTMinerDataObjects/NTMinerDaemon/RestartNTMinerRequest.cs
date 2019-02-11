using System;

namespace NTMiner.NTMinerDaemon {
    public class RestartNTMinerRequest {
        public RestartNTMinerRequest() { }
        public Guid WorkId { get; set; }
    }
}
