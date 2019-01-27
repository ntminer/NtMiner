using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class GetMinerGroupsResponse : ResponseBase {
        public GetMinerGroupsResponse() {
            this.Data = new List<MinerGroupData>();
        }

        public GetMinerGroupsResponse(List<MinerGroupData> data) {
            this.Data = data;
        }

        [DataMember]
        public List<MinerGroupData> Data { get; set; }
    }
}
