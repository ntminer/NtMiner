using System.Collections.Generic;

namespace NTMiner.Core.MinerServer {
    public class KernelOutputKeywordsResponse : ResponseBase {
        public KernelOutputKeywordsResponse() {
            Timestamp = 0;
        }

        public static KernelOutputKeywordsResponse Ok(List<KernelOutputKeywordData> data, long timestamp) {
            return new KernelOutputKeywordsResponse {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data,
                Timestamp = timestamp
            };
        }

        public List<KernelOutputKeywordData> Data { get; set; }
        public long Timestamp { get; set; }
    }
}
