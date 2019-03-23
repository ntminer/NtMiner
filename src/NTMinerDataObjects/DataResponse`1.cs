using System;

namespace NTMiner {
    public class DataResponse<T> : ResponseBase {
        public DataResponse() {
        }

        public static DataResponse<T> Ok(Guid messageId, T data) {
            return new DataResponse<T> {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        public T Data { get; set; }
    }
}