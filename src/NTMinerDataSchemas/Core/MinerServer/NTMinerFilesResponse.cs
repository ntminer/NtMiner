using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer {
    public class NTMinerFilesResponse : ResponseBase {
        public NTMinerFilesResponse() {
            this.Data = new List<NTMinerFileData>();
        }

        public static NTMinerFilesResponse Ok(List<NTMinerFileData> data, DateTime timestamp) {
            return new NTMinerFilesResponse() {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data,
                Timestamp = timestamp
            };
        }

        public List<NTMinerFileData> Data { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
