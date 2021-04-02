using StackExchange.Redis;

namespace NTMiner {
    public interface IRedis {
        IDatabase GetDatabase();
    }
}
