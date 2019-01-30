using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTMiner.ServiceContracts.ControlCenter.DataObjects {
    [DataContract]
    public class GetWalletsResponse : ResponseBase {
        public GetWalletsResponse() {
            this.Data = new List<WalletData>();
        }

        public static GetWalletsResponse Ok(Guid messageId, List<WalletData> data) {
            return new GetWalletsResponse() {
                MessageId = messageId,
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data
            };
        }

        [DataMember]
        public List<WalletData> Data { get; set; }
    }
}
