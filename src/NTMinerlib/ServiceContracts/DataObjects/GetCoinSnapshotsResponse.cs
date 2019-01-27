using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class GetCoinSnapshotsResponse : ResponseBase {
        public GetCoinSnapshotsResponse() {
            this.Data = new List<CoinSnapshotData>();
        }

        public GetCoinSnapshotsResponse(List<CoinSnapshotData> data) {
            this.Data = data;
        }

        [DataMember]
        public List<CoinSnapshotData> Data { get; set; }

        [DataMember]
        public int TotalMiningCount { get; set; }

        [DataMember]
        public int TotalOnlineCount { get; set; }
    }
}
