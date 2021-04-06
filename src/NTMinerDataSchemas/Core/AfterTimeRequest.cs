using System;

namespace NTMiner.Core {
    public class AfterTimeRequest {
        public AfterTimeRequest() { }

        public string LoginName { get; set; }
        public Guid StudioId { get; set; }
        public Guid ClientId { get; set; }
        public long AfterTime { get; set; }
    }
}
