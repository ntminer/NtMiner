using RabbitMQ.Client;
using StackExchange.Redis;

namespace NTMiner {
    public interface IServerConnection {
        ConnectionMultiplexer RedisConn { get; }
        IModel Channel { get; }
    }
}
