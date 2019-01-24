using System;
using System.Runtime.Serialization;
using System.Text;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
    public class QueryClientsRequest : RequestBase {
        [DataMember]
        public string LoginName { get; set; }
        [DataMember]
        public int PageIndex { get; set; }
        [DataMember]
        public int PageSize { get; set; }
        [DataMember]
        public Guid? MineWorkId { get; set; }
        [DataMember]
        public string MinerIp { get; set; }
        [DataMember]
        public string MinerName { get; set; }
        [DataMember]
        public MineStatus MineState { get; set; }
        [DataMember]
        public string MainCoin { get; set; }
        [DataMember]
        public string MainCoinPool { get; set; }
        [DataMember]
        public string MainCoinWallet { get; set; }
        [DataMember]
        public string DualCoin { get; set; }
        [DataMember]
        public string DualCoinPool { get; set; }
        [DataMember]
        public string DualCoinWallet { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string Kernel { get; set; }
        [DataMember]
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
                .Append(nameof(IUser.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }
    }
}
