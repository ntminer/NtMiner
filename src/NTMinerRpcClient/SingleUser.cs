using System;

namespace NTMiner {
    public class SingleUser {
        public static string LoginName { get; set; }

        private static string s_passwordSha1;
        public static string PasswordSha1 {
            get { return s_passwordSha1; }
        }

        public static void SetPasswordSha1(string passwordSha1) {
            s_passwordSha1 = passwordSha1;
        }

        public static string GetRemotePassword(Guid clientId) {
            return HashUtil.Sha1($"{HashUtil.Sha1(PasswordSha1)}{clientId}");
        }
    }
}
