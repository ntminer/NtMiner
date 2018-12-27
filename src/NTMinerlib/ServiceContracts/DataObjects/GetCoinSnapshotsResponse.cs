using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class GetCoinSnapshotsResponse : ResponseBase {
        public static new GetCoinSnapshotsResponse Forbidden(Guid messageId, string description = "无权访问") {
            var response = ResponseBase.Forbidden(messageId, description);
            return new GetCoinSnapshotsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public static new GetCoinSnapshotsResponse Expired(Guid messageId) {
            var response = ResponseBase.Expired(messageId);
            return new GetCoinSnapshotsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public static new GetCoinSnapshotsResponse InvalidInput(Guid messageId, string description) {
            var response = ResponseBase.InvalidInput(messageId, description);
            return new GetCoinSnapshotsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public static new GetCoinSnapshotsResponse ServerError(Guid messageId, string description) {
            var response = ResponseBase.ServerError(messageId, description);
            return new GetCoinSnapshotsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public static new GetCoinSnapshotsResponse ClientError(Guid messageId, string description) {
            var response = ResponseBase.ClientError(messageId, description);
            return new GetCoinSnapshotsResponse {
                StateCode = response.StateCode,
                ReasonPhrase = response.ReasonPhrase,
                Description = response.Description,
                MessageId = response.MessageId
            };
        }

        public GetCoinSnapshotsResponse() {
            this.Data = new List<CoinSnapshotData>();
        }

        public GetCoinSnapshotsResponse(List<CoinSnapshotData> data) {
            this.Data = data;
        }

        [DataMember]
        public List<CoinSnapshotData> Data { get; set; }

        [DataMember]
        public int TotalMiningCount { get; set; }

        [DataMember]
        public int TotalOnlineCount { get; set; }
    }
}
