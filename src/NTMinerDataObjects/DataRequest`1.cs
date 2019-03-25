using System;
using System.Text;

namespace NTMiner {
    public class DataRequest<T> : RequestBase, ISignatureRequest {
        public DataRequest() { }
        public string LoginName { get; set; }
        public T Data { get; set; }
        public string Sign { get; set; }

        public void SignIt(string password) {
            this.Sign = this.GetSign(password);
        }

        private string GetData() {
            Type dataType = typeof(T);
            if (dataType.IsValueType) {
                return dataType.ToString();
            }

            if (Data == null) {
                return string.Empty;
            }
            if (typeof(IGetSignData).IsAssignableFrom(dataType)) {
                return ((IGetSignData)Data).GetSignData().ToString();
            }

            return Data.ToString();
        }

        public string GetSign(string password) {
            StringBuilder sb = GetSignData().Append(nameof(UserData.Password)).Append(password);
            return HashUtil.Sha1(sb.ToString());
        }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(LoginName)).Append(LoginName)
                .Append(nameof(Data)).Append(GetData())
                .Append(nameof(Timestamp)).Append(Timestamp.ToUlong());
            return sb;
        }
    }
}
