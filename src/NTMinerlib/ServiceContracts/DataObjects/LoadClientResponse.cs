using System;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class LoadClientResponse : ResponseBase {
        public static new LoadClientResponse Forbidden(Guid messageId, string description = "无权访问") {
            var response = ResponseBase.Forbidden(messageId, description);
            return new LoadClientResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public static new LoadClientResponse Expired(Guid messageId) {
            var response = ResponseBase.Expired(messageId);
            return new LoadClientResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public static new LoadClientResponse InvalidInput(Guid messageId, string description) {
            var response = ResponseBase.InvalidInput(messageId, description);
            return new LoadClientResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public static new LoadClientResponse ServerError(Guid messageId, string description) {
            var response = ResponseBase.ServerError(messageId, description);
            return new LoadClientResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public static new LoadClientResponse ClientError(Guid messageId, string description) {
            var response = ResponseBase.ClientError(messageId, description);
            return new LoadClientResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        [DataMember]
        public ClientData Data { get; set; }
    }
}
