using NTMiner.Core;
using System.Collections.Generic;

namespace NTMiner.MinerServer {
    public class KernelOutputKeywordsResponse : ResponseBase {
        public KernelOutputKeywordsResponse() {
            Timestamp = 0;
        }

        public static KernelOutputKeywordsResponse Ok(List<KernelOutputKeywordData> data, ulong timestamp) {
            return new KernelOutputKeywordsResponse {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data,
                Timestamp = timestamp
            };
        }

        public List<KernelOutputKeywordData> Data { get; set; }
        public ulong Timestamp { get; set; }
    }
}
