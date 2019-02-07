using System;
using System.Collections.Generic;

namespace NTMiner {
    public class GetCoinSnapshotsResponse : ResponseBase {
        public GetCoinSnapshotsResponse() {
            this.Data = new List<CoinSnapshotData>();
        }

        public static GetCoinSnapshotsResponse Ok(Guid messageId, List<CoinSnapshotData> data, int totalMiningCount, int totalOnlineCount) {
            return new GetCoinSnapshotsResponse() {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data,
                TotalMiningCount = totalMiningCount,
                TotalOnlineCount = totalOnlineCount
            };
        }

        public List<CoinSnapshotData> Data { get; set; }

        public int TotalMiningCount { get; set; }

        public int TotalOnlineCount { get; set; }
    }
}
