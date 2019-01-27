using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    public class GetAppSettingsResponse : ResponseBase {
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
