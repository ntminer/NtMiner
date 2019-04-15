using System.Collections.Generic;

namespace NTMiner.MinerServer {
    public class QueryClientsResponse : ResponseBase {
        public QueryClientsResponse() {
            this.Data = new List<ClientData>();
        }

        public static QueryClientsResponse Ok(List<ClientData> data, int total, int miningCount) {
            return new QueryClientsResponse() {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data,
                Total = total,
                MiningCount = miningCount
            };
        }

        public List<ClientData> Data { get; set; }

        public int Total { get; set; }

        public int MiningCount { get; set; }
    }
}
