using System;
using System.Text;

namespace NTMiner.Profile {
    public class WorkProfileRequest : RequestBase, ISignatureRequest {
        public WorkProfileRequest() { }

        public string LoginName { get; set; }

        public Guid WorkId { get; set; }

        public Guid DataId { get; set; }

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
                .Append(nameof(DataId)).Append(DataId)
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong());

            return sb;
        }
    }
}
