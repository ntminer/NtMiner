using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class LoadClientsResponse : ResponseBase {
        public LoadClientsResponse() {
            this.Data = new List<ClientData>();
        }

        public LoadClientsResponse(List<ClientData> data) {
            this.Data = data;
        }

        [DataMember]
        public List<ClientData> Data { get; set; }
    }
}
