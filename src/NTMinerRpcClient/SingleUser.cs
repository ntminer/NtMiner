namespace NTMiner {
    public class SingleUser {
        public static string LoginName { get; set; }

        private static string s_passwordSha1;
        public static string PasswordSha1 {
            get { return s_passwordSha1; }
            set {
                s_passwordSha1 = value;
                PasswordSha1Sha1 = HashUtil.Sha1(value);
            }
        }

        public static string PasswordSha1Sha1 {
            get; private set;
        }
    }
}
