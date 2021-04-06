using NTMiner.ServerNode;
using System.Threading.Tasks;

namespace NTMiner.Core.Redis.Impl {
    public class ClientTestIdDataRedis : IClientTestIdDataRedis {
        protected const string _redisKeyClientTestId = RedisKeyword.ClientTestId;

        protected readonly IRedis _redis;
        public ClientTestIdDataRedis(IRedis redis) {
            _redis = redis;
        }

        public Task<ClientTestIdData> GetAsync() {
            var db = _redis.GetDatabase();
            return db.StringGetAsync(_redisKeyClientTestId).ContinueWith(t => {
                string json = t.Result;
                if (!string.IsNullOrEmpty(json)) {
                    return VirtualRoot.JsonSerializer.Deserialize<ClientTestIdData>(json);
                }
                return null;
            });
        }

        public Task SetAsync(ClientTestIdData clientTestId) {
            var db = _redis.GetDatabase();
            if (clientTestId == null) {
                return db.KeyDeleteAsync(_redisKeyClientTestId);
            }
            return db.StringSetAsync(_redisKeyClientTestId, VirtualRoot.JsonSerializer.Serialize(clientTestId));
        }
    }
}
