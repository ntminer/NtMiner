using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class ReadOnlyWsServerNodeRedis : IReadOnlyWsServerNodeRedis {
        protected const string _redisKeyWsServerNodeByAddress = RedisKeyword.WsServerNodesWsServerNodeByAddress;
        protected const string _redisKeyWsServerNodeAddress = RedisKeyword.WsServerNodesAddress;

        protected readonly IRedis _redis;
        public ReadOnlyWsServerNodeRedis(IRedis redis) {
            _redis = redis;
        }

        public Task<Dictionary<string, DateTime>> GetAllAddress() {
            var db = _redis.GetDatabase();
            return db.HashGetAllAsync(_redisKeyWsServerNodeAddress).ContinueWith(t => {
                Dictionary<string, DateTime> dic = new Dictionary<string, DateTime>();
                foreach (var item in t.Result) {
                    if (item.Value.HasValue && DateTime.TryParse(item.Value, out DateTime activeOn)) {
                        dic.Add(item.Name, activeOn);
                    }
                }
                return dic;
            });
        }
    }
}
