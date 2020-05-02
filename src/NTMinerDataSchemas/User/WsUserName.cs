using System;

namespace NTMiner.User {
    public class WsUserName : IWsUserName {
        public WsUserName() { }

        public NTMinerAppType ClientType { get; set; }
        /// <summary>
        /// 为服务端提供一个根据客户端的版本号向后兼容的机会
        /// </summary>
        public string ClientVersion { get; set; }
        public Guid ClientId { get; set; }
        public string UserId { get; set; }
        /// <summary>
        /// 表示是否支持收发二进制格式的WsMessage。
        /// </summary>
        public bool IsBinarySupported { get; set; }

        public bool IsValid() {
            return !string.IsNullOrEmpty(UserId) && ClientId != Guid.Empty && Version.TryParse(ClientVersion, out _);
        }
    }
}
