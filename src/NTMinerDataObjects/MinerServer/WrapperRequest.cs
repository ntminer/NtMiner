using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class WrapperRequest<T> : RequestBase, ISignatureRequest where T : IGetSignData {
        public WrapperRequest() { }

        public string LoginName { get; set; }

        public string ObjectId { get; set; }

        public Guid ClientId { get; set; }

        public string ClientIp { get; set; }

        public T InnerRequest { get; set; }

        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        public string GetSign(string password) {
            StringBuilder sb = GetSignData().Append(nameof(UserData.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(LoginName)).Append(LoginName)
                .Append(nameof(ObjectId)).Append(ObjectId)
                .Append(nameof(ClientId)).Append(ClientId)
                .Append(nameof(ClientIp)).Append(ClientIp)
                .Append(nameof(InnerRequest)).Append(nameof(Sign)).Append(InnerRequest.GetSignData())
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            return sb;
        }
    }
}
