using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.ControlCenter.DataObjects {
    [DataContract]
    public class GetMinerGroupsResponse : ResponseBase {
        public GetMinerGroupsResponse() {
            this.Data = new List<MinerGroupData>();
        }

        public static GetMinerGroupsResponse Ok(Guid messageId, List<MinerGroupData> data) {
            return new GetMinerGroupsResponse {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        [DataMember]
        public List<MinerGroupData> Data { get; set; }
    }
}
