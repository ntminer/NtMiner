using System.Collections.Generic;

namespace NTMiner.Core.Gpus {
    public class QueryGpuNamesResponse : ResponseBase {
        public QueryGpuNamesResponse() {
            this.Data = new List<GpuName>();
        }

        public static QueryGpuNamesResponse Ok(List<GpuName> data, int total) {
            return new QueryGpuNamesResponse() {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data,
                Total = total
            };
        }

        public List<GpuName> Data { get; set; }
        public int Total { get; set; }
    }
}
