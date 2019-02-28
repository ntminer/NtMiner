using System;
using System.Collections.Generic;

namespace NTMiner.OverClock {
    public class GetOverClockDatasResponse : ResponseBase {
        public GetOverClockDatasResponse() {
            this.Data = new List<OverClockData>();
        }

        public static GetOverClockDatasResponse Ok(Guid messageId, List<OverClockData> data) {
            return new GetOverClockDatasResponse {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        public List<OverClockData> Data { get; set; }
    }
}
