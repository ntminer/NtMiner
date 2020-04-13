using NTMiner.Core.Mq.MqMessagePaths;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace NTMiner {
    public interface IServerContext {
        ConnectionMultiplexer RedisConn { get; }
        IModel Channel { get; }
        IMqMessagePath[] MqMessagePaths { get; }
    }
}
