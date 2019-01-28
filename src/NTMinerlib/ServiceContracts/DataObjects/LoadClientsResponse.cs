using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
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

        [DataMember]
        public List<ClientData> Data { get; set; }
    }
}
