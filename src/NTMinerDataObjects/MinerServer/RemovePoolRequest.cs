using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class RemovePoolRequest : RequestBase, ISignatureRequest {
        public RemovePoolRequest() { }
        public string LoginName { get; set; }
        public Guid PoolId { get; set; }
        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        public string GetSign(string password) {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(MessageId)).Append(MessageId)
                .Append(nameof(LoginName)).Append(LoginName)
                .Append(nameof(PoolId)).Append(PoolId)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong())
                .Append(nameof(UserData.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }
    }
}
