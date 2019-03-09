using System;
using System.Collections.Generic;

namespace NTMiner.MinerServer {
    public class GetMineWorksResponse : ResponseBase {
        public GetMineWorksResponse() {
            this.Data = new List<MineWorkData>();
        }

        public static GetMineWorksResponse Ok(Guid messageId, List<MineWorkData> data) {
            return new GetMineWorksResponse {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        public List<MineWorkData> Data { get; set; }
    }
}
