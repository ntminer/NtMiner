using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class GetWalletsResponse : ResponseBase {
        public GetWalletsResponse() {
            this.Data = new List<WalletData>();
        }

        public GetWalletsResponse(List<WalletData> data) {
            this.Data = data;
        }

        [DataMember]
        public List<WalletData> Data { get; set; }
    }
}
