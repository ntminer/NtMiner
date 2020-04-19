using System.Collections.Generic;

namespace NTMiner.Core.MinerServer {
    public class QueryClientsResponse : ResponseBase {
        public QueryClientsResponse() {
            this.Data = new List<ClientData>();
            this.LatestSnapshots = new List<CoinSnapshotData>();
        }

        public static QueryClientsResponse Ok(List<ClientData> data, int total, List<CoinSnapshotData> latestSnapshots, int totalMiningCount, int totalOnlineCount) {
            return new QueryClientsResponse() {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data,
                Total = total,
                LatestSnapshots = latestSnapshots,
                TotalMiningCount = totalMiningCount,
                TotalOnlineCount = totalOnlineCount
            };
        }

        public List<ClientData> Data { get; set; }

        public List<CoinSnapshotData> LatestSnapshots { get; set; }

        public int Total { get; set; }

        public int TotalMiningCount { get; set; }

        public int TotalOnlineCount { get; set; }
    }
}
