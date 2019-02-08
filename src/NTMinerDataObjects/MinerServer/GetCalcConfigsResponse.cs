using System;
using System.Collections.Generic;

namespace NTMiner.MinerServer {
    public class GetCalcConfigsResponse : ResponseBase {
        public GetCalcConfigsResponse() {
            this.Data = new List<CalcConfigData>();
        }

        public static GetCalcConfigsResponse Ok(Guid messageId, List<CalcConfigData> data) {
            return new GetCalcConfigsResponse() {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        public List<CalcConfigData> Data { get; set; }
    }
}
