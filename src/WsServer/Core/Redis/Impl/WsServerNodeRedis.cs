using NTMiner.ServerNode;
using System;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class WsServerNodeRedis : ReadOnlyWsServerNodeRedis, IWsServerNodeRedis {
        public WsServerNodeRedis(IMqRedis redis) : base(redis) {
        }

        public Task SetAsync(WsServerNodeState data) {
            if (data == null || string.IsNullOrEmpty(data.Address)) {
                return TaskEx.CompletedTask;
            }
            var db = _redis.RedisConn.GetDatabase();
            var t1 = db.HashSetAsync(_redisKeyWsServerNodeByAddress, data.Address, VirtualRoot.JsonSerializer.Serialize(data));
            // 如果WsServer节点服务器的时间和WebApiServer相差很多则该WsServer节点会被WebApiServer节点从redis中删除从而下线该节点
            var t2 = db.HashSetAsync(_redisKeyWsServerNodeAddress, data.Address, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return Task.WhenAll(t1, t2);
        }

        public Task DeleteByAddressAsync(string address) {
            if (string.IsNullOrEmpty(address)) {
                return TaskEx.CompletedTask;
            }
            var db = _redis.RedisConn.GetDatabase();
            var t1 = db.HashDeleteAsync(_redisKeyWsServerNodeByAddress, address);
            var t2 = db.HashDeleteAsync(_redisKeyWsServerNodeAddress, address);
            return Task.WhenAll(t1, t2);
        }
    }
}
