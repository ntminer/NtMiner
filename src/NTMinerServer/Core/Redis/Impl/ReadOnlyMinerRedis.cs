using NTMiner.Core.MinerServer;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class ReadOnlyMinerRedis : IReadOnlyMinerRedis {
        protected const string _redisKeyMinerById = "miners.MinerById";// 根据Id索引Miner对象的json

        protected readonly ConnectionMultiplexer _connection;
        public ReadOnlyMinerRedis(ConnectionMultiplexer connection) {
            _connection = connection;
        }

        public Task<List<MinerData>> GetAllAsync() {
            var db = _connection.GetDatabase();
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
            var db = _connection.GetDatabase();
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
