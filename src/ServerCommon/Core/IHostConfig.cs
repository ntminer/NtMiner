namespace NTMiner.Core {
    public interface IHostConfig {
        string OssAccessKeyId { get; }
        string OssAccessKeySecret { get; }
        string OssEndpoint { get; }
        string KodoAccessKey { get; }
        string KodoSecretKey { get; }
        string KodoZoneID { get; }
        string RedisConfig { get; }
        string MqHostName { get; }
        string MqUserName { get; }
        string MqPassword { get; }
        /// <summary>
        /// Ip:Port格式
        /// </summary>
        string ThisServerAddress { get; }
        /// <summary>
        /// WebApiServer的本地地址，用于内网调用节省外网带宽。
        /// </summary>
        string RpcServerLocalAddress { get; }
        string RpcLoginName { get; }
        string RpcPassword { get; }
        ushort GetServerPort();
    }
}
