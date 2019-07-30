using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class WrapperRequest<T> : RequestBase, IGetSignData where T : IGetSignData {
        public WrapperRequest() { }

        public string ObjectId { get; set; }

        public Guid ClientId { get; set; }

        public string ClientIp { get; set; }

        [ManualSign]
        public T InnerRequest { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = this.BuildSign();
            sb.Append(nameof(InnerRequest)).Append(InnerRequest.GetSignData().ToString());
            return sb;
        }
    }
}
