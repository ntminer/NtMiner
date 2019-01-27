using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class QueryClientsResponse : ResponseBase {
        public QueryClientsResponse() {
            this.Data = new List<ClientData>();
        }

        public QueryClientsResponse(List<ClientData> data) {
            this.Data = data;
        }

        [DataMember]
        public List<ClientData> Data { get; set; }

        [DataMember]
        public int Total { get; set; }
    }
}
