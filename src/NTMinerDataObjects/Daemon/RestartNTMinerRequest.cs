using System;

namespace NTMiner.Daemon {
    public class RestartNTMinerRequest : RequestBase {
        public RestartNTMinerRequest() { }
        public Guid WorkId { get; set; }
    }
}
