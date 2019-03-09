using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.MinerServer {
    public class GetPoolsResponse : ResponseBase {
        public GetPoolsResponse() {
            this.Data = new List<PoolData>();
        }

        public static GetPoolsResponse Ok(Guid messageId, List<PoolData> data) {
            return new GetPoolsResponse() {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        public List<PoolData> Data { get; set; }
    }
}
