using System;
using System.Text;

namespace NTMiner.Profile {
    public class SetCoinKernelProfilePropertyRequest : RequestBase, ISignatureRequest {
        public SetCoinKernelProfilePropertyRequest() { }
        public string LoginName { get; set; }
        public Guid WorkId { get; set; }
        public Guid CoinKernelId { get; set; }
        public string PropertyName { get; set; }
        public object Value { get; set; }
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
                .Append(nameof(WorkId)).Append(WorkId)
                .Append(nameof(CoinKernelId)).Append(CoinKernelId)
                .Append(nameof(PropertyName)).Append(PropertyName)
                .Append(nameof(Value)).Append(Value)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            return sb;
        }
    }
}
