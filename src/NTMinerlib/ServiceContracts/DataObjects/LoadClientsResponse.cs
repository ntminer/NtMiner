using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class LoadClientsResponse : ResponseBase {
        public static new LoadClientsResponse Forbidden(Guid messageId, string description = "无权访问") {
            var response = ResponseBase.Forbidden(messageId, description);
            return new LoadClientsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public static new LoadClientsResponse Expired(Guid messageId) {
            var response = ResponseBase.Expired(messageId);
            return new LoadClientsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public static new LoadClientsResponse InvalidInput(Guid messageId, string description) {
            var response = ResponseBase.InvalidInput(messageId, description);
            return new LoadClientsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public static new LoadClientsResponse ServerError(Guid messageId, string description) {
            var response = ResponseBase.ServerError(messageId, description);
            return new LoadClientsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public static new LoadClientsResponse ClientError(Guid messageId, string description) {
            var response = ResponseBase.ClientError(messageId, description);
            return new LoadClientsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

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
