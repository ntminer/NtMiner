using NTMiner.Profile;
using System;

namespace NTMiner.MinerServer {
    public class MinerProfileResponse : ResponseBase {
        public MinerProfileResponse() {
        }

        public static MinerProfileResponse Ok(Guid messageId, MinerProfileData data) {
            return new MinerProfileResponse {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        public MinerProfileData Data { get; set; }
    }
}