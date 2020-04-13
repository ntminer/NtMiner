namespace NTMiner {
    public class ResponseBase {
        public static ResponseBase Forbidden(string description = "无权访问") {
            return Forbidden<ResponseBase>(description);
        }

        public static ResponseBase Expired() {
            return Expired<ResponseBase>();
        }

        public static ResponseBase InvalidInput(string description = "输入不正确") {
            return InvalidInput<ResponseBase>(description);
        }

        public static ResponseBase Ok(string description = null) {
            return new ResponseBase() {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = description ?? "成功"
            };
        }

        public static ResponseBase ServerError(string description) {
            return ServerError<ResponseBase>(description);
        }

        public static ResponseBase ClientError(string description) {
            return ClientError<ResponseBase>(description);
        }

        public static ResponseBase NotExist(string description = "访问的对象不存在") {
            return new ResponseBase {
                StateCode = 404,
                ReasonPhrase = "NotExist",
                Description = description
            };
        }

        public static T NotExist<T>(string description = "访问的对象不存在") where T : ResponseBase, new() {
            return new T {
                StateCode = 404,
                ReasonPhrase = "NotExist",
                Description = description
            };
        }

        public static T Forbidden<T>(string description = "无权访问") where T : ResponseBase, new() {
            return new T {
                StateCode = 403,
                ReasonPhrase = "Forbidden",
                Description = description
            };
        }

        public static T Expired<T>() where T : ResponseBase, new() {
            return new T {
                StateCode = 490,
                ReasonPhrase = "Expired",
                Description = $"远端和本地时间不一致，时差超过{Timestamp.DesyncSeconds.ToString()}秒"
            };
        }

        public static T InvalidInput<T>(string description = "输入不正确") where T : ResponseBase, new() {
            return new T {
                StateCode = 491,
                ReasonPhrase = "InvalidInput",
                Description = description
            };
        }

        public static T ServerError<T>(string description) where T : ResponseBase, new() {
            return new T {
                StateCode = 500,
                ReasonPhrase = "ServerError",
                Description = description
            };
        }

        public static T ClientError<T>(string description) where T : ResponseBase, new() {
            return new T {
                StateCode = 400,
                ReasonPhrase = "ClientError",
                Description = description
            };
        }

        public ResponseBase() {
        }

        public int StateCode { get; set; }

        public string ReasonPhrase { get; set; }

        public string Description { get; set; }
    }
}
