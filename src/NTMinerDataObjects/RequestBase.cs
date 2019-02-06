using System;

namespace NTMiner {
    public class RequestBase {
        public RequestBase() {
            this.MessageId = Guid.NewGuid();
        }

        public Guid MessageId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
