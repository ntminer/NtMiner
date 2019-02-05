using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner {
    [DataContract]
    public class QueryClientsResponse : ResponseBase {
        public QueryClientsResponse() {
            this.Data = new List<ClientData>();
        }

        public static QueryClientsResponse Ok(Guid messageId, List<ClientData> data, int total) {
            return new QueryClientsResponse() {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data,
                Total = total
            };
        }

        [DataMember]
        public List<ClientData> Data { get; set; }

        [DataMember]
        public int Total { get; set; }
    }
}
