using System;

namespace NTMiner.Core {
    public class GetConsoleOutLinesRequest {
        public GetConsoleOutLinesRequest() { }

        public string LoginName { get; set; }
        public Guid ClientId { get; set; }
        public long AfterTime { get; set; }
    }
}
