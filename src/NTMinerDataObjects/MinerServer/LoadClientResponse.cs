using System;

namespace NTMiner.MinerServer {
    public class LoadClientResponse : ResponseBase {
        public LoadClientResponse() { }
        public static LoadClientResponse Ok(Guid messageId, ClientData data) {
            return new LoadClientResponse() {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        public ClientData Data { get; set; }
    }
}
