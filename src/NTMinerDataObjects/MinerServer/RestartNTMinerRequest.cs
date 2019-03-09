using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class RestartNTMinerRequest : RequestBase, ISignatureRequest {
        public string ClientIp { get; set; }
        public Guid WorkId { get; set; }
        public string LoginName { get; set; }
        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        public string GetSign(string password) {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(MessageId)).Append(MessageId)
                .Append(nameof(LoginName)).Append(LoginName)
                .Append(nameof(ClientIp)).Append(ClientIp)
                .Append(nameof(WorkId)).Append(WorkId)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong())
                .Append(nameof(UserData.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }
    }
}
