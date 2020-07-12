using System.Collections.Generic;

namespace NTMiner.Gpus {
    public class QueryGpuNameCountsResponse : ResponseBase {
        public QueryGpuNameCountsResponse() {
            this.Data = new List<GpuNameCount>();
        }

        public static QueryGpuNameCountsResponse Ok(List<GpuNameCount> data, int total) {
            return new QueryGpuNameCountsResponse() {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data,
                Total = total
            };
        }

        public List<GpuNameCount> Data { get; set; }
        public int Total { get; set; }
    }
}
