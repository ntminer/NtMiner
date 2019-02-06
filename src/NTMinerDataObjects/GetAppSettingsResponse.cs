using System;
using System.Collections.Generic;

namespace NTMiner {
    public class GetAppSettingsResponse : ResponseBase {
        public GetAppSettingsResponse() {
            this.Data = new List<AppSettingData>();
        }

        public static GetAppSettingsResponse Ok(Guid messageId, List<AppSettingData> data) {
            return new GetAppSettingsResponse() {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        public List<AppSettingData> Data { get; set; }
    }
}
