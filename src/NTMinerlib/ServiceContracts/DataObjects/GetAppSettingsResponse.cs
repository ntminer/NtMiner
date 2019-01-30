using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
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

        [DataMember]
        public List<AppSettingData> Data { get; set; }
    }
}
