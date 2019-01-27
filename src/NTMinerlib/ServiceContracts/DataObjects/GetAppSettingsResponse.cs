using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    public class GetAppSettingsResponse : ResponseBase {
        public static new GetAppSettingsResponse ServerError(Guid messageId, string erroMessage) {
            var serverError = ResponseBase.ServerError(messageId, erroMessage);
            return new GetAppSettingsResponse {
                StateCode = serverError.StateCode,
                ReasonPhrase = serverError.ReasonPhrase,
                Description = serverError.Description,
                MessageId = serverError.MessageId
            };
        }

        public static new GetAppSettingsResponse ClientError(Guid messageId, string description) {
            var response = ResponseBase.ClientError(messageId, description);
            return new GetAppSettingsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public GetAppSettingsResponse() {
            this.Data = new List<AppSettingData>();
        }

        public GetAppSettingsResponse(List<AppSettingData> data) {
            this.Data = data;
        }

        [DataMember]
        public List<AppSettingData> Data { get; set; }
    }
}
