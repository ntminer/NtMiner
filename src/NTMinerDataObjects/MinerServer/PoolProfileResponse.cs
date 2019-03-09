using NTMiner.Profile;
using System;

namespace NTMiner.MinerServer {
    public class PoolProfileResponse : ResponseBase {
        public PoolProfileResponse() {
        }

        public static PoolProfileResponse Ok(Guid messageId, PoolProfileData data) {
            return new PoolProfileResponse {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        public PoolProfileData Data { get; set; }
    }
}