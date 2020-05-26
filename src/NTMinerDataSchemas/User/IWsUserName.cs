using System;

namespace NTMiner.User {
    public interface IWsUserName {
        /// <summary>
        /// 客户端类型，不同类型的客户端的登录方式不同，所以需要区分客户端类型。
        /// </summary>
        NTMinerAppType ClientType { get; }
        /// <summary>
        /// 为服务端提供一个根据客户端的版本号向后兼容的机会
        /// </summary>
        string ClientVersion { get; }
        /// <summary>
        /// 客户端的标识。
        /// </summary>
        Guid ClientId { get; }
        /// <summary>
        /// 可能是LoginName、Email、Mobile
        /// </summary>
        string UserId { get; }
        /// <summary>
        /// 表示是否支持收发二进制格式的WsMessage。
        /// </summary>
        bool IsBinarySupported { get; }
    }
}
