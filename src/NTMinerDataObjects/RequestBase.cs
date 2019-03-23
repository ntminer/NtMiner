using System;

namespace NTMiner {
    public class RequestBase {
        public RequestBase() {
            this.Timestamp = DateTime.Now;
        }

        public DateTime Timestamp { get; set; }
    }
}
