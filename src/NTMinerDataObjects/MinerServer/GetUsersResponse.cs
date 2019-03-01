using System;
using System.Collections.Generic;

namespace NTMiner.MinerServer {
    public class GetUsersResponse : ResponseBase {
        public GetUsersResponse() {
            this.Data = new List<UserData>();
        }

        public static GetUsersResponse Ok(Guid messageId, List<UserData> data) {
            return new GetUsersResponse {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        public List<UserData> Data { get; set; }
    }
}
