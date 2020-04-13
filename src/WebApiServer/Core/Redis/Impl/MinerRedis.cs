using NTMiner.Core.MinerServer;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class MinerRedis : ReadOnlyMinerRedis, IMinerRedis {
        public MinerRedis(ConnectionMultiplexer connection) : base(connection) {
        }

        public Task SetAsync(MinerData data) {
            if (data == null || string.IsNullOrEmpty(data.Id)) {
                return TaskEx.CompletedTask;
            }
            var db = _connection.GetDatabase();
            return db.HashSetAsync(_redisKeyMinerById, data.Id, VirtualRoot.JsonSerializer.Serialize(data));
        }

        public Task DeleteAsync(MinerData data) {
            if (data == null || string.IsNullOrEmpty(data.Id)) {
                return TaskEx.CompletedTask;
            }
            var db = _connection.GetDatabase();
            return db.HashDeleteAsync(_redisKeyMinerById, data.Id);
        }
    }
}
