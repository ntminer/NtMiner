using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class GetCalcConfigsResponse : ResponseBase {
        public static new GetCalcConfigsResponse ServerError(Guid messageId, string erroMessage) {
            var serverError = ResponseBase.ServerError(messageId, erroMessage);
            return new GetCalcConfigsResponse {
                StateCode = serverError.StateCode,
                ReasonPhrase = serverError.ReasonPhrase,
                Description = serverError.Description,
                MessageId = serverError.MessageId
            };
        }

        public static new GetCalcConfigsResponse ClientError(Guid messageId, string description) {
            var response = ResponseBase.ClientError(messageId, description);
            return new GetCalcConfigsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public GetCalcConfigsResponse() {
            this.Data = new List<CalcConfigData>();
        }

        public GetCalcConfigsResponse(List<CalcConfigData> data) {
            this.Data = data;
        }

        [DataMember]
        public List<CalcConfigData> Data { get; set; }
    }
}
