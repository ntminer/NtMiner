using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class QueryClientsResponse : ResponseBase {
        public static new QueryClientsResponse Forbidden(Guid messageId, string description = "无权访问") {
            var response = ResponseBase.Forbidden(messageId, description);
            return new QueryClientsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public static new QueryClientsResponse Expired(Guid messageId) {
            var response = ResponseBase.Expired(messageId);
            return new QueryClientsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public static new QueryClientsResponse InvalidInput(Guid messageId, string description) {
            var response = ResponseBase.InvalidInput(messageId, description);
            return new QueryClientsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public static new QueryClientsResponse ServerError(Guid messageId, string description) {
            var response = ResponseBase.ServerError(messageId, description);
            return new QueryClientsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public static new QueryClientsResponse ClientError(Guid messageId, string description) {
            var response = ResponseBase.ClientError(messageId, description);
            return new QueryClientsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

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
