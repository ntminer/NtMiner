using NTMiner.User;
using System;

namespace NTMiner.Core.Impl {
    public abstract class AbstractSession : ISession {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="wsSessionID"></param>
        public AbstractSession(IUser user, WsUserName userName, string wsSessionID) {
            this.ClientId = userName.ClientId;
            this.ClientVersion = Version.Parse(userName.ClientVersion);// 因为前面的流程已经过验证所以可以直接Parse
            this.LoginName = user.LoginName;
            this.ActiveOn = DateTime.Now;
            this.WsSessionId = wsSessionID;
        }

        public Guid ClientId { get; private set; }

        public Version ClientVersion { get; private set; }

        public string LoginName { get; private set; }

        public DateTime ActiveOn { get; private set; }

        public string WsSessionId { get; private set; }

        public void Active() {
            this.ActiveOn = DateTime.Now;
        }
    }
}
