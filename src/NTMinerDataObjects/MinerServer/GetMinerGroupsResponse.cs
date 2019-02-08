using System;
using System.Collections.Generic;

namespace NTMiner.MinerServer {
    public class GetMinerGroupsResponse : ResponseBase {
        public GetMinerGroupsResponse() {
            this.Data = new List<MinerGroupData>();
        }

        public static GetMinerGroupsResponse Ok(Guid messageId, List<MinerGroupData> data) {
            return new GetMinerGroupsResponse {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        public List<MinerGroupData> Data { get; set; }
    }
}
