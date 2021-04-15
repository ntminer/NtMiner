using System.Collections.Generic;

namespace NTMiner.ServerNode {
    public class TopNRemoteIpsResponse : ResponseBase {
        public TopNRemoteIpsResponse() {
            this.Data = new List<RemoteIpEntryDto>();
        }

        public static TopNRemoteIpsResponse Ok(List<RemoteIpEntryDto> data, int total) {
            return new TopNRemoteIpsResponse() {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data,
                Total = total
            };
        }

        public List<RemoteIpEntryDto> Data { get; set; }
        public int Total { get; set; }
    }
}
