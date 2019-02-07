using System;
using System.Collections.Generic;

namespace NTMiner.MinerServer {
    public class GetWalletsResponse : ResponseBase {
        public GetWalletsResponse() {
            this.Data = new List<WalletData>();
        }

        public static GetWalletsResponse Ok(Guid messageId, List<WalletData> data) {
            return new GetWalletsResponse() {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        public List<WalletData> Data { get; set; }
    }
}
