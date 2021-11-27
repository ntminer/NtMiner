using NTMiner.Core.MinerServer;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class CalcConfigDataRedis : ICalcConfigDataRedis {
        protected const string _redisKeyCalcConfigs = RedisKeyword.CalcConfigs;

        protected readonly IRedis _redis;
        public CalcConfigDataRedis(IRedis redis) {
            _redis = redis;
        }

        public Task<CalcConfigData[]> GetAllAsync() {
            var db = _redis.GetDatabase();
            return db.HashGetAllAsync(_redisKeyCalcConfigs).ContinueWith(t => {
                List<CalcConfigData> list = new List<CalcConfigData>();
                foreach (var item in t.Result) {
                    if (item.Value.HasValue) {
                        CalcConfigData data = VirtualRoot.JsonSerializer.Deserialize<CalcConfigData>(item.Value);
                        if (data != null) {
                            list.Add(data);
                        }
                    }
                }
                return list.ToArray();
            });
        }

        public Task SetAsync(List<CalcConfigData> data) {
            if (data == null || data.Count == 0) {
                return TaskEx.CompletedTask;
            }
            var db = _redis.GetDatabase();
            return db.HashKeysAsync(_redisKeyCalcConfigs).ContinueWith(t => {
                foreach (var key in t.Result) {
                    db.HashDelete(_redisKeyCalcConfigs, key);
                }
                List<HashEntry> entries = new List<HashEntry>();
                foreach (var item in data) {
                    entries.Add(new HashEntry(item.CoinCode, VirtualRoot.JsonSerializer.Serialize(item)));
                }
                db.HashSet(_redisKeyCalcConfigs, entries.ToArray());
            });
        }
    }
}
