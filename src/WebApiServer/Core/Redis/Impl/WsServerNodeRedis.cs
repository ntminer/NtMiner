using NTMiner.ServerNode;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class WsServerNodeRedis : ReadOnlyWsServerNodeRedis, IWsServerNodeRedis {
        public WsServerNodeRedis(IRedis redis) : base(redis) {
        }

        public Task<List<WsServerNodeState>> GetAllAsync() {
            var db = _redis.GetDatabase();
            return db.HashGetAllAsync(_redisKeyWsServerNodeByAddress).ContinueWith(t => {
                List<WsServerNodeState> list = new List<WsServerNodeState>();
                foreach (var item in t.Result) {
                    if (item.Value.HasValue) {
                        WsServerNodeState data = VirtualRoot.JsonSerializer.Deserialize<WsServerNodeState>(item.Value);
                        if (data != null) {
                            list.Add(data);
                        }
                    }
                }
                return list;
            });
        }

        public Task ClearAsync(List<string> offlines) {
            if (offlines == null || offlines.Count == 0) {
                return TaskEx.CompletedTask;
            }
            var db = _redis.GetDatabase();
            var list = new List<RedisValue>();
            foreach (var item in offlines) {
                list.Add(item);
            }
            var hashFileds = list.ToArray();
            var t1 = db.HashDeleteAsync(_redisKeyWsServerNodeByAddress, hashFileds);
            var t2 = db.HashDeleteAsync(_redisKeyWsServerNodeAddress, hashFileds);
            return Task.WhenAll(t1, t2);
        }
    }
}
