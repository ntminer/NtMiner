using System;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    public class GetAppSettingResponse : ResponseBase {
        public static new GetAppSettingResponse ServerError(Guid messageId, string erroMessage) {
            var serverError = ResponseBase.ServerError(messageId, erroMessage);
            return new GetAppSettingResponse {
                StateCode = serverError.StateCode,
                ReasonPhrase = serverError.ReasonPhrase,
                Description = serverError.Description,
                MessageId = serverError.MessageId
            };
        }

        public static new GetAppSettingResponse ClientError(Guid messageId, string description) {
            var response = ResponseBase.ClientError(messageId, description);
            return new GetAppSettingResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public GetAppSettingResponse() {
        }

        public GetAppSettingResponse(AppSettingData data) {
            this.Data = data;
        }

        [DataMember]
        public AppSettingData Data { get; set; }
    }
}
