using System;
using System.Collections.Generic;

namespace NTMiner.OverClock {
    public class OverClockDatasResponse : ResponseBase {
        public OverClockDatasResponse() {
            this.Data = new List<OverClockData>();
        }

        public static OverClockDatasResponse Ok(Guid messageId, List<OverClockData> data) {
            return new OverClockDatasResponse {
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
