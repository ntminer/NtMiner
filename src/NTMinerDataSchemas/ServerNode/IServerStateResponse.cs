namespace NTMiner.ServerNode {
    public interface IServerStateResponse {
        /// <summary>
        /// server.json版本
        /// </summary>
        string JsonFileVersion { get; }
        /// <summary>
        /// 可升级到的最新的挖矿端客户端版本
        /// </summary>
        string MinerClientVersion { get; }
        /// <summary>
        /// 服务器当前时间。
        /// 注意：这个字段不用展示在界面，当发现服务器时间不对时才展示。
        /// </summary>
        long Time { get; }
        /// <summary>
        /// 服务端消息时间戳
        /// </summary>
        long MessageTimestamp { get; }
        /// <summary>
        /// 内核输出关键字时间戳
        /// </summary>
        long OutputKeywordTimestamp { get; }
        /// <summary>
        /// <see cref="ServerNode.WsStatus"/>
        /// </summary>
        WsStatus WsStatus { get; }
        /// <summary>
        /// 指示客户端是否需要重新生成ClientId
        /// </summary>
        bool NeedReClientId { get; }
    }
}
