using System;
using System.Collections.Generic;

namespace NTMiner {
    public class LoadClientsResponse : ResponseBase {
        public LoadClientsResponse() {
            this.Data = new List<ClientData>();
        }

        public static LoadClientsResponse Ok(Guid messageId, List<ClientData> data) {
            return new LoadClientsResponse() {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        public List<ClientData> Data { get; set; }
    }
}
