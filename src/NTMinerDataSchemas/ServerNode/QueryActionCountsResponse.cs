using System.Collections.Generic;

namespace NTMiner.ServerNode {
    public class QueryActionCountsResponse : ResponseBase {
        public QueryActionCountsResponse() {
            this.Data = new List<ActionCountData>();
        }

        public static QueryActionCountsResponse Ok(List<ActionCountData> data, int total) {
            return new QueryActionCountsResponse() {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data,
                Total = total
            };
        }

        public List<ActionCountData> Data { get; set; }
        public int Total { get; set; }
    }
}
