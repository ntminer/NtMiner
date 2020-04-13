namespace NTMiner.Core {
    public interface IHostConfig {
        string OssAccessKeyId { get; }
        string OssAccessKeySecret { get; }
        string OssEndpoint { get; }
        string RedisConfig { get; }
        string MqHostName { get; }
        string MqUserName { get; }
        string MqPassword { get; }
        string ThisServerAddress { get; }
        string RpcLoginName { get; }
        string RpcPassword { get; }
        ushort GetServerPort();
    }
}
