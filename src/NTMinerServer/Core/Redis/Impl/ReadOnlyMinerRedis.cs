using NTMiner.Core.MinerServer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class ReadOnlyMinerRedis : IReadOnlyMinerRedis {
        protected const string _redisKeyMinerById = RedisKeyword.MinersMinerById;

        protected readonly IMqRedis _redis;
        public ReadOnlyMinerRedis(IMqRedis redis) {
            _redis = redis;
        }

        public Task<List<MinerData>> GetAllAsync() {
            var db = _redis.RedisConn.GetDatabase();
            return db.HashGetAllAsync(_redisKeyMinerById).ContinueWith(t => {
                List<MinerData> list = new List<MinerData>();
                foreach (var item in t.Result) {
                    if (item.Value.HasValue) {
                        MinerData data = VirtualRoot.JsonSerializer.Deserialize<MinerData>(item.Value);
                        if (data != null) {
                            list.Add(data);
                        }
                    }
                }
                return list;
            });
        }

        public Task<MinerData> GetByIdAsync(string minerId) {
            if (string.IsNullOrEmpty(minerId)) {
                return Task.FromResult<MinerData>(null);
            }
            var db = _redis.RedisConn.GetDatabase();
            return db.HashGetAsync(_redisKeyMinerById, minerId).ContinueWith(t => {
                if (t.Result.HasValue) {
                    return VirtualRoot.JsonSerializer.Deserialize<MinerData>(t.Result);
                }
                else {
                    return null;
                }
            });
        }
    }
}
