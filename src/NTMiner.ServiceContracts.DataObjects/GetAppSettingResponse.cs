using System;
using System.Runtime.Serialization;

namespace NTMiner {
    public class GetAppSettingResponse : ResponseBase {
        public GetAppSettingResponse() {
        }

        public static GetAppSettingResponse Ok(Guid messageId, AppSettingData data) {
            return new GetAppSettingResponse() {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        [DataMember]
        public AppSettingData Data { get; set; }
    }
}
