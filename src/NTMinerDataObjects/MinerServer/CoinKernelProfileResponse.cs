using NTMiner.Profile;
using System;

namespace NTMiner.MinerServer {
    public class CoinKernelProfileResponse : ResponseBase {
        public CoinKernelProfileResponse() {
        }

        public static CoinKernelProfileResponse Ok(Guid messageId, CoinKernelProfileData data) {
            return new CoinKernelProfileResponse {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        public CoinKernelProfileData Data { get; set; }
    }
}