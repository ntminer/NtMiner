using System;
using System.Runtime.Serialization;

namespace NTMiner {
    [DataContract]
    public class LoadClientResponse : ResponseBase {
        public static LoadClientResponse Ok(Guid messageId, ClientData data) {
            return new LoadClientResponse() {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        [DataMember]
        public ClientData Data { get; set; }
    }
}
