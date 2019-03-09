using NTMiner.Profile;
using System;

namespace NTMiner.MinerServer {
    public class CoinProfileResponse : ResponseBase {
        public CoinProfileResponse() {
        }

        public static CoinProfileResponse Ok(Guid messageId, CoinProfileData data) {
            return new CoinProfileResponse {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        public CoinProfileData Data { get; set; }
    }
}