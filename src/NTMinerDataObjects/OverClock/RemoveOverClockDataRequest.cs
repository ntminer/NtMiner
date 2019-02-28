using System;
using System.Text;

namespace NTMiner.OverClock {
    public class RemoveOverClockDataRequest : RequestBase, ISignatureRequest {
        public RemoveOverClockDataRequest() { }
        public string LoginName { get; set; }
        public Guid OverClockDataId { get; set; }
        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        public string GetSign(string password) {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(MessageId)).Append(MessageId)
                .Append(nameof(LoginName)).Append(LoginName)
                .Append(nameof(OverClockDataId)).Append(OverClockDataId)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong())
                .Append(nameof(UserData.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }
    }
}
