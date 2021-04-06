using StackExchange.Redis;

namespace NTMiner.Core {
    public interface IRedis {
        IDatabase GetDatabase();
    }
}
