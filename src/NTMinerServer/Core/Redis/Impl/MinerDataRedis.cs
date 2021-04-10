using NTMiner.Core.MinerServer;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class MinerDataRedis : ReadOnlyMinerDataRedis, IMinerDataRedis {
        public MinerDataRedis(IRedis redis) : base(redis) {
        }

        public Task SetAsync(MinerData data) {
            if (data == null || string.IsNullOrEmpty(data.Id)) {
                return TaskEx.CompletedTask;
            }
            var db = _redis.GetDatabase();
            return db.HashSetAsync(_redisKeyMinerById, data.Id, VirtualRoot.JsonSerializer.Serialize(data));
        }

        public Task UpdateAsync(MinerSign minerSign) {
            if (minerSign == null) {
                return TaskEx.CompletedTask;
            }
            var db = _redis.GetDatabase();
            return db.HashGetAsync(_redisKeyMinerById, minerSign.Id).ContinueWith(t => {
                MinerData minerData = null;
                if (t.Result.HasValue) {
                    minerData = VirtualRoot.JsonSerializer.Deserialize<MinerData>(t.Result);
                    minerData?.Update(minerSign);
                }
                else {
                    minerData = MinerData.Create(minerSign);
                }
                if (minerData != null) {
                    db.HashSetAsync(_redisKeyMinerById, minerSign.Id, VirtualRoot.JsonSerializer.Serialize(minerSign));
                }
            });
        }

        public Task DeleteAsync(IMinerData data) {
            if (data == null || string.IsNullOrEmpty(data.Id)) {
                return TaskEx.CompletedTask;
            }
            var db = _redis.GetDatabase();
            return db.HashDeleteAsync(_redisKeyMinerById, data.Id);
        }

        public Task DeleteAsync(IEnumerable<IMinerData> datas) {
            if (datas == null) {
                return TaskEx.CompletedTask;
            }
            var db = _redis.GetDatabase();
            List<RedisValue> ids = new List<RedisValue>();
            foreach (var item in datas) {
                ids.Add(item.Id);
            }
            return db.HashDeleteAsync(_redisKeyMinerById, ids.ToArray());
        }
    }
}
