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
            this.ClientVersion = userName.ClientVersion;
            this.LoginName = user.LoginName;
            this.ActiveOn = DateTime.Now;
            this.WsSessionId = wsSessionID;
        }

        public Guid ClientId { get; private set; }

        public string ClientVersion { get; private set; }

        public string LoginName { get; private set; }

        public DateTime ActiveOn { get; private set; }

        public string WsSessionId { get; private set; }

        public void Active() {
            this.ActiveOn = DateTime.Now;
        }
    }
}
