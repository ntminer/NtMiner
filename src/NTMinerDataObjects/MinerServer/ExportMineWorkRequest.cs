using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class ExportMineWorkRequest : RequestBase, ISignatureRequest {
        public ExportMineWorkRequest() { }
        public string LoginName { get; set; }
        public Guid MineWorkId { get; set; }
        public string LocalJson { get; set; }
        public string ServerJson { get; set; }
        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        public string GetSign(string password) {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(MessageId)).Append(MessageId)
                .Append(nameof(LoginName)).Append(LoginName)
                .Append(nameof(MineWorkId)).Append(MineWorkId)
                .Append(nameof(LocalJson)).Append(LocalJson)
                .Append(nameof(ServerJson)).Append(ServerJson)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong())
                .Append(nameof(UserData.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }
    }
}
