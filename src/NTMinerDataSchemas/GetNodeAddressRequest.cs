using System;

namespace NTMiner {
    public class GetNodeAddressRequest {
        public GetNodeAddressRequest() { }

        public Guid ClientId { get; set; }
        public string UserId { get; set; }
    }
}
