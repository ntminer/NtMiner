using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    public class GetAppSettingResponse : ResponseBase {
        public GetAppSettingResponse() {
        }

        public GetAppSettingResponse(AppSettingData data) {
            this.Data = data;
        }

        [DataMember]
        public AppSettingData Data { get; set; }
    }
}
