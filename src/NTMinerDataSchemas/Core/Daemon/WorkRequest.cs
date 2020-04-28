using System;

namespace NTMiner.Core.Daemon {
    public class WorkRequest : IRequest {
        public WorkRequest() { }

        public Guid WorkId { get; set; }
        public string WorkerName { get; set; }
        public string LocalJson { get; set; }
        public string ServerJson { get; set; }
    }
}
