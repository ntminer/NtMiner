using System;
using System.Collections.Generic;

namespace NTMiner.MinerServer {
    public class GetColumnsShowsResponse : ResponseBase {
        public GetColumnsShowsResponse() {
            this.Data = new List<ColumnsShowData>();
        }

        public static GetColumnsShowsResponse Ok(Guid messageId, List<ColumnsShowData> data) {
            return new GetColumnsShowsResponse {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        public List<ColumnsShowData> Data { get; set; }
    }
}
