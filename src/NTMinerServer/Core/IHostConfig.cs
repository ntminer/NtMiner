using System;

namespace NTMiner.Core {
    public interface IHostConfig {
        /// <summary>
        /// 没有什么用，只是因为持久层需要一个Id
        /// </summary>
        Guid Id { get; }
        string Description { get; }
        string OssAccessKeyId { get; }
        string OssAccessKeySecret { get; }
        string OssEndpoint { get; }
        string KodoAccessKey { get; }
        string KodoSecretKey { get; }
        string KodoDomain { get; }
        string RedisConfig { get; }
        string MqHostName { get; }
        string MqUserName { get; }
        string MqPassword { get; }
        /// <summary>
        /// Ip:Port格式
        /// </summary>
        string ThisServerAddress { get; }
        ushort GetServerPort();
    }
}
