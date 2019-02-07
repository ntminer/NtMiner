using System;
using System.Text;

namespace NTMiner.MinerServer {
    public class QueryClientsRequest : RequestBase, ISignatureRequest {
        public string LoginName { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public Guid? MineWorkId { get; set; }
        public string MinerIp { get; set; }
        public string MinerName { get; set; }
        public MineStatus MineState { get; set; }
        public string MainCoin { get; set; }
        public string MainCoinPool { get; set; }
        public string MainCoinWallet { get; set; }
        public string DualCoin { get; set; }
        public string DualCoinPool { get; set; }
        public string DualCoinWallet { get; set; }
        public string Version { get; set; }
        public string Kernel { get; set; }
        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        public string GetSign(string password) {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(MessageId)).Append(MessageId)
                .Append(nameof(LoginName)).Append(LoginName)
                .Append(nameof(PageIndex)).Append(PageIndex)
                .Append(nameof(PageSize)).Append(PageSize)
                .Append(nameof(MineWorkId)).Append(MineWorkId)
                .Append(nameof(MinerIp)).Append(MinerIp)
                .Append(nameof(MinerName)).Append(MinerName)
                .Append(nameof(MineState)).Append(MineState)
                .Append(nameof(MainCoin)).Append(MainCoin)
                .Append(nameof(MainCoinPool)).Append(MainCoinPool)
                .Append(nameof(MainCoinWallet)).Append(MainCoinWallet)
                .Append(nameof(DualCoin)).Append(DualCoin)
                .Append(nameof(DualCoinPool)).Append(DualCoinPool)
                .Append(nameof(DualCoinWallet)).Append(DualCoinWallet)
                .Append(nameof(Version)).Append(Version)
                .Append(nameof(Kernel)).Append(Kernel)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong())
                .Append(nameof(UserData.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }
    }
}
