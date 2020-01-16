using System;

namespace NTMiner.User {
    public class RpcUser {
        public static readonly RpcUser Empty = new RpcUser(string.Empty, string.Empty);

        public RpcUser(string loginName, string passwordSha1) {
            this.LoginName = loginName;
            this.PasswordSha1 = passwordSha1;
        }

        public string LoginName { get; private set; }

        public string PasswordSha1 { get; private set; }

        public string GetRemotePassword(Guid clientId) {
            return HashUtil.Sha1($"{HashUtil.Sha1(PasswordSha1)}{clientId.ToString()}");
        }
    }
}
