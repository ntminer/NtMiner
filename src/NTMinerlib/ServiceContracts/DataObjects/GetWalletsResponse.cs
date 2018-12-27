using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class GetWalletsResponse : ResponseBase {
        public static new GetWalletsResponse ServerError(Guid messageId, string erroMessage) {
            var serverError = ResponseBase.ServerError(messageId, erroMessage);
            return new GetWalletsResponse {
                StateCode = serverError.StateCode,
                ReasonPhrase = serverError.ReasonPhrase,
                Description = serverError.Description,
                MessageId = serverError.MessageId
            };
        }

        public static new GetWalletsResponse ClientError(Guid messageId, string description) {
            var response = ResponseBase.ClientError(messageId, description);
            return new GetWalletsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public GetWalletsResponse() {
            this.Data = new List<WalletData>();
        }

        public GetWalletsResponse(List<WalletData> data) {
            this.Data = data;
        }

        [DataMember]
        public List<WalletData> Data { get; set; }
    }
}
