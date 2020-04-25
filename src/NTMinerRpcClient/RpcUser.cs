using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace NTMiner {
    public class RpcUser : ISignUser {
        public static readonly RpcUser Empty = new RpcUser(string.Empty, string.Empty);

        public static Dictionary<string, string> GetSignData(string loginName, string passwordSha1, object data = null) {
            var timestamp = Timestamp.GetTimestamp(DateTime.Now);
            return new Dictionary<string, string> {
                    {"loginName", loginName },
                    {"sign", CalcSign(loginName, passwordSha1, timestamp, data) },
                    {"timestamp", timestamp.ToString() }
                };
        }

        public static string CalcSign(string loginName, string passwordSha1, long timestamp, object data = null) {
            StringBuilder sb;
            if (data == null || !(data is ISignableData signableData)) {
                sb = new StringBuilder();
            }
            else {
                sb = signableData.GetSignData();
            }
            sb.Append("LoginName").Append(loginName).Append("Password").Append(passwordSha1).Append("Timestamp").Append(timestamp);
            return HashUtil.Sha1(sb.ToString());
        }

        public RpcUser(LoginedUser loginedUser, string passwordSha1) {
            this.LoginedUser = loginedUser;
            this.LoginName = loginedUser.LoginName;
            this.Password = passwordSha1;
        }

        public RpcUser(string loginName, string passwordSha1) {
            this.LoginName = loginName;
            this.Password = passwordSha1;
            this.LoginedUser = LoginedUser.Empty;
        }

        public LoginedUser LoginedUser { get; private set; }

        public string LoginName { get; private set; }

        public string Password {
            get { return GetPassword(); }
            private set {
                SetPassword(value);
            }
        }

        private IntPtr ptr = IntPtr.Zero;
        private void SetPassword(string password) {
            if (string.IsNullOrEmpty(password)) {
                return;
            }
            using (SecureString secureString = new SecureString()) {
                foreach (var c in password.ToCharArray()) {
                    secureString.AppendChar(c);
                }
                if (ptr != IntPtr.Zero) {
                    Marshal.ZeroFreeBSTR(ptr);
                }
                ptr = Marshal.SecureStringToBSTR(secureString);
            }
        }

        private string GetPassword() {
            if (ptr == IntPtr.Zero) {
                return string.Empty;
            }
            return Marshal.PtrToStringBSTR(ptr);
        }

        public void Logout() {
            this.LoginName = string.Empty;
            this.LoginedUser = LoginedUser.Empty;
            if (ptr != IntPtr.Zero) {
                Marshal.ZeroFreeBSTR(ptr);
            }
        }

        /// <summary>
        /// 对给定的数据和内部附加的时间戳进行签名，传入的签名数据可以为null，此时相当于只签名内部附加的时间戳。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetSignData(object data = null) {
            var timestamp = Timestamp.GetTimestamp(DateTime.Now);
            return new Dictionary<string, string> {
                    {"loginName", this.LoginName },
                    {"sign", CalcSign(this.LoginName, this.Password, timestamp, data) },
                    {"timestamp", timestamp.ToString() }
                };
        }
    }
}
