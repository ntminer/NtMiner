using System;

namespace NTMiner.Core.MinerServer {
    public class NTMinerFilesRequest : IRequest {
        public NTMinerFilesRequest() { }

        public DateTime Timestamp { get; set; }
    }
}
