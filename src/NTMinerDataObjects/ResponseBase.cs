using System;

namespace NTMiner {
    public class ResponseBase {
        public static ResponseBase Forbidden(Guid messageId, string description = "无权访问") {
            return Forbidden<ResponseBase>(messageId, description);
        }

        public static ResponseBase Expired(Guid messageId) {
            return Expired<ResponseBase>(messageId);
        }

        public static ResponseBase InvalidInput(Guid messageId, string description = "输入不正确") {
            return InvalidInput<ResponseBase>(messageId, description);
        }

        public static ResponseBase Ok(Guid messageId, string message = null) {
            return new ResponseBase() {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = message ?? "成功"
            };
        }

        public static ResponseBase ServerError(Guid messageId, string description) {
            return ServerError<ResponseBase>(messageId, description);
        }

        public static ResponseBase ClientError(Guid messageId, string description) {
            return ClientError<ResponseBase>(messageId, description);
        }

        public static T NotExist<T>(Guid messageId, string description = "访问的对象不存在") where T : ResponseBase, new() {
            return new T {
                MessageId = messageId,
                StateCode = 404,
                ReasonPhrase = "NotExist",
                Description = description
            };
        }

        public static T Forbidden<T>(Guid messageId, string description = "无权访问") where T : ResponseBase, new() {
            return new T {
                MessageId = messageId,
                StateCode = 403,
                ReasonPhrase = "Forbidden",
                Description = description
            };
        }

        public static T Expired<T>(Guid messageId) where T : ResponseBase, new() {
            return new T {
                MessageId = messageId,
                StateCode = 490,
                ReasonPhrase = "Expired",
                Description = $"远端和本地时间不一致，时差超过{Timestamp.DesyncSeconds}秒"
            };
        }

        public static T InvalidInput<T>(Guid messageId, string description = "输入不正确") where T : ResponseBase, new() {
            return new T {
                MessageId = messageId,
                StateCode = 491,
                ReasonPhrase = "InvalidInput",
                Description = description
            };
        }

        public static T ServerError<T>(Guid messageId, string description) where T : ResponseBase, new() {
            return new T {
                MessageId = messageId,
                StateCode = 500,
                ReasonPhrase = "ServerError",
                Description = description
            };
        }

        public static T ClientError<T>(Guid messageId, string description) where T : ResponseBase, new() {
            return new T {
                MessageId = messageId,
                StateCode = 400,
                ReasonPhrase = "ClientError",
                Description = description
            };
        }

        public ResponseBase() {
        }

        public Guid MessageId { get; set; }

        public int StateCode { get; set; }

        public string ReasonPhrase { get; set; }

        public string Description { get; set; }
    }
}
