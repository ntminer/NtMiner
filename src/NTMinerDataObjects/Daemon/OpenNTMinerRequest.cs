using System;

namespace NTMiner.Daemon {
    public class OpenNTMinerRequest : RequestBase {
        public OpenNTMinerRequest() { }

        public Guid WorkId { get; set; }
    }
}
