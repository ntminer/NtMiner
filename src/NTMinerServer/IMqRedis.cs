using RabbitMQ.Client;
using StackExchange.Redis;

namespace NTMiner {
    public interface IMqRedis {
        ConnectionMultiplexer RedisConn { get; }
        IModel MqChannel { get; }
    }
}
