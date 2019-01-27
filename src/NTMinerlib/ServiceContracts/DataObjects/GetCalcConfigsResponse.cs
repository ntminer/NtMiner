using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class GetCalcConfigsResponse : ResponseBase {
        public GetCalcConfigsResponse() {
            this.Data = new List<CalcConfigData>();
        }

        public GetCalcConfigsResponse(List<CalcConfigData> data) {
            this.Data = data;
        }

        [DataMember]
        public List<CalcConfigData> Data { get; set; }
    }
}
