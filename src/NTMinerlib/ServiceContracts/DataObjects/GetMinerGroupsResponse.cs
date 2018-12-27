using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class GetMinerGroupsResponse : ResponseBase {
        public static new GetMinerGroupsResponse ServerError(Guid messageId, string erroMessage) {
            var serverError = ResponseBase.ServerError(messageId, erroMessage);
            return new GetMinerGroupsResponse {
                StateCode = serverError.StateCode,
                ReasonPhrase = serverError.ReasonPhrase,
                Description = serverError.Description,
                MessageId = serverError.MessageId
            };
        }

        public static new GetMinerGroupsResponse ClientError(Guid messageId, string description) {
            var response = ResponseBase.ClientError(messageId, description);
            return new GetMinerGroupsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

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
