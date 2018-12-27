using System;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class ResponseBase : object {
        public static ResponseBase Forbidden(Guid messageId, string description = "无权访问") {
            return new ResponseBase() {
                MessageId = messageId,
                StateCode = 403,
                ReasonPhrase = "Forbidden",
                Description = description
            };
        }

        public static ResponseBase Expired(Guid messageId) {
            return new ResponseBase() {
                MessageId = messageId,
                StateCode = 490,
                ReasonPhrase = "Expired",
                Description = "过期"
            };
        }

        public static ResponseBase InvalidInput(Guid messageId, string description = "输入不正确") {
            return new ResponseBase() {
                MessageId = messageId,
                StateCode = 491,
                ReasonPhrase = "InvalidInput",
                Description = description
            };
        }

        public static ResponseBase Ok(Guid messageId) {
            return new ResponseBase() {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功"
            };
        }

        public static ResponseBase ServerError(Guid messageId, string description) {
            return new ResponseBase {
                MessageId = messageId,
                StateCode = 500,
                ReasonPhrase = "ServerError",
                Description = description
            };
        }

        public static ResponseBase ClientError(Guid messageId, string description) {
            return new ResponseBase {
                MessageId = messageId,
                StateCode = 400,
                ReasonPhrase = "ClientError",
                Description = description
            };
        }

        public ResponseBase() {
        }

        public bool IsSuccess() {
            return this.StateCode == 200;
        }

        [DataMember]
        public Guid MessageId { get; set; }

        [DataMember]
        public int StateCode { get; set; }

        [DataMember]
        public string ReasonPhrase { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}
