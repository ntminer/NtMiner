using StackExchange.Redis;

namespace NTMiner {
    public interface IRedis {
        ConnectionMultiplexer RedisConn { get; }
    }
}
