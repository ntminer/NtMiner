using RabbitMQ.Client;
using StackExchange.Redis;

namespace NTMiner {
    public interface IServerConfig {
        ConnectionMultiplexer RedisConn { get; }
        IModel Channel { get; }
    }
}
