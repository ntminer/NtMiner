using System;

namespace NTMiner.User {
    public class WsUserName {
        public WsUserName() { }

        public NTMinerAppType ClientType { get; set; }
        /// <summary>
        /// 为服务端提供一个根据客户端的版本号向后兼容的机会
        /// </summary>
        public string ClientVersion { get; set; }
        public Guid ClientId { get; set; }
        public string UserId { get; set; }
    }
}
