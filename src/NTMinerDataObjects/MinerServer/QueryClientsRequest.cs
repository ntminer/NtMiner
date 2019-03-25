using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class QueryClientsRequest : RequestBase, ISignatureRequest {
        public QueryClientsRequest() { }
        public string LoginName { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? WorkId { get; set; }
        public string MinerIp { get; set; }
        public string MinerName { get; set; }
        public MineStatus MineState { get; set; }
        public string Coin { get; set; }
        public string Pool { get; set; }
        public string Wallet { get; set; }
        public string Version { get; set; }
        public string Kernel { get; set; }
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
                .Append(nameof(PageIndex)).Append(PageIndex)
                .Append(nameof(PageSize)).Append(PageSize)
                .Append(nameof(GroupId)).Append(GroupId)
                .Append(nameof(WorkId)).Append(WorkId)
                .Append(nameof(MinerIp)).Append(MinerIp)
                .Append(nameof(MinerName)).Append(MinerName)
                .Append(nameof(MineState)).Append(MineState)
                .Append(nameof(Coin)).Append(Coin)
                .Append(nameof(Pool)).Append(Pool)
                .Append(nameof(Wallet)).Append(Wallet)
                .Append(nameof(Version)).Append(Version)
                .Append(nameof(Kernel)).Append(Kernel)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            return sb;
        }
    }
}
