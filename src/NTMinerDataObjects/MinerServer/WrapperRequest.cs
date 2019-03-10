using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class WrapperRequest<T> : RequestBase, ISignatureRequest where T : ISignatureRequest {
        public WrapperRequest() { }

        public string LoginName { get; set; }

        public Guid ClientId { get; set; }

        public string ClientIp { get; set; }

        public T InnerRequest { get; set; }

        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        public string GetSign(string password) {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(MessageId)).Append(MessageId)
                .Append(nameof(LoginName)).Append(LoginName)
                .Append(nameof(ClientId)).Append(ClientId)
                .Append(nameof(ClientIp)).Append(ClientIp)
                .Append(nameof(InnerRequest)).Append(nameof(Sign)).Append(InnerRequest.Sign)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong())
                .Append(nameof(UserData.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }
    }
}
