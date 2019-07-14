using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class WrapperRequest<T> : RequestBase, IGetSignData where T : IGetSignData {
        public WrapperRequest() { }

        public string ObjectId { get; set; }

        public Guid ClientId { get; set; }

        public string ClientIp { get; set; }

        public T InnerRequest { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(ObjectId)).Append(ObjectId)
              .Append(nameof(ClientId)).Append(ClientId)
              .Append(nameof(ClientIp)).Append(ClientIp)
              .Append(nameof(InnerRequest)).Append(InnerRequest.GetSignData());
            return sb;
        }
    }
}
